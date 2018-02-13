using UnityEngine.Assertions;

using UnityEngine;
using System.Collections;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;

        // todo consider the arguments here.  do we need more details about the hit?
        // todo consider just removing this -- its confusing.
        public delegate void OnWeaponHit(WeaponSystem weaponSystem, GameObject hitObject, float damage);
        public event OnWeaponHit onWeaponHit;

        float lastHitTime = 0f;
        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        bool isAttacking = false;

        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig);

            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                float targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position); 
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }

            var characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = characterHealth <= Mathf.Epsilon;

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }

        public GameObject GetWeaponObject()
        {
            return weaponObject;
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();

            Destroy(weaponObject);

            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.transform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.transform.localRotation;
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        public void Attack()
        {
            StartCoroutine(AttackRepeatedly());
        }

        IEnumerator AttackRepeatedly()
        {
            float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
            float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();
            bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

            if (isTimeToHitAgain)
            {
                AttackOnce();
            }

            yield return new WaitForSeconds(timeToWait);
        }

        void AttackOnce()
        {
            isAttacking = true;

            animator.SetTrigger(ATTACK_TRIGGER);

            // todo get from the weapon itself;
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            lastHitTime = Time.time;

            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }


        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly()
        {
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                }

                yield return new WaitForSeconds(timeToWait);
            }
        }

        void AttackTargetOnce()
        {
            transform.LookAt(target.transform);

            animator.SetTrigger(ATTACK_TRIGGER);

            isAttacking = true;

            // todo get from the weapon itself;
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            lastHitTime = Time.time;

            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
            Debug.DrawLine(GetWeaponObject().transform.position + Vector3.up, target.transform.position, Color.green, 2.0f);
            isAttacking = false;
        }

        void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + " with an animator override controller ");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController();

                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
            }
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;

            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts on player.  Only 1 allowed.");

            return dominantHands[0].gameObject;
        }

        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }

        private void OnTriggerEnter(Collider other)
        {
            var healthSystem = other.GetComponent<HealthSystem>();

            if (healthSystem && this.transform != other.transform)
            {
                if (!isAttacking)
                    return;

                var damage = CalculateDamage();
            
                healthSystem.TakeDamage(damage);

                if (onWeaponHit != null)
                {
                    onWeaponHit(this, healthSystem.gameObject, damage);
                }
            }
        }

    }
}
