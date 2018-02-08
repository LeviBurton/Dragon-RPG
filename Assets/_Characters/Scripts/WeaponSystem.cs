using UnityEngine.Assertions;

using UnityEngine;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;

        float lastHitTime = 0f;
        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;

        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        const string ATTACK_TRIGGER = "Attack";

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;

            print("Attacking " + targetToAttack);

            // todo use a repeat attack co-routine
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

        void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            var animatorOverrideController = character.GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts on player.  Only 1 allowed.");
            return dominantHands[0].gameObject;
        }

        void AttackTarget()
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBetweenHits())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }

        float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }
    }
}
