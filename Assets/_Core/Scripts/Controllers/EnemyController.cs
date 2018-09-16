using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

using Panda;
using UnityEngine.AI;
using UnityEngine.UI;
using RPG.Characters;

namespace RPG.Characters
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterSystem))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] EnemyConfig enemyConfig;
        [SerializeField] Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);
        [SerializeField] Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);
        [SerializeField] Image recoveryCircleImage;
        [SerializeField] Sprite moveActionImage;
        [SerializeField] Sprite attackActionImage;
        [SerializeField] Image actionImage;

        private float minRecoveryTimeSeconds;
        private float maxRecoveryTimeSeconds;
        private float currentRecoveryTimeSeconds;

        CharacterSystem character;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        void Start()
        {
            character = GetComponent<CharacterSystem>();
            AddOutlinesToMeshes();
            character.SetOutlinesEnabled(false);

            if (enemyConfig == null)
                throw new Exception("enemyConfig can't be null!");

            minRecoveryTimeSeconds = character.GetCharacterConfig().GetRecoveryTime();
            currentRecoveryTimeSeconds = 0.0f;
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
                    outline.color = 1;
                }
            }
            var meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var mesh in meshRenderers)
            {
                if (!mesh.gameObject.GetComponent<cakeslice.Outline>())
                {
                    var outline = mesh.gameObject.AddComponent<cakeslice.Outline>();
                    outline.color = 1;
                }
            }
        }

        void OnDrawGizmos()
        {
         
        }

   
    }
}