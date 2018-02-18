using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] AnimationClip idleAnimation;
        [SerializeField] bool useOtherHand;

        [SerializeField] float additionalDamage = 0.0f;
        [SerializeField] float damageDelay = 0.25f;

        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public bool GetUseOtherHand()
        {
            return useOtherHand;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        // TODO consider wether we take animation time into account
        public float GetMinTimeBetweenHits()
        {
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetIdleAnimationClip()
        {
            return idleAnimation;
        }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();

            return attackAnimation;
        }

        // So that asset packs cannot cause crashes/bugs.
        private void RemoveAnimationEvents()
        {
            if (attackAnimation == null)
                return;

            attackAnimation.events = new AnimationEvent[0];
        }
    }
}
