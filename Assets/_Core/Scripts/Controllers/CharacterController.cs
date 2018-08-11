using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Panda;

using RPG.Characters;
using System.Collections.Generic;
using Pathfinding;
using System;

namespace RPG.Character
{
    public enum ECharacterType
    {
        Player,
        Enemy,
        NPC
    }

    public enum ECharacterSize
    {
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan
    }

    [ExecuteInEditMode]
    public class CharacterController : MonoBehaviour
    {
        [Header("RPG Character")]
        public ECharacterType characterType;
        [SerializeField] CharacterConfig characterConfig;
        [SerializeField] Animator animator;
        [SerializeField] Rigidbody rigidBody;
        [SerializeField] AIPath aiAgent;

        float currentForwardSpeed;
        public float CurrentForwardSpeed
        {
            get
            {
                return currentForwardSpeed;
            }
            set
            {
                currentForwardSpeed = value;
                aiAgent.maxSpeed = currentForwardSpeed;
            }
        }

        float currentSideSpeed;
        public float CurrentSideSpeed
        {
            get
            {
                return currentSideSpeed;
            }
            set
            {
                currentSideSpeed = value;
            }
        }

        [Task]
        public bool SetWalking()
        {
            CurrentForwardSpeed = MaxForwardSpeed * 0.2f;
            CurrentSideSpeed = MaxSideSpeed * 0.2f;
            return true;
        }

        [Task]
        public bool SetRunning()
        {
            CurrentForwardSpeed = MaxForwardSpeed * 0.8f;
            CurrentSideSpeed = MaxSideSpeed * 0.8f;
            return true;
        }

        [Task]
        public bool SetSprinting()
        {
            CurrentForwardSpeed = MaxForwardSpeed;
            CurrentSideSpeed = MaxForwardSpeed;
            return true;
        }

        [SerializeField] float maxForwardSpeed;
        public float MaxForwardSpeed
        {
            get
            {
                return maxForwardSpeed;
            }
            set
            {
                maxForwardSpeed = value;
                aiAgent.maxSpeed = maxForwardSpeed;
            }
        }

        [SerializeField] float maxSideSpeed;
        public float MaxSideSpeed
        {
            get
            {
                return maxSideSpeed;
            }
            set
            {
                maxSideSpeed = value;
                // TODO: what to do about this for an aiAgent?
                // Probably don't need to worry about it since side movement will not be done with the AI.
                // aiAgent.maxSpeed = maxForwardSpeed;
            }
        }


        [SerializeField] float maxRotationSpeed;
        public float MaxRotationSpeed
        {
            get
            {
                return maxRotationSpeed;
            }
            set
            {
                maxRotationSpeed = value;
                aiAgent.rotationSpeed = maxRotationSpeed;
            }
        }

        [SerializeField] bool manualRotation = false;
        [SerializeField] bool manualPosition = true;
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] ECharacterSize characterSize = ECharacterSize.Medium;
        [SerializeField] float animationSpeed;
        
        // Render Texture for portrait camera
        public RenderTexture portraitTexture;

        // Targeting
        public GameObject targetCursor = null;
        public GameObject target = null;

        // Testing
        public Vector3 homePosition;

        private Selectable selectable;
        private HealthSystem healthSystem;
        private WeaponSystem weaponSystem;
        private CommandSystem commandSystem;

        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;
        private float idleWaitTime = 0.0f;

        [Header("Gizmos")]
        [SerializeField] Color boundingBoxColor = Color.white;

        #region MonoBehavior
        void Awake()
        {
            maxForwardSpeed = characterConfig.GetMaxForwardSpeed();
            maxSideSpeed = characterConfig.GetMaxSideSpeed();
            maxRotationSpeed = characterConfig.GetMaxRotationSpeed();
            characterSize = characterConfig.GetSize();

            var collider = GetComponent<BoxCollider>();
            collider.center = characterConfig.GetCharacterCenter();
            collider.size = characterConfig.GetCharacterSize();

            aiAgent = GetComponent<AIPath>();
            aiAgent.maxSpeed = maxForwardSpeed;
            aiAgent.rotationSpeed = maxRotationSpeed;

            rigidBody = GetComponent<Rigidbody>();

            animator = GetComponent<Animator>();
            animator.speed = animationSpeed;
            animator.applyRootMotion = false;
           
            healthSystem = GetComponent<HealthSystem>();
            weaponSystem = GetComponent<WeaponSystem>();

            var portraitCamera = GetComponentInChildren<Camera>();
            if (portraitCamera)
            {
                portraitTexture = new RenderTexture(512, 512, 32, RenderTextureFormat.ARGB32);
                portraitTexture.Create();
                portraitTexture.name = string.Format("{0} Portrait Texture", this.name);
                portraitCamera.targetTexture = portraitTexture;
            }

            commandSystem = GetComponent<CommandSystem>();
        }

        private void OnEnable()
        {
            if (healthSystem)
            {
                healthSystem.onDamage += OnDamage;
                healthSystem.onHeal += OnHeal;
            }
            if (weaponSystem)
            {
                weaponSystem.onAttackComplete += OnAttackComplete;
                weaponSystem.onHit += OnHit;
            }
        }

        private void OnDisable()
        {
            if (healthSystem)
            {
                healthSystem.onDamage -= OnDamage;
                healthSystem.onHeal -= OnHeal;
            }
            if (weaponSystem)
            {
                weaponSystem.onAttackComplete -= OnAttackComplete;
                weaponSystem.onHit -= OnHit;
            }
        }

        void Start()
        {
            homePosition = transform.position;
            SetOutlinesEnabled(false);
            minRecoveryTimeSeconds = characterConfig.GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;
            SetRunning();
            RegisterSelectableEventHandlers();
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;

            UpdateAnimator();
        }

        #endregion

        #region Selectable Events
        void OnSelected()
        {
            SetOutlinesEnabled(true);
        }

        void OnDeselected()
        {
            SetOutlinesEnabled(false);
        }

        void OnDeHighlight()
        {
            if (!selectable.isSelected)
            {
                SetOutlinesEnabled(false);
            }
        }

        void OnHighlight()
        {
            SetOutlinesEnabled(true);
        }
        #endregion

        #region Animation Events
        private void HandleHitAndShootAnimationEvents()
        {
            // TODO: we are telling the target to run their GetHitTrigger animation.
            // We may need to use a component here, or something, since telling
            // it directly to their animator here seems brittle (but it works.)
            var targetAnimator = target.GetComponent<Animator>();
            if (targetAnimator == null)
                return;

            int numPossibleHits = 5;
            int hitNumber = UnityEngine.Random.Range(1, numPossibleHits + 1);

            targetAnimator.SetInteger("Action", hitNumber);
            targetAnimator.SetTrigger("GetHitTrigger");
        }

        public void Shoot()
        {
            HandleHitAndShootAnimationEvents();
        }

        public void Hit()
        {
            HandleHitAndShootAnimationEvents();
        }

        public void FootL()
        {

        }

        public void FootR()
        {

        }
        #endregion

        #region WeaponSystem Events
        public void OnAttackComplete(WeaponSystem weaponSystem, GameObject hitObject)
        {
            // This is called AFTER DamageAfterDelay!!!
            currentRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds(); ;
        }

        public void OnHit(WeaponSystem weaponSystem, GameObject hitObject, float damage)
        {
            Debug.LogFormat("{0} hit {1} with {2} for {3} damage", name, hitObject.name, weaponSystem.GetCurrentWeapon().GetWeaponName(), damage);
            hitObject.GetComponent<Animator>().applyRootMotion = false;
        }
        #endregion

        #region HealthSystem Events
        void OnDamage(float damageAmount)
        {
            bool characterDies = healthSystem.HealthAsPercentage <= Mathf.Epsilon;

            if (characterDies)
            {
                OnDeath();
            }
        }

        void OnHeal(float healAmount)
        {

        }

        // Note that we just call this internally -- it is not an event handler.
        void OnDeath()
        {
            currentRecoveryTimeSeconds = 0.0f;
            maxRecoveryTimeSeconds = 0.0f;

            // Disable the world UI
            var worldUI = GetComponentInChildren<Canvas>();
            if (worldUI)
            {
                worldUI.enabled = false;
            }

            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            aiAgent.enabled = false;
            GetComponent<Seeker>().enabled = false;

            var circleProjector = GetComponentInChildren<Projector>();
            if (circleProjector)
            {
                circleProjector.enabled = false;
            }

            // Trigger the death animation
            animator.SetTrigger("Death1Trigger");

            // Destroy us for good 1 minutes later.
            Destroy(gameObject, 60);
        }

        #endregion

        #region Movement
        public float GetStoppingDistance()
        {
            return 0;
        }

        [Task]
        public bool IsAlive()
        {
            return healthSystem != null && healthSystem.IsAlive();
        }

        public void SetDestination(Vector3 worldPosition)
        {
            aiAgent.isStopped = false;
            aiAgent.destination = worldPosition;
        }

        public void RotateTowardsPosition(Vector3 position)
        {
            if (manualRotation)
            {
                Vector3 targetDir = (position - transform.position).normalized;
                if (targetDir != Vector3.zero)
                {
                    var lookRotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * maxRotationSpeed);
                }
            }
        }

        public void RotateTowardsTarget(GameObject target)
        {
            RotateTowardsPosition(target.transform.position);
        }
        #endregion

        #region Targets
        public GameObject GetTarget()
        {
            return target;
        }

        public void SetTargetCursorWorldPosition(Vector3 worldPosition)
        {
            targetCursor.transform.parent = null;

            // Move the target cursor to worldPosition
            targetCursor.transform.position = worldPosition;
        }

        // TODO: might not need this.  we currently just move the target around by a worldPosition.
        public void AttachTargetCursorTo(GameObject targetGameObject)
        {
            // attach our target cursor to the targetGameObject
            //targetCursor.transform.SetParent(targetGameObject.transform);
        }

        public void ResetTargetCursor()
        {
            targetCursor.transform.SetParent(transform);
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public void ClearTarget()
        {
            ResetTargetCursor();
            this.target = null;
        }

        #endregion

        #region Size
        public ECharacterSize GetCharacterSize()
        {
            return characterSize;
        }

        public float GetCharacterRadius()
        {
            float radius = 0.0f;

            if (characterSize == ECharacterSize.Small)
            {
                radius = 0.5f;
            }
            else if (characterSize == ECharacterSize.Medium)
            {
                radius = 1.0f;
            }
            else if (characterSize == ECharacterSize.Large)
            {
                return 3.0f;
            }
            else if (characterSize == ECharacterSize.Gargantuan)
            {
                return 10.0f;
            }

            return radius;
        }
        #endregion

        // CharacterConfig
        public CharacterConfig GetCharacterConfig()
        {
            return characterConfig;
        }

        public float GetCurrentRecoveryTime()
        {
            return currentRecoveryTimeSeconds;
        }
        public float GetMaxRecoveryTime()
        {
            return maxRecoveryTimeSeconds;
        }
        public float RecoveryAsPercentage { get { return currentRecoveryTimeSeconds / maxRecoveryTimeSeconds; } }

        void RegisterSelectableEventHandlers()
        {
            selectable = GetComponent<Selectable>();

            if (selectable != null)
            {
                selectable.onSelected += OnSelected;
                selectable.onDeselected += OnDeselected;
                selectable.onHighlight += OnHighlight;
                selectable.onDeHighlight += OnDeHighlight;
            }

            OnDeselected();
            OnDeHighlight();
        }

        public void SetOutlinesEnabled(bool enabled)
        {
            foreach (var outliner in GetComponentsInChildren<cakeslice.Outline>())
            {
                outliner.enabled = enabled;
            }
        }

        void UpdateAnimator()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);

            // Side velocity
            float velocityX = transform.InverseTransformDirection(aiAgent.velocity).x / maxSideSpeed;

            // Forward velocity
            float velocityZ = transform.InverseTransformDirection(aiAgent.velocity).z / maxForwardSpeed;

            Vector3 animatorVelocity = new Vector3(velocityX, 0.0f, velocityZ);

            if (animatorVelocity.magnitude > 0.01f)
            {
                animator.SetBool("Moving", true);
                animator.SetFloat("Velocity Z", animatorVelocity.z, 0.1f, Time.deltaTime);
                animator.SetFloat("Velocity X", animatorVelocity.x, 0.1f, Time.deltaTime);
            }
            else
            {
                animator.SetBool("Moving", false);
                animator.SetFloat("Velocity Z", 0);
                animator.SetFloat("Velocity X", 0);
            }

         }

        #region Tasks

        [Task]
        bool SetRecoveryTimeSeconds(float seconds)
        {
            currentRecoveryTimeSeconds = seconds;
            return true;
        }
        
        [Task]
        public bool StopMoving()
        {
            aiAgent.isStopped = true;

            return true;
        }

        // TODO: i feel having a separate one of these for target vs target cursor is bad design, but whatever...
        [Task]
        bool IsAtTargetCursor()
        {
            return Vector3.Distance(transform.position, targetCursor.transform.position) <= aiAgent.endReachedDistance;
        }

        [Task]
        bool IsAtTarget()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= aiAgent.endReachedDistance;
        }

        [Task]
        bool IsWithinWeaponRange()
        {
            bool isInRange = false;

            if (weaponSystem != null)
            {
                var currentWeapon = weaponSystem.GetCurrentWeapon();

                float distance = 0.0f;

                if (target != null)
                {
                    distance = Vector3.Distance(transform.position, target.transform.position);
                }
                else
                {
                    distance = Vector3.Distance(transform.position, targetCursor.transform.position);
                }

                if (currentWeapon != null)
                {
                    isInRange = distance <= currentWeapon.GetWeaponRange();

                }
            }

            return isInRange;
        }

        [Task]
        void MoveToTargetCursor()
        {
            SetDestination(targetCursor.transform.position);
        }

        // TODO: so this works, but it is not pretty, at all.
        // I think I need to re-implement a stop moving task.
        [Task]
        bool MoveAttack()
        {
            if (IsWithinWeaponRange())
            {
                aiAgent.isStopped = true;
                return true;
            }

            aiAgent.isStopped = false;

            Vector3 worldPosition = Vector3.zero;

            if (target != null)
            {
                SetTargetCursorWorldPosition(target.transform.position);
            }

            SetDestination(targetCursor.transform.position);

            return IsWithinWeaponRange();
        }

        [Task]
        bool MoveToTarget()
        {
            aiAgent.isStopped = false;

            Vector3 worldPosition = Vector3.zero;

            if (target != null)
            {
                SetTargetCursorWorldPosition(target.transform.position);
            }
 
            SetDestination(targetCursor.transform.position);

            return IsAtTarget();
        }

        [Task]
        bool Idle()
        {
            return true;
        }


        [Task]
        bool HasTarget()
        {
            return target != null;
        }

        [Task]
        bool TargetIsAlive()
        {
            if (target == null)
                return false;

            var targetHealthsysytem = target.GetComponent<HealthSystem>();
            return targetHealthsysytem && targetHealthsysytem.IsAlive();
        }

        [Task]
        bool LookAtTarget()
        {
            // TODO: simplify this to just use the targetCursor.  Note that we only update the position
            // of the targetCursor occasionally, so this does not work well unless the targetCursor follows a character.
            if (target)
            {
                Quaternion lookAtRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, aiAgent.rotationSpeed * Time.deltaTime);
            }

            return true;
        }

        [Task]
        void WaitForRandomTime()
        {
            if (Task.current.isStarting)
            {
                idleWaitTime = UnityEngine.Random.Range(1.0f, 2.5f);
            }

            if (Task.isInspected)
            {
                float tta = Mathf.Clamp(idleWaitTime, 0.0f, float.PositiveInfinity);
                Task.current.debugInfo = string.Format("t-{0:0.000}", tta);
            }

            idleWaitTime -= Time.deltaTime;

            if (idleWaitTime <= 0)
            {
                idleWaitTime = 0;
                Task.current.Succeed();
            }
        }

        [Task]
        public void WaitForRecovery()
        {
            if (!weaponSystem.isAttacking)
            {
                currentRecoveryTimeSeconds -= Time.deltaTime;
            }

            if (Task.isInspected)
            {
                float tta = Mathf.Clamp(currentRecoveryTimeSeconds, 0.0f, float.PositiveInfinity);
                Task.current.debugInfo = string.Format("t-{0:0.000}", tta);
            }

            if (currentRecoveryTimeSeconds <= 0)
            {
                currentRecoveryTimeSeconds = 0;
                Task.current.Succeed();
            }
        }

        [Task]
        void ChooseRandomPosition()
        {
            var point = UnityEngine.Random.insideUnitSphere * 25f;
            point.y = 0;
            point += aiAgent.position;

            SetTargetCursorWorldPosition(point);
            aiAgent.SearchPath();

            Task.current.Succeed();
        }

        [Task]
        bool AllEnemiesDead()
        {
            var enemies = FindObjectsOfType<EnemyController>();

            foreach (var enemy in enemies)
            {
                if (enemy.GetComponent<HealthSystem>().IsAlive())
                {
                    return false;
                }
            }

            return true;
        }

        [Task]
        bool AllHeroesDead()
        {
            var heroes = FindObjectsOfType<HeroController>();

            foreach (var hero in heroes)
            {
                if (hero.GetComponent<HealthSystem>().IsAlive())
                {
                    return false;
                }
            }

            return true;
        }

        [Task]
        bool FindThreatenedHeroes()
        {
            var heroes = FindObjectsOfType<HeroController>();
            var enemies = FindObjectsOfType<EnemyController>();

            HealthSystem lowestHealthHero = null;

            foreach (var hero in heroes)
            {
                var healthSystem = hero.GetComponent<HealthSystem>();
                if (healthSystem)
                {
                    if (lowestHealthHero == null)
                    {
                        lowestHealthHero = healthSystem;
                        continue;
                    }

                    if (healthSystem.GetCurrentHealth() <= lowestHealthHero.GetCurrentHealth())
                    {
                        lowestHealthHero = healthSystem;
                    }
                }
            }

            if (lowestHealthHero != null)
            {
                HealthSystem lowestHealthEnemy = null;

                foreach (var enemy in enemies)
                {
                    var enemyHealthSystem = enemy.GetComponent<HealthSystem>();

                    if (enemy.GetComponent<CharacterController>().GetTarget() != lowestHealthHero.gameObject)
                        continue;

                    if (lowestHealthEnemy == null)
                    {
                        lowestHealthEnemy = enemyHealthSystem;
                        continue;
                    }
               
                    if (enemyHealthSystem.GetCurrentHealth() <= lowestHealthEnemy.GetCurrentHealth())
                    {
                        lowestHealthEnemy = enemyHealthSystem;
                    }
                }

                if (lowestHealthEnemy)
                {
                    SetTarget(lowestHealthEnemy.gameObject);
                    weaponSystem.SetTarget(lowestHealthEnemy.gameObject);
                    SetTargetCursorWorldPosition(lowestHealthEnemy.transform.position);
                    return true;
                }
            }

            return false;
        }

        private float chooseRandomTime = 5.0f;

        [Task]
        bool ChooseRandomEnemy()
        {
            chooseRandomTime -= Time.deltaTime;

            if (chooseRandomTime <= 0)
            {
                chooseRandomTime = 5.0f;
                return true;
            }


            return false;
        }

        bool ChooseRandomHero()
        {
            return true;
        }

        [Task]
        bool FindClosestEnemy()
        {
            var enemies = FindObjectsOfType<EnemyController>();

            float shortestDistance = Mathf.Infinity;
            GameObject closest = null;

            foreach (var enemy in enemies)
            {
                var healthSystem = enemy.GetComponent<HealthSystem>();
                if (healthSystem == null)
                    continue;

                if (healthSystem.IsAlive())
                {
                    var dist = Vector3.Distance(transform.position, healthSystem.transform.position);

                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        closest = enemy.gameObject;
                    }
                }
            }

            if (closest != null)
            {
                SetTarget(closest);
                SetTargetCursorWorldPosition(closest.transform.position);
                weaponSystem.SetTarget(closest);

                return true;
            }
            else
            {
                return false;
            }
        }

        [Task]
        bool FindClosestHero()
        {
            var heroes = FindObjectsOfType<HeroController>();

            float shortestDistanceToFriendly = Mathf.Infinity;
            GameObject closestHero = null;

            foreach (var friendly in heroes)
            {
                var healthSystem = friendly.GetComponent<HealthSystem>();
                if (healthSystem == null)
                    continue;

                if (healthSystem.IsAlive())
                {
                    var dist = Vector3.Distance(transform.position, healthSystem.transform.position);

                    if (dist < shortestDistanceToFriendly)
                    {
                        shortestDistanceToFriendly = dist;
                        closestHero = friendly.gameObject;
                    }
                }
            }

            if (closestHero != null)
            {
                SetTarget(closestHero);
         
                SetTargetCursorWorldPosition(closestHero.transform.position);

                weaponSystem.SetTarget(closestHero);
                return true;
            }
            else
            {
                return false;
            }
        }

        [Task]
        void Attack()
        {
            weaponSystem.SetTarget(target);
      
            SetTargetCursorWorldPosition(target.transform.position);

            maxRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds();
            currentRecoveryTimeSeconds = maxRecoveryTimeSeconds;

            weaponSystem.Attack();
    
            Task.current.Succeed();
        }

     
        [Task]
        void SetTargetToHome()
        {
            SetTargetCursorWorldPosition(homePosition);
            target = targetCursor;

            Task.current.Succeed();
        }

        #endregion

        #region Gizmos

        void OnDrawGizmos()
        {
            if (IsAlive())
            {
                var previousColor = Gizmos.color;

                var boxCollider = GetComponent<BoxCollider>();
                if (boxCollider)
                {
                    Gizmos.color = boundingBoxColor;
                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
                    Gizmos.matrix = Matrix4x4.TRS(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.transform.rotation, boxCollider.transform.lossyScale);
                    Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
                    Gizmos.matrix = oldGizmosMatrix;
                    Gizmos.color = previousColor;
                }
            }
        }
        #endregion

    }
}