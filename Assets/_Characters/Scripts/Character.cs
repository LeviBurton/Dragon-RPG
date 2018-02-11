using UnityEngine;
using UnityEngine.AI;

using RPG.CameraUI;
using System;

namespace RPG.Characters
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField]float audioSourceSpatialBlend;
            
        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar characterAvatar;
        [SerializeField] [Range(0.1f, 1)] float animatorForwardCap = 1.0f;

        [Header("Capsule Collider")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 1, 0);
        [SerializeField] float colliderHeight = 2.0f;
        [SerializeField] float colliderRadius = 0.2f;

        [Header("Movement")]
        [SerializeField] bool lockMoveSpeedToAnimationSpeed = true;
        [SerializeField] float moveSpeedMultiplier = 1.0f;
        [SerializeField] float animationSpeedMultiplier = 1.5f;
        [SerializeField] float movingTurnSpeed = 360;
        [SerializeField] float stationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1.0f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentSteeringSpeed = 1.0f;
        [SerializeField] float navMeshStoppingDistance = 1.3f;
        [SerializeField] bool updateNavPosition = true;
        [SerializeField] bool updateNavRotation = false;

        Animator animator;
        Rigidbody rigidBody;
        NavMeshAgent navMeshAgent;
        bool isAlive = true;
        float turnAmount;
        float forwardAmount;
        
        void Awake()
        {
            AddRequiredComponents();     
        }

        void AddRequiredComponents()
        {
            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = characterAvatar;

            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;
            capsuleCollider.isTrigger = false;

            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.updatePosition = updateNavPosition;
            navMeshAgent.updateRotation = updateNavRotation;
            navMeshAgent.stoppingDistance = navMeshStoppingDistance;
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
            navMeshAgent.autoBraking = false;
        }

        void Start()
        { 
        }

        void Update()
        {
            if (updateNavPosition == true)
            {
                if (navMeshAgent.remainingDistance > navMeshStoppingDistance && isAlive)
                {
                    Move(navMeshAgent.desiredVelocity);
                }
                else
                {
                    Move(Vector3.zero);
                }
            }
        }

        public float GetAnimSpeedMultiplier()
        {
            return animator.speed;
        }

        public void ControllerMove(Vector3 moveDirection)
        {
            Move(moveDirection * moveSpeedMultiplier);
        }

        public void SetDestination(Vector3 worldPosition)
        {
            navMeshAgent.SetDestination(worldPosition);
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void Kill()
        {
            isAlive = false;
        }
      
        void SetForwardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }

            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void UpdateAnimator()
        {
            animator.speed = lockMoveSpeedToAnimationSpeed ? moveSpeedMultiplier : animationSpeedMultiplier;
            animator.SetFloat("Forward", forwardAmount * animatorForwardCap, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        }

        void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                velocity.y = rigidBody.velocity.y;
                rigidBody.velocity = velocity;
            }
        }

      
    }
}