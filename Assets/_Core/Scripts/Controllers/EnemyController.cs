using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

using Panda;
using UnityEngine.AI;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.Controllers
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] EnemyConfig enemyConfig;
        [SerializeField] Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);
        [SerializeField] Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] Image actionImage;

        public ECommandType command = ECommandType.Attack;

        public Queue<Command> commandQueue = new Queue<Command>();

        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;

        CharacterController character;
        public GameObject target;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        public float recoveryAsPercentage { get { return currentRecoveryTimeSeconds / maxRecoveryTimeSeconds; } }

        void Start()
        {
            character = GetComponent<CharacterController>();

            weaponSystem = GetComponent<WeaponSystem>();
            weaponSystem.onWeaponHit += OnWeaponHit;
            weaponSystem.target = null;
       
            if (enemyConfig == null)
                throw new Exception("enemyConfig can't be null!");

            minRecoveryTimeSeconds = character.GetCharacterConfig().GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;
        }

        void Update()
        {
           
        }

        private void LateUpdate()
        {
            UpdateRecoveryBar();
        }

        void QueueCommand(Command command)
        {
            commandQueue.Enqueue(command);
        }


        void UpdateRecoveryBar()
        {
            recoveryCircleImage.fillAmount = recoveryAsPercentage;
        }

        public WeaponSystem GetWeaponSystem()
        {
            return weaponSystem;
        }

        #region Tasks
        [Task]
        bool IsCurrentCommandDone()
        {
            if (commandQueue.Count == 0)
                return true;

            return commandQueue.Peek().CommandStatus == ECommandStatus.Done;
        }

        [Task]
        bool PopCommand()
        {
            if (commandQueue.Count == 0)
                return true;

            commandQueue.Dequeue();

            return true;
        }
        
        [Task]
        bool CommandDone()
        {
            if (commandQueue.Count == 0)
                return true;

            var command = commandQueue.Peek();
            command.CommandStatus = ECommandStatus.Done;

            return true;

        }

        [Task]
        bool IsCurrentCommand_Move()
        {
            if (commandQueue.Count == 0)
                return false;

            return commandQueue.Peek().CommandType == ECommandType.Move;
        }

        [Task]
        bool IsCurrentCommand_Attack()
        {
            if (commandQueue.Count == 0)
                return false;

            return commandQueue.Peek().CommandType == ECommandType.Attack;
        }
        [Task]
        bool Enemy_MoveCommand()
        {
            return command == ECommandType.Move;

        }

        [Task]
        bool Enemy_AttackCommand()
        {
            return command == ECommandType.Attack;
        }

        [Task]
        bool Enemy_HasTarget()
        {
            return target != null;
        }
        [Task]
        bool Enemy_SetRecoveryTimeSeconds(float seconds)
        {
            currentRecoveryTimeSeconds = seconds;
            return true;
        }

        [Task]
        void Enemy_WaitForRecovery()
        {
            // Recovery time is added to as actions are performed.  
            // This makes things really simple.
            currentRecoveryTimeSeconds -= Time.deltaTime;

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
        bool Enemy_StopMoving()
        {
            character.StopMoving();
    
            return true;
        }

        [Task]
        bool StopRotating()
        {
            return true;
        }

        [Task]
        bool Enemy_TargetIsAlive()
        {
            if (target == null)
                return false;

            var targetHealthsysytem = target.GetComponent<HealthSystem>();

            if (targetHealthsysytem == null)
                return false;

            return targetHealthsysytem.IsAlive();
        }

        [Task]
        bool Target_InAttackRange()
        {
            if (target == null)
                return false;

            bool isInRange = false;
         
            if (weaponSystem != null)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

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
        void Target_Acquire()
        {
            // Just aquire the single player game object.
            target = GameObject.FindGameObjectWithTag("Player");
            character.SetTarget(target);
            Task.current.Complete(target != null);
        }

        [Task]
        bool Enemy_SetSpeed(float speed)
        {
            character.SetMovementSpeed(speed);
            return true;
        }

        [Task]
        void Target_MoveTo()
        {

            actionImage.sprite = moveActionImage;
            var distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= character.GetStoppingDistance())
            {
                Task.current.Succeed();
            }
            else
            {
                
                character.SetDestination(target.transform.position);
            }
        }

        // Note that this is identical to MoveToRanged.  We will update them separately later on.
        [Task]
        void Target_MoveToMelee()
        {
            actionImage.sprite = moveActionImage;

            var distance = Vector3.Distance(transform.position, target.transform.position);

            weaponSystem.StopAttacking();

            if (Target_InAttackRange())
            {
                character.StopMoving();
                Task.current.Succeed();
            }
            else
            {
                character.SetDestination(target.transform.position);
            }
        }

        [Task]
        void Target_MoveToRanged()
        {
            actionImage.sprite = moveActionImage;

            weaponSystem.StopAttacking();

            var dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange() ||
                dist <= character.GetStoppingDistance())
            {
                Task.current.Succeed();
            }
            else
            {
                character.SetDestination(target.transform.position);
            }
        }

        [Task]
        void Enemy_Attack()
        {
            Debug.Log("Enemy_Attack");
            actionImage.sprite = attackActionImage;

            weaponSystem.SetTarget(target);
            weaponSystem.Attack();

     
            Debug.DrawLine(transform.position + (Vector3.up ) , target.transform.position + (Vector3.up), Color.green, 1.0f);

            // Reset recovery time.
            maxRecoveryTimeSeconds = weaponSystem.GetCurrentWeapon().GetRecoveryTimeSeconds();
            currentRecoveryTimeSeconds = maxRecoveryTimeSeconds;

            Task.current.Succeed();
        }

        #endregion

        void OnWeaponHit(WeaponSystem weaponSystem, GameObject hitObject, float damage)
        {
            Debug.LogFormat("{0} hit {1} with {2} for {3} damage", weaponSystem.name, hitObject.name, weaponSystem.GetCurrentWeapon().name, damage);
        }

        void OnDrawGizmos()
        {
         
        }

   
    }
}