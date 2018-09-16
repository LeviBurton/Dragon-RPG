﻿using RPG.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
    
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(CharacterSystem))]
    [RequireComponent(typeof(WeaponSystem))]
    public class HeroController : MonoBehaviour
    {
        [SerializeField] HeroConfig friendlyConfig;
        [SerializeField] Light selectedLight = null;

        CharacterSystem character;
        WeaponSystem weaponSystem;
        HealthSystem healthSystem;

        bool playerControlEnabled = false;

        void OnEnable()
        {
            RegisterEventHandlers();
        }

        void OnDisable()
        {
            UnRegisterEventHandlers();
        }

        void Start()
        {
            character = GetComponent<CharacterSystem>();
            weaponSystem = character.GetComponent<WeaponSystem>();
            healthSystem = character.GetComponent<HealthSystem>();
        }

        void RegisterEventHandlers()
        {
            var selectable = GetComponent<Selectable>();
            if (selectable)
            {
                selectable.onSelected += OnSelected;
                selectable.onDeselected += OnDeselected;
            }
        }

        void UnRegisterEventHandlers()
        {
            var selectable = GetComponent<Selectable>();
            if (selectable)
            {
                selectable.onSelected -= OnSelected;
                selectable.onDeselected -= OnDeselected;
            }
        }

        public bool IsPlayerControlEnabled()
        {
            return playerControlEnabled;
        }

        public void SetPlayerControlEnabled(bool enabled)
        {
            playerControlEnabled = enabled;
            GetComponent<NavMeshAgent>().enabled = !playerControlEnabled;
        }

        public void OnSelected()
        {
            Debug.Log(name + ": HeroController OnSelected");
            SetPlayerControlEnabled(true);

            if (selectedLight)
            {
                selectedLight.enabled = true;
            }
        }

        public void OnDeselected()
        {
            Debug.Log(name + ": HeroController OnDeselected");
            SetPlayerControlEnabled(false);

            if (selectedLight)
            {
                selectedLight.enabled = false;
            }
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