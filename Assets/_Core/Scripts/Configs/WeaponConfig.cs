using RPG.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public enum EHand
    {
        Two = 0,    // Two handed weapons
        Left = 1,
        Right = 2,
        Dual = 3,   // dual-wield wepaons
    }

    public enum EWeaponType
    {
        Unarmed,
        Two_Hand_Sword,
        Two_Hand_Spear,
        Two_Hand_Axe,
        Two_Hand_Bow,
        Two_Hand_Crossbow,
        Two_Hand_Club,
        Staff,
        Sword,
        Mace,
        Dagger,
        Item,
        Pistol,
        Rifle,
    }

    public enum EWeaponAnimationType
    {
        UNARMED = 0,
        TWOHANDSWORD = 1,
        TWOHANDSPEAR = 2,
        TWOHANDAXE = 3,
        TWOHANDBOW = 4,
        TWOHANDCROSSBOW = 5,
        STAFF = 6,
        ONEHANDED = 7,
        RELAX = 8,
     //   RIFLE = 9,
        TWOHANDCLUB = 20,
        SHIELD = 11,
        ARMEDSHIELD = 12,
        RIFLE = 18
    }
    
    // This is from the animation state machine -- which is a
    // complete fucking mess.  Great animations, but the setup and state machines
    // are fucking awful.
    public enum EWeaponAnimationArmedType
    {
        UNARMED = 0,
        TWOHANDSWORD = 1,
        TWOHANDSPEAR = 2,
        TWOHANDAXE = 3,
        TWOHANDBOW = 4,
        TWOHANDCROSSBOW = 5,
        STAFF = 6,
        SHIELD = 7,
        LEFT_SWORD = 8,
        RIGHT_SWORD = 9,
        LEFT_MACE = 10,
        RIGHT_MACE = 11,
        LEFT_DAGGER = 12,
        RIGHT_DAGGER = 13,
        LEFT_ITEM = 14,
        RIGHT_ITEM = 15,
        LEFT_PISTOL = 16,
        RIGHT_PISTOL = 17,
        RIFLE = 18,
        RIGHT_SPEAR = 19,
        TWOHANDCLUB = 20
    }

    [CreateAssetMenu(menuName = "RPG/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] string weaponName;

        // Transform applied to the weapon when attached to a game object
        public Transform gripTransform;

        // Where the tip of a firearm/ranged weapon is.
        public Transform muzzleTipTransform;

        // The character will use this to play the correct weapon animations associated with this weapon.
        [SerializeField] EWeaponType weaponType;

        // This will get added to the weapon holders recovery time in seconds
        [SerializeField] float recoveryTimeSeconds = 1.0f;

        // List of damage types this weapon uses.
        [SerializeField] List<DamageTypeConfig> damageTypes;

        // How long an attack takes before the recovery period begins.
        // Right now, this is going to get hard coded to the weapon animation length,
        // since I can't figure out a way to get this reliably at runtime.
        [SerializeField] float attackSpeedSeconds = 1.0f;

        // Attack range in units (meters)
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] bool useOtherHand;
        [SerializeField] float baseDamage = 0.0f;

        [SerializeField] bool aimWeapon;
        [SerializeField] GameObject particlePrefab;
 
        public string GetWeaponName()
        {
            return weaponName;
        }

        public EWeaponType GetWeaponType()
        {
            return weaponType;
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

        public float GetBaseDamage()
        {
            return baseDamage;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }
    }
}
