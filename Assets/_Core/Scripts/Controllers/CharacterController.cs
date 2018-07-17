using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Panda;

using RPG.Characters;
using System.Collections.Generic;

namespace RPG.Controllers
{
    public enum ECharacterType
    {
        Player,
        Enemy,
        NPC
    }

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

        private HealthSystem healthSystem;
        private WeaponSystem weaponSystem;
        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;

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

        Selectable selectable;

        List<cakeslice.Outline> outliners = new List<cakeslice.Outline>();

        #region MonoBehavior
        void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();     
            rigidBody = GetComponent<Rigidbody>();
            animator.speed = animationSpeed;
            animator.applyRootMotion = true;
            walkSpeed = characterConfig.GetWalkSpeed();
            runSpeed = characterConfig.GetRunSpeed();
            sprintSpeed = characterConfig.GetSprintSpeed();
            rotationSpeed = characterConfig.GetRotationSpeed();
            //navMeshAgent.updatePosition = manualPosition;
            //navMeshAgent.updateRotation = manualRotation;
        }

        void Start()
        {
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

            AddOutlinesToMeshes();
        }

        void Update()
        {
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
            UpdateRecoveryBar();
        }

        #endregion

        #region Selectable Events
        void OnSelected()
        {
            foreach (var outliner in outliners)
            {
                outliner.enabled = true;
            }
        }

        void OnDeselected()
        {
            foreach (var outliner in outliners)
            {
                outliner.enabled = false;
            }
        }

        void OnDeHighlight()
        {
            foreach (var outliner in outliners)
            {
                outliner.enabled = false;
            }
        }

        void OnHighlight()
        {
            foreach (var outliner in outliners)
            {
                outliner.enabled = true;
            }
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

        #endregion

        #region WeaponSystem Events

        public void OnAttackComplete(WeaponSystem weaponSystem)
        {
            Debug.LogFormat("{0} OnAttackComplete {1}", name, weaponSystem.name);

            currentRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds(); ;
        }

        public void OnHit(WeaponSystem weaponSystem, GameObject hitObject, float damage)
        {
            Debug.LogFormat("{0} OnHit {1} with {2} for {3} damage", name, hitObject.name, weaponSystem.name, damage);
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
            animator.applyRootMotion = true;

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
            objectTarget = target;
        }

        public void SetMovementTarget(Vector3 position)
        {
            movementTarget = position;
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
                    outliners.Add(outline);
                }
            }
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 0;
                    outliners.Add(outline);
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

            if (InAttackRange())
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

            if (InAttackRange())
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
            var distance = Vector3.Distance(transform.position, movementTarget);
            if (distance <= GetStoppingDistance())
            {
                Task.current.Succeed();
            }
            else
            {

               SetDestination(movementTarget);
            }
        }

        [Task]
        bool InAttackRange()
        {
            bool isInRange = false;

            if (weaponSystem != null)
            {
                float distance = Vector3.Distance(transform.position, movementTarget);

                var currentWeapon = weaponSystem.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    isInRange = distance <= currentWeapon.GetMaxAttackRange() ||
                                            Mathf.Approximately(distance, currentWeapon.GetMaxAttackRange());

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
        bool Target_Acquire()
        {
            // Just aquire the single player game object.
            var target = GameObject.FindGameObjectWithTag("Player");
            if (target == null)
            {
                return false;
            }

            SetObjectTarget(target);
            SetMovementTarget(target.transform.position);

            return true;
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

        #endregion

    }
}