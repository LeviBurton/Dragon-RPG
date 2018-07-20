using UnityEngine.Assertions;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RPG.Characters
{
    public class AttackAnimationRanges
    {
        public EWeaponType weaponType;
        public EHand hand;

        public int min;
        public int max;

        public AttackAnimationRanges(EWeaponType weaponType, EHand hand, int min, int max)
        {
            this.weaponType = weaponType;
            this.hand = hand;
            this.min = min;
            this.max = max;
        }

        public int GetRandomAttack()
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
    }

    public class WeaponSystem : MonoBehaviour
    {
        private static List<AttackAnimationRanges> attackAnimationRanges = new List<AttackAnimationRanges>();

        [SerializeField] float baseDamage = 10f;

        [SerializeField] WeaponConfig leftHandWeaponConfig;
        [SerializeField] WeaponConfig rightHandWeaponConfig;

        [SerializeField] WeaponConfig currentWeaponConfig;
 
        // todo consider the arguments here.  do we need more details about the hit?
        // todo consider just removing this -- its confusing.
        public delegate void OnHit(WeaponSystem weaponSystem, GameObject hitObject, float damage);
        public event OnHit onHit;

        public delegate void OnAttackComplete(WeaponSystem weaponSystem, GameObject hitObject);
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
            if (attackAnimationRanges.Count == 0)
            {
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Sword, EHand.Two, 1, 11));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Club, EHand.Two, 1, 11));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Spear, EHand.Two, 1, 11));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Axe, EHand.Two, 1, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Bow, EHand.Two, 1, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Two_Hand_Crossbow, EHand.Two, 1, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Staff, EHand.Two, 1, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Sword, EHand.Left, 1, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Sword, EHand.Right, 7, 13));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Pistol, EHand.Left, 1, 3));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Pistol, EHand.Right, 4, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Mace, EHand.Left, 1, 3));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Mace, EHand.Right, 4, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Dagger, EHand.Left, 1, 3));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Dagger, EHand.Right, 4, 6));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Item, EHand.Left, 1, 4));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Item, EHand.Right, 5, 9));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Unarmed, EHand.Left, 1, 3));
                attackAnimationRanges.Add(new AttackAnimationRanges(EWeaponType.Unarmed, EHand.Right, 4, 6));
            }

            character = GetComponent<CharacterController>();
        }

        void Start()
        {
            animator = GetComponent<Animator>();

            if (currentWeaponConfig != null)
            {
                EquipWeapon(currentWeaponConfig);

                // TODO: replace this with our left and right weaponConfig
                PutWeaponInHand(currentWeaponConfig, currentWeaponConfig.GetUseOtherHand());
            }
        }

        public void EquipWeapon(WeaponConfig config, EHand hand = EHand.Right)
        {
            if (currentWeaponConfig != config)
                currentWeaponConfig = config;

            EWeaponType weaponType = currentWeaponConfig.GetWeaponType();

            animator.SetInteger("LeftRight", 0);
            animator.SetInteger("LeftWeapon", 0);
            animator.SetInteger("RightWeapon", 0);

            switch (weaponType)
            {
                case EWeaponType.Two_Hand_Sword:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDSWORD);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDSWORD);
                    break;

                case EWeaponType.Two_Hand_Club:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDCLUB);  
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDCLUB);
                    break;

                case EWeaponType.Two_Hand_Spear:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDSPEAR);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDSPEAR);
                    break;

                case EWeaponType.Two_Hand_Axe:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDAXE);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDAXE);
                    break;

                case EWeaponType.Two_Hand_Bow:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDBOW);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDBOW);
                    break;

                case EWeaponType.Two_Hand_Crossbow:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.TWOHANDCROSSBOW);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.TWOHANDCROSSBOW);
                    break;

                case EWeaponType.Staff:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.STAFF);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationType.STAFF);
                    break;

                case EWeaponType.Dagger:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.ONEHANDED);
                    animator.SetInteger("LeftRight", (int)hand);
                    if (hand == EHand.Left)
                    {
                        animator.SetInteger("LeftWeapon", (int)EWeaponAnimationArmedType.LEFT_DAGGER);
                    }
                    else if (hand == EHand.Right)
                    {
                        animator.SetInteger("RightWeapon", (int)EWeaponAnimationArmedType.RIGHT_DAGGER);

                    }
                    break;

                case EWeaponType.Sword:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.ONEHANDED);
                    animator.SetInteger("LeftRight", (int)hand);
                    if (hand == EHand.Left)
                    {
                        animator.SetInteger("LeftWeapon", (int)EWeaponAnimationArmedType.LEFT_SWORD);
                    }
                    else if (hand == EHand.Right)
                    {
                        animator.SetInteger("RightWeapon", (int)EWeaponAnimationArmedType.RIGHT_SWORD);
    
                    }
                    break;

                case EWeaponType.Mace:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.ONEHANDED);
                    animator.SetInteger("LeftRight", (int)hand);
                    if (hand == EHand.Left)
                    {
                        animator.SetInteger("LeftWeapon", (int)EWeaponAnimationArmedType.LEFT_MACE);
                    }
                    else if (hand == EHand.Right)
                    {
                        animator.SetInteger("RightWeapon", (int)EWeaponAnimationArmedType.RIGHT_MACE);

                    }
                    break;

                case EWeaponType.Pistol:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.ONEHANDED);
                    animator.SetInteger("LeftRight", (int)hand);
                    if (hand == EHand.Left)
                    {
                        animator.SetInteger("LeftWeapon", (int)EWeaponAnimationArmedType.LEFT_PISTOL);
                    }
                    else if (hand == EHand.Right)
                    {
                        animator.SetInteger("RightWeapon", (int)EWeaponAnimationArmedType.RIGHT_PISTOL);

                    }
                    break;

                case EWeaponType.Unarmed:
                    animator.SetInteger("Weapon", (int)EWeaponAnimationType.UNARMED);
                    animator.SetInteger("LeftRight", (int)hand);
                    animator.SetInteger("AttackSide", (int)hand);
                    animator.SetInteger("LeftWeapon", (int)EWeaponAnimationArmedType.UNARMED);
                    animator.SetInteger("RightWeapon", (int)EWeaponAnimationArmedType.UNARMED);

                    break;

                default:
                    break;
            }

            animator.SetTrigger("InstantSwitchTrigger");


            Debug.LogFormat("Weapon: {0}", animator.GetInteger("Weapon"));
            Debug.LogFormat("LeftRight: {0}", animator.GetInteger("LeftRight"));
            Debug.LogFormat("RightWeapon: {0}", animator.GetInteger("RightWeapon"));
            Debug.LogFormat("LeftWeapon: {0}", animator.GetInteger("LeftWeapon"));
        }

        public void UnequipWeapon(EHand hand)
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

        // TODO: Add which hand parameter
        public void Attack()
        {
            isAttacking = true;

            // Note this stuff is just to deal with the complete 
            // hack of a state machine 
            animator.applyRootMotion = false;

            // TODO: Fix this for left and right weaponConfigs
            animator.SetInteger("AttackSide", (int)EHand.Right);

            EWeaponType weaponType = GetCurrentWeapon().GetWeaponType();

            var query = attackAnimationRanges.Where(x => x.weaponType == weaponType && (x.hand == EHand.Right || x.hand == EHand.Two));
            var ranges = query.SingleOrDefault();
            int attackNumber = ranges.GetRandomAttack();
            Debug.LogFormat("{0} {1}: ", Enum.GetName(typeof(EWeaponType), weaponType), attackNumber);

            animator.SetInteger("Action", ranges.GetRandomAttack());
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
            var damage = CalculateDamage();
            onHit(this, target, damage);

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
                targetHealthComponent.Damage(damage);
            }

            isAttacking = false;

            onAttackComplete(this, target);
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
            return currentWeaponConfig.GetBaseDamage();
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
