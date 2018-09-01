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
    public class HeroController : MonoBehaviour
    {
        [SerializeField] HeroConfig friendlyConfig;

        CharacterController character;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        void Start()
        {
            character = GetComponent<CharacterController>();
            weaponSystem = character.GetComponent<WeaponSystem>();
            healthSystem = character.GetComponent<HealthSystem>();
        }

        void AddOutlinesToMeshes()
        {
            // Outline stuff.
            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var mesh in skinnedMeshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 0;
                }
            }
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 0;
                }
            }
        }
    }
}