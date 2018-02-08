using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;
        [SerializeField] float additionalDamage = 0.0f;

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

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();

            return attackAnimation;
        }

        // So that asset packs cannot cause crashes/bugs.
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}
