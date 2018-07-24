using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Panda;

using RPG.Characters;
using System.Collections.Generic;

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
        [SerializeField] NavMeshAgent navMeshAgent;
        [SerializeField] float walkSpeed = 1.35f;
        [SerializeField] float runSpeed = 6.0f;
        [SerializeField] float sprintSpeed = 6.0f;
        [SerializeField] float rotationSpeed = 40f;
        [SerializeField] bool manualRotation = false;
        [SerializeField] bool manualPosition = true;
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] Image actionImage;
        [SerializeField] ECharacterSize characterSize = ECharacterSize.Medium;
        
        private HealthSystem healthSystem;
        private WeaponSystem weaponSystem;
        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;
        private bool isSelected;

        [Header("Animation Pack Stuff")]
        public bool crouch;
        public bool useMeshNav;
        public bool isMoving = false;
        public bool canMove = true;
        Vector3 inputVec;
        Vector3 newVelocity;
        public bool onAllowableSlope;
        public bool isSprinting;

        //Rolling.
        public float rollSpeed = 8;
        bool isRolling = false;
        public float rollduration;

        //Weapon and Shield.
        //public Weapon weapon = Weapon.RELAX;

        [HideInInspector]
        bool isSwitchingFinished = true;

        //Weapon Parameters.
        [HideInInspector]
        public int rightWeapon = 0;
        [HideInInspector]
        public int leftWeapon = 0;
        bool weaponSwitch;
        public bool instantWeaponSwitch;
        public bool dualSwitch;

        //Strafing/action.
        public bool hipShooting = false;
        [HideInInspector]
        public bool canAction = true;
        bool isStrafing = false;
        [HideInInspector]
        public bool isDead = false;

        [HideInInspector]
        public bool isBlocking = false;
        public float knockbackMultiplier = 1f;
        bool isKnockback;
        [HideInInspector]
        public bool isSitting = false;
        bool isAiming = false;
        [HideInInspector]
        public bool
        isClimbing = false;
        [HideInInspector]
        public bool
        isNearLadder = false;
        [HideInInspector]
        public bool isNearCliff = false;
        [HideInInspector]
        public GameObject ladder;
        [HideInInspector]
        public GameObject cliff;
        [HideInInspector]
        public bool isCasting;
        public int specialAttack = 0;
        public float aimHorizontal;
        public float aimVertical;
        public float bowPull;
        bool injured;
        public bool headLook = false;
        bool isHeadlook = false;
        public int numberOfConversationClips;
        int currentConversation;
        float idleTimer;
        float idleTrigger = 0f;

        // Input
        bool inputJump;
        bool inputLightHit;
        bool inputDeath;
        bool inputUnarmed;
        bool inputShield;
        bool inputAttackL;
        bool inputAttackR;
        bool inputCastL;
        bool inputCastR;
        float inputSwitchUpDown;
        float inputSwitchLeftRight;
        bool inputStrafe;
        float inputTargetBlock = 0;
        float inputDashVertical = 0;
        float inputDashHorizontal = 0;
        float inputHorizontal = 0;
        float inputVertical = 0;
        bool inputAiming;
        public float animationSpeed = 1;

        bool isAlive = true;
        float turnAmount;
        float forwardAmount;


        GameObject objectTarget;
        Vector3 movementTarget;
        Vector3 homePosition;
        bool isAtObjectTarget;

        Selectable selectable;

        [Header("Gizmos")]
        [SerializeField] Color boundingBoxColor = Color.white;

        #region MonoBehavior
        void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();     
            rigidBody = GetComponent<Rigidbody>();
            animator.speed = animationSpeed;
            animator.applyRootMotion = false;
            walkSpeed = characterConfig.GetWalkSpeed();
            runSpeed = characterConfig.GetRunSpeed();
            navMeshAgent.speed = runSpeed;
            sprintSpeed = characterConfig.GetSprintSpeed();
            rotationSpeed = characterConfig.GetRotationSpeed();

            characterSize = characterConfig.GetSize();
            var collider = GetComponent<BoxCollider>();
            collider.center = characterConfig.GetCharacterCenter();
            collider.size = characterConfig.GetCharacterSize();

            //navMeshAgent.updatePosition = manualPosition;
            //navMeshAgent.updateRotation = manualRotation;
        }

        void Start()
        {
            //if (!Application.isPlaying)
            //    return;

            homePosition = transform.position;
       
            AddOutlinesToMeshes();
            SetOutlinesEnabled(false);

            healthSystem = GetComponent<HealthSystem>();
            if (healthSystem)
            {
                healthSystem.onDamage += OnDamage;
                healthSystem.onHeal += OnHeal;
            }

            weaponSystem = GetComponent<WeaponSystem>();
            if (weaponSystem)
            {
                weaponSystem.onAttackComplete += OnAttackComplete;
                weaponSystem.onHit += OnHit;
            }

            minRecoveryTimeSeconds = characterConfig.GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;

            RegisterSelectableEventHandlers();
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;

            navMeshAgent.speed = runSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;

            if (manualRotation)
            {
                if (movementTarget != null)
                {
                    Vector3 targetPosition;

                    if (navMeshAgent.path.corners.Length > 1)
                    {
                        var pathWaypoint = navMeshAgent.path.corners[1];
                        if (pathWaypoint != null)
                            targetPosition = pathWaypoint;
                        else
                            targetPosition = movementTarget;
                    }
                    else
                    {
                        targetPosition = movementTarget;
                    }

                    RotateTowardsPosition(targetPosition);
                }
            }

            UpdateAnimator();
        }

        void LateUpdate()
        {
            if (!Application.isPlaying)
                return;

            UpdateRecoveryBar();
        }

        #endregion

        #region Selectable Events
        void OnSelected()
        {
            isSelected = true;
            SetOutlinesEnabled(true);
        }

        void OnDeselected()
        {
            isSelected = false;
            SetOutlinesEnabled(false);
        }

        void OnDeHighlight()
        {
            if (!isSelected)
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
            int numPossibleHits = 5;
            var targetAnimator = objectTarget.GetComponent<Animator>();
            int hitNumber = Random.Range(1, numPossibleHits + 1);
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
           // Debug.LogFormat("{0} OnAttackComplete {1}", name, weaponSystem.name);
            currentRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds(); ;
            // TODO: fixme -- wrong place to do this.
           // hitObject.GetComponent<Animator>().applyRootMotion = true;
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
            // Disable the world UI
            var worldUI = GetComponentInChildren<Canvas>();
            if (worldUI)
            {
                worldUI.enabled = false;
            }

            navMeshAgent.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
     
            var circleProjector = GetComponentInChildren<Projector>();
            if (circleProjector)
            {
                circleProjector.enabled = false;
            }

            // Trigger the death animation
            animator.SetTrigger("Death1Trigger");

            // Destroy us for good 2 minutes later.
            Destroy(gameObject, 120);
        }

        #endregion

        #region Movement
        public void SetMovementSpeed(float newSpeed)
        {
            navMeshAgent.speed = newSpeed;
        }

        public float GetStoppingDistance()
        {
            return navMeshAgent.stoppingDistance;
        }

        public bool IsAlive()
        {
            return healthSystem != null && healthSystem.HealthAsPercentage > 0;
        }

        public void SetDestination(Vector3 worldPosition)
        {
       //     animator.applyRootMotion = true;

            navMeshAgent.isStopped = false;
            navMeshAgent.destination = worldPosition;
        }

        public void RotateTowardsPosition(Vector3 position)
        {
            if (manualRotation)
            {
                Vector3 targetDir = (position - transform.position).normalized;
                var lookRotation = Quaternion.LookRotation(new Vector3(targetDir.x, 0, targetDir.z));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }

        public void RotateTowardsTarget(GameObject target)
        {
            RotateTowardsPosition(target.transform.position);
        }
        #endregion

        #region Targets
        public void SetObjectTarget(GameObject target)
        {
            isAtObjectTarget = false;
            objectTarget = target;
        }

        public void SetMovementTarget(Vector3 position)
        {
            isAtObjectTarget = false;
            movementTarget = position;
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

        #region Collider
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == objectTarget)
            {
                isAtObjectTarget = true;
                StopMoving();
            }
        }

        private void OnTriggerExit(Collider other)
        {
        
        }
        #endregion

        // CharacterConfig
        public CharacterConfig GetCharacterConfig()
        {
            return characterConfig;
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

        void SetOutlinesEnabled(bool enabled)
        {
            foreach (var outliner in GetComponentsInChildren<cakeslice.Outline>())
            {
                outliner.enabled = enabled;
            }
        }

        void AddOutlinesToMeshes()
        {
            // Outline stuff.
            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mesh in skinnedMeshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 0;
                }
            }
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 0;
                }
            }
        }

        void UpdateAnimator()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);

            float velocityXel = transform.InverseTransformDirection(navMeshAgent.velocity).x;
            float velocityZel = transform.InverseTransformDirection(navMeshAgent.velocity).z;

            animator.SetFloat("Velocity Z", velocityZel);
            animator.SetBool("Moving", navMeshAgent.velocity.sqrMagnitude > 0);
        }

        void UpdateRecoveryBar()
        {
            recoveryCircleImage.fillAmount = RecoveryAsPercentage;
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
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            animator.applyRootMotion = false;
            return true;
        }

        [Task]
        public void MoveToRanged()
        {
            actionImage.sprite = moveActionImage;
            var distance = Vector3.Distance(transform.position, movementTarget);

            if (weaponSystem != null)
            {
                weaponSystem.StopAttacking();
            }

            if (IsWithinWeaponRange())
            {
                StopMoving();
                Task.current.Succeed();
            }
            else
            {
                SetDestination(movementTarget);
            }
        }

        [Task]
        public void MoveToMelee()
        {
            actionImage.sprite = moveActionImage;
            var distance = Vector3.Distance(transform.position, movementTarget);

            if (weaponSystem != null)
            {
                weaponSystem.StopAttacking();
            }

            if (IsWithinWeaponRange())
            {
                StopMoving();
                Task.current.Succeed();
            }
            else
            {
                SetDestination(movementTarget);
            }
        }

        [Task]
        void MoveToTargetPosition()
        {
            actionImage.sprite = moveActionImage;

            if (isAtObjectTarget)
            {
                Task.current.Succeed();
            }
            else
            {
                SetDestination(movementTarget);
            }

            //var distance = Vector3.Distance(transform.position, movementTarget);
            //if (distance <= GetStoppingDistance())
            //{
            //    Task.current.Succeed();
            //}
            //else
            //{
            //   SetDestination(movementTarget);
            //}
        }

        [Task]
        bool Idle()
        {
            return true;
        }

        [Task]
        bool IsWithinWeaponRange()
        {
            bool isInRange = false;

            if (weaponSystem != null)
            {
                var currentWeapon = weaponSystem.GetCurrentWeapon();

                if (isAtObjectTarget && currentWeapon.GetWeaponRange() == 0)
                    return true;

                float distance = Vector3.Distance(transform.position, movementTarget);

                if (currentWeapon != null)
                {
                    isInRange = distance <= currentWeapon.GetWeaponRange() ||
                                            Mathf.Approximately(distance, currentWeapon.GetWeaponRange());

                }
            }

            return isInRange;
        }

        [Task]
        bool HasTarget()
        {
            return objectTarget != null;
        }

        [Task]
        bool TargetIsAlive()
        {
            bool isAlive = false;

            var targetHealthsysytem = objectTarget.GetComponent<HealthSystem>();

            if (targetHealthsysytem)
            {
                isAlive = targetHealthsysytem.IsAlive();
                if (!isAlive)
                {
                    SetObjectTarget(null);
     
                }
            }

            return isAlive;
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
        bool FindCombatTarget()
        {
            SetObjectTarget(null);

            var friendlies = FindObjectsOfType<HeroController>();

            float shortestDistanceToFriendly = Mathf.Infinity;
            GameObject closestFriendly = null;

            foreach (var friendly in friendlies)
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
                        closestFriendly = friendly.gameObject;
                    }
                }
            }

            if (closestFriendly != null)
            {
                SetObjectTarget(closestFriendly.gameObject);
                SetMovementTarget(closestFriendly.gameObject.transform.position);
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
            actionImage.sprite = attackActionImage;

            weaponSystem.SetTarget(objectTarget);
            weaponSystem.Attack();

            Debug.DrawLine(transform.position + (Vector3.up), objectTarget.transform.position + (Vector3.up), Color.green, 1.0f);

            maxRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds();
            currentRecoveryTimeSeconds = maxRecoveryTimeSeconds;

            Task.current.Succeed();
        }

        [Task]
        bool SetSpeed(float speed)
        {
            SetMovementSpeed(speed);
            return true;
        }

        [Task]
        void SetTargetToHome()
        {
            SetMovementTarget(homePosition);
            Task.current.Succeed();
        }
        #endregion

        #region Gizmos
        
        void OnDrawGizmos()
        {
            var previousColor = Gizmos.color;
            Gizmos.color = boundingBoxColor;
            var boxCollider = GetComponent<BoxCollider>();
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.transform.rotation, boxCollider.transform.lossyScale);
            Gizmos.DrawWireCube(Vector3.zero, boxCollider.size);
            Gizmos.matrix = oldGizmosMatrix;    
            Gizmos.color = previousColor;
        }
        #endregion

    }
}