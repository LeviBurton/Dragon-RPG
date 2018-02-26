using UnityEngine.Assertions;

using UnityEngine;
using System.Collections;

// todo: need to re-architect weapon firing/attacking mechanics.
// the current system isn't really built around the concept of weapons
// such as semi-automatic, burst fire or fully automatic.
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
        GameObject target = null;
        GameObject weaponObject;
        Animator animator;
        Character character;
        public bool isAttacking = false;
        float animStartTime;

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig, currentWeaponConfig.GetUseOtherHand());

            SetAnimatorOverrideController();
        }

        void Update()
        {
            CheckIfShouldStopAttacking();
        }

        private void CheckIfShouldStopAttacking()
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

        public void SetAnimatorOverrideController()
        {
            var animatorOverrideController = currentWeaponConfig.GetAnimatorOverrideController();
            
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public GameObject GetWeaponObject()
        {
            return weaponObject;
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse, bool useOtherHand = false)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();

            GameObject handToPutIn;

            if (useOtherHand)
            {
                handToPutIn = RequestOtherHand();
            }
            else
            {
                handToPutIn = RequestDominantHand();
            }


            Destroy(weaponObject);

            weaponObject = Instantiate(weaponPrefab, handToPutIn.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.transform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.transform.localRotation;

        }

        public void SetAiming(bool Aim)
        {
            animator.SetBool("Aim", Aim);
        }

        public void Reload()
        {
            StopAttacking();
            SetAiming(false);
            animator.SetTrigger("Reload");
        }

        public void StopAttacking()
        {
            StopAllCoroutines();
        }

        public void AutoAttack()
        {
            StartCoroutine(AttackLoop());
        }

        public void Attack()
        {
            // todo: weapon rearchitecture starts here..
            // it really ooils down to: is this a looping animation, or not?
            AttackLoop();
        }

        IEnumerator AttackLoop()
        {
            float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
            isAttacking = true;
            float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();

            while (isAttacking)
            {
                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackOnce();
                    lastHitTime = Time.time;
                }

                yield return new WaitForSeconds(timeToWait);
                animator.ResetTrigger("Attack");
            }

            yield return null;
        }

        void AttackOnce()
        {
            // todo get from the weapon itself;
            float damageDelay = currentWeaponConfig.GetDamageDelay();

            animator.SetTrigger("Attack");

            var particlePrefab = currentWeaponConfig.GetParticlePrefab();

            // todo -- consider renaming this stuff since its specific to things with muzzles
            if (particlePrefab != null)
            {
                var muzzleTip = weaponObject.transform.Find("MuzzleTip");

                var particleObject = Instantiate(particlePrefab, muzzleTip.transform);
                particleObject.transform.localPosition = muzzleTip.transform.localPosition;
                particleObject.transform.localRotation = muzzleTip.transform.localRotation;
             
                particleObject.GetComponent<ParticleSystem>().Play();

                StartCoroutine(DestroyParticleWhenFinished(particleObject));
            }
           //StartCoroutine(DamageAfterDelay(damageDelay));
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

            animator.SetTrigger("Attack");

            isAttacking = true;
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            lastHitTime = Time.time;

            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target == null)
                yield return null;

            var targetHealthComponent = target.GetComponent<HealthSystem>();
            if (targetHealthComponent != null)
            {
                targetHealthComponent.TakeDamage(CalculateDamage());
                Debug.DrawLine(GetWeaponObject().transform.position + Vector3.up, target.transform.position, Color.green, 2.0f);
            }
        }


        GameObject RequestOtherHand()
        {
            var otherHands = GetComponentsInChildren<OtherHand>();
            int numberOfOtherHands = otherHands.Length;

            Assert.IsFalse(numberOfOtherHands > 1, "Multiple dominant hand scripts on player.  Only 1 allowed.");

            return otherHands[0].gameObject;
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

        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(20f);
            }

            Destroy(particlePrefab);

            yield return new WaitForEndOfFrame();
        }
    }
}
