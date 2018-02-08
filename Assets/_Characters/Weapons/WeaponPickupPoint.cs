using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Characters;

namespace RPG.Characters
{
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] WeaponConfig weaponConfig = null;
        [SerializeField] AudioClip pickupSFX = null;

        AudioSource audioSource;

        void OnTriggerEnter(Collider other)
        {
            FindObjectOfType<WeaponSystem>().PutWeaponInHand(weaponConfig);
            audioSource.PlayOneShot(pickupSFX);
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();    
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
            }
        }

        void DestroyChildren()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void InstantiateWeapon()
        {
            var weapon = weaponConfig.GetWeaponPrefab();
         
            Instantiate(weapon, gameObject.transform);
            weapon.transform.position = Vector3.zero;
        }
    }
}