using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public enum EWeaponType
    {
        Melee,
        Ranged
    }

    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;
        public Transform muzzleTipTransform;

        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] bool useOtherHand;
        [SerializeField] EWeaponType weaponType;
        [SerializeField] float additionalDamage = 0.0f;
        [SerializeField] float damageDelay = 0.25f;
        [SerializeField] bool aimWeapon;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] float baseReloadSpeed;

        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public bool GetAimWeapon()
        {
            return aimWeapon;
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

        public EWeaponType GetWeaponType()
        {
            return weaponType;
        }
    }
}
