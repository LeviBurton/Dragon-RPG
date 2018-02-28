using UnityEngine.Assertions;

using UnityEngine;
using System.Collections;

// todo: need to re-architect weapon firing/attacking mechanics.
// we also need to handle blocking!
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

        float lastAttackTime = 0f;
        public GameObject target = null;
        GameObject weaponObject;
        Animator animator;
        Character character;
        public bool isAttacking = false;
        float animStartTime;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();
        }

        void Start()
        {
            PutWeaponInHand(currentWeaponConfig, currentWeaponConfig.GetUseOtherHand());

            SetAnimatorOverrideController();
        }

        void Update()
        {
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

        // todo: should we use the instance target?
        public bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            return distanceToTarget <= GetCurrentWeapon().GetMaxAttackRange();
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
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
            isAttacking = false;
            StopAllCoroutines();
        }

        public void AutoAttack()
        {
            if (isAttacking)
            {
                return;
            }

            isAttacking = true;
            StartCoroutine(AttackLoop());
        }

        public void Attack()
        {
            AttackOnce();
        }

        IEnumerator AttackLoop()
        {
            while (isAttacking)
            {
                AttackOnce();

                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        void AttackOnce()
        {
            float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
            float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();
            bool canAttackAgain = false;

            // special case for never having attacked.  seems smelly.
            if (lastAttackTime == 0)
            {
                canAttackAgain = true;
            }
            else
            {
                canAttackAgain = Time.time - lastAttackTime > timeToWait;
            }

            if (canAttackAgain)
            {
                lastAttackTime = Time.time;
                var particlePrefab = currentWeaponConfig.GetParticlePrefab();
                
                // todo: how to support multiple attack types?
                animator.SetTrigger("Attack");

                if (particlePrefab != null)
                {
                    var muzzleTip = weaponObject.transform.Find("MuzzleTip");

                    var particleObject = Instantiate(particlePrefab, muzzleTip.transform);
                    particleObject.transform.localPosition = muzzleTip.transform.localPosition;
                    particleObject.transform.localRotation = muzzleTip.transform.localRotation;
                    particleObject.GetComponent<ParticleSystem>().Play();

                    StartCoroutine(DestroyParticleWhenFinished(particleObject));
                }

                if (target != null)
                {
                    float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
                    if (distanceToTarget <= currentWeaponConfig.GetMaxAttackRange())
                    {
                        // todo: check to make sure we hit by rolling a to-hit roll, etc.
                        StartCoroutine(DamageAfterDelay(currentWeaponConfig.GetDamageDelay()));
                    }
                }
            }
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            // Pause for delay time to better simulate when a weapon attack actually hits the target
            // think of a sword swing for example -- we dont want to damage on the wind up, etc.
            yield return new WaitForSeconds(delay);

            if (target == null)
            {
                yield return new WaitForEndOfFrame();
            }

            var targetHealthComponent = target.GetComponent<HealthSystem>();
            if (targetHealthComponent != null)
            {
                var damage = CalculateDamage();

                targetHealthComponent.TakeDamage(damage);

                // notify anyone that we have a hit.
                onWeaponHit(this, target, damage);

                // Debug.DrawLine(GetWeaponObject().transform.position + Vector3.up, target.transform.position, Color.green, 2.0f);
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

        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(3f);
            }

            Destroy(particlePrefab);

            yield return new WaitForEndOfFrame();
        }

        // todo: this was for more precise weapon collision detection.
        // for now we are just going to simply check an attack score vs the target.
        //private void OnTriggerEnter(Collider other)
        //{
        //    var healthSystem = other.GetComponent<HealthSystem>();

        //    if (healthSystem && this.transform != other.transform)
        //    {
        //        if (!isAttacking)
        //            return;

        //        var damage = CalculateDamage();

        //        healthSystem.TakeDamage(damage);

        //        if (onWeaponHit != null)
        //        {
        //            onWeaponHit(this, healthSystem.gameObject, damage);
        //        }
        //    }
        //}

    }
}
