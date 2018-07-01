using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

using Panda;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RPG.Characters
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] EnemyConfig enemyConfig;
        [SerializeField] Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);
        [SerializeField] Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] Image actionImage;

        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;

        Character character;
        public GameObject target;
        WeaponSystem weaponSystem;
        Selectable selectable;

        bool isAttacking = false;

        public float recoveryAsPercentage { get { return currentRecoveryTimeSeconds / maxRecoveryTimeSeconds; } }

        void Start()
        {
            character = GetComponent<Character>();
            selectable = GetComponent<Selectable>();

            weaponSystem = GetComponent<WeaponSystem>();
            weaponSystem.onWeaponHit += OnWeaponHit;
            weaponSystem.target = null;

            if (enemyConfig == null)
                throw new Exception("enemyConfig can't be null!");

            character.SetMovementSpeed(enemyConfig.GetMovementSpeed());
            minRecoveryTimeSeconds = enemyConfig.GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;
        }

        void Update()
        {
           
        }

        private void LateUpdate()
        {
            UpdateRecoveryBar();
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
            character.SetStopped(true);
    
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
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (weaponSystem != null)
            {
                var currentWeapon = weaponSystem.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    isInRange = distance <= currentWeapon.GetMaxAttackRange();
                }
            }

            return isInRange;
        }

        [Task]
        void Target_Acquire()
        {
            // Just aquire the single player game object.
            target = GameObject.FindGameObjectWithTag("Player");
            Task.current.Complete(target != null);
        }

        [Task]
        bool Enemy_SetSpeed(float speed)
        {
            character.SetMovementSpeed(speed);
            return true;
        }

        // Note that this is identical to MoveToRanged.  We will update them separately later on.
        [Task]
        void Target_MoveToMelee()
        {
            actionImage.sprite = moveActionImage;

            var dist = Vector3.Distance(transform.position, target.transform.position);
       
            weaponSystem.StopAttacking();

            if (dist <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange() ||
                dist <= character.StoppingDistance())
            {
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
                dist <= character.StoppingDistance())
            {
                Task.current.Succeed();
            }
            else
            {
                character.SetDestination(target.transform.position);
            }
        }

        [Task]
        void Target_MoveTo()
        {
            var dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange() ||
                dist <= character.StoppingDistance())
            {
                Task.current.Succeed();
                character.SetStopped(true);
            }
            else
            {
                character.SetDestination(target.transform.position);
            }
        }

        [Task]
        void Enemy_Attack()
        {
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