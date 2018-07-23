using RPG.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Character
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(WeaponSystem))]
    public class FriendlyController : MonoBehaviour
    {
        [SerializeField] FriendlyConfig friendlyConfig;

        CharacterController character;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        void Start()
        {
            character = GetComponent<CharacterController>();
            weaponSystem = character.GetComponent<WeaponSystem>();
            healthSystem = character.GetComponent<HealthSystem>();

            if (friendlyConfig == null)
            {
                throw new Exception("friendlyConfig can't be null!");
            }


        }
    }
}