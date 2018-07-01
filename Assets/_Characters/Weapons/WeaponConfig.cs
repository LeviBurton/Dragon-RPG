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
        // Transform applied to the weapon when attached to a game object
        public Transform gripTransform;

        // Where the tip of a firearm/ranged weapon is.
        public Transform muzzleTipTransform;

        // The character will use this to play the correct weapon animations associated with this weapon.
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        // This will get added to the weapon holders recovery time in seconds
        [SerializeField] float recoveryTimeSeconds = 1.0f;

        // How long an attack takes before the recovery period begins.
        // Right now, this is going to get hard coded to the weapon animation length,
        // since I can't figure out a way to get this reliably at runtime.
        [SerializeField] float attackSpeedSeconds = 1.0f;

        // Attack range in units (meters)
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] bool useOtherHand;
        [SerializeField] EWeaponType weaponType;
        [SerializeField] float additionalDamage = 0.0f;

        [SerializeField] bool aimWeapon;
        [SerializeField] GameObject particlePrefab;
        [SerializeField] float baseReloadSpeed;

        public AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public float GetRecoveryTimeSeconds()
        {
            return recoveryTimeSeconds;
        }

        public float GetAttackSpeedSeconds()
        {
            return attackSpeedSeconds;
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

        public float GetAdditionalDamage()
        {
            return additionalDamage;
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
