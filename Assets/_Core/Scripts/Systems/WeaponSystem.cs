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
        public delegate void OnHit(WeaponSystem weaponSystem, GameObject hitObject, float damage);
        public event OnHit onHit;

        public delegate void OnAttackComplete(WeaponSystem weaponSystem);
        public event OnAttackComplete onAttackComplete;



        float lastAttackTime = 0f;
        public GameObject target = null;
        GameObject weaponObject;
        Animator animator;
        CharacterController character;
        public bool isAttacking = false;
        float animStartTime;

        private void Awake()
        {
            character = GetComponent<CharacterController>();
        }

        void Start()
        {
            if (currentWeaponConfig != null)
            {
                animator = GetComponent<Animator>();
                animator.SetInteger("Weapon", -2);

                animator.SetTrigger("InstantSwitchTrigger");
                animator.SetInteger("LeftRight", (int)EHand.Right);
                animator.SetInteger("Weapon", (int) currentWeaponConfig.GetWeaponAnimatorType());
                animator.SetInteger("RightWeapon", (int)currentWeaponConfig.GetArmedWeapon());
                animator.SetInteger("LeftWeapon", 0);
                PutWeaponInHand(currentWeaponConfig, currentWeaponConfig.GetUseOtherHand());
            }
        }

        void Update()
        {

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
            weaponObject.transform.localScale = new Vector3(1, 1, 1);
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


        public void StopAttacking()
        {
            isAttacking = false;
            StopAllCoroutines();
        }

        public void Attack()
        {
            isAttacking = true;

            // Note this stuff is just to deal with the complete 
            // hack of a state machine 
            animator.applyRootMotion = false;
            animator.SetInteger("AttackSide", (int)EHand.Right);
            int attackNumber = 0;
            EArmedWeapon armedWeapon = GetCurrentWeapon().GetArmedWeapon();
            EWeaponType weaponType = GetCurrentWeapon().GetWeaponAnimatorType();

            if (weaponType == EWeaponType.TWOHANDSWORD)
            {
                attackNumber = Mathf.RoundToInt(Random.Range(1, 11));
            }
            else
            {
                if (armedWeapon == EArmedWeapon.RIGHT_SWORD)
                {
                    attackNumber = Random.Range(8, 14);
                }
                else if (armedWeapon == EArmedWeapon.RIGHT_MACE)
                {
                    attackNumber = Random.Range(4, 7);
                }
                else if (armedWeapon == EArmedWeapon.RIGHT_PISTOL)
                {
                    attackNumber = Random.Range(4, 7);
                }
            }

            animator.SetInteger("Action", attackNumber);
            animator.SetTrigger("AttackTrigger");

            var particlePrefab = currentWeaponConfig.GetParticlePrefab();

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
                StartCoroutine(DamageAfterDelay(currentWeaponConfig.GetAttackSpeedSeconds()));
            }

        }

        IEnumerator DamageAfterDelay(float delay)
        {
            // Pause for delay time to better simulate when a weapon attack actually hits the target
            // think of a sword swing for example -- we dont want to damage on the wind up, etc.
            yield return new WaitForSeconds(delay);
            animator.applyRootMotion = true;
            if (target == null)
            {
                yield return new WaitForEndOfFrame();
            }

            var targetHealthComponent = target.GetComponent<HealthSystem>();
            if (targetHealthComponent != null)
            {
                var damage = CalculateDamage();

                targetHealthComponent.Damage(damage);

                // notify anyone that we have a hit.
                onHit(this, target, damage);
            }

            isAttacking = false;

            onAttackComplete(this);
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

        //Placeholder functions for Animation events.
        public void Hit()
        {
        }

        public void Shoot()
        {
        }

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Land()
        {
        }


    }
}
