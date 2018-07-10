using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
using System;
using System.Collections;
using Panda;

using RPG.Characters;

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

        private HealthSystem healthSystem;
        private WeaponSystem weaponSystem;

        // Events 
        public delegate void OnSelected(CharacterController character);
        public event OnSelected onSelected;

        public delegate void OnDeselected(CharacterController character);
        public event OnDeselected onDeselected;

        public delegate void OnDeath(CharacterController character);
        public event OnDeath onDeath;

        public delegate void OnSpawn(CharacterController character);
        public event OnSpawn onSpawn;

        // todo consider the arguments here.  do we need more details about the hit?
        // todo consider just removing this -- its confusing.
        public delegate void OnWeaponHit(WeaponSystem weaponSystem, GameObject hitObject, float damage);
        public event OnWeaponHit onWeaponHit;

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
        GameObject currentTarget;

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
            weaponSystem = GetComponent<WeaponSystem>();
        }

        private void Update()
        {
            navMeshAgent.speed = runSpeed;
            navMeshAgent.angularSpeed = rotationSpeed;
      
            if (manualRotation)
            {
                if (currentTarget != null)
                {
                    Vector3 targetPosition;

                    if (navMeshAgent.path.corners.Length > 1)
                    {
                        var pathWaypoint = navMeshAgent.path.corners[1];
                        if (pathWaypoint != null)
                            targetPosition = pathWaypoint;
                        else
                            targetPosition = currentTarget.transform.position;
                    }
                    else
                    {
                        targetPosition = currentTarget.transform.position;
                    }

                   // RotateTowardsPosition(navMeshAgent.steeringTarget);

                     RotateTowardsPosition(targetPosition);
                }
            }

            UpdateAnimator();
        }

        void UpdateAnimator()
        {
            animator.SetFloat("AnimationSpeed", animationSpeed);

            float velocityXel = transform.InverseTransformDirection(navMeshAgent.velocity).x;
            float velocityZel = transform.InverseTransformDirection(navMeshAgent.velocity).z;

            animator.SetFloat("Velocity Z", velocityZel);
            animator.SetBool("Moving", navMeshAgent.velocity.sqrMagnitude > 0);
        }
    
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }

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
            return healthSystem != null && healthSystem.healthAsPercentage > 0;
        }

        public void StopMoving()
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
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

        // CharacterConfig
        public CharacterConfig GetCharacterConfig()
        {
            return characterConfig;
        }

    }
}