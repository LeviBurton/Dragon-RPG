﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;      // TODO: get from owner config (such as Enemy Config, Player Config)
        [SerializeField] float deathVanishSeconds = 2.0f;
        [SerializeField] Image healthBar;
        [SerializeField] Slider healthBarSlider;

        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character character;

        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            character = GetComponent<Character>();
            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            UpdateHealthBar();
        }

        public bool IsAlive()
        {
            return healthAsPercentage > 0;
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
         
           // var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
          //  audioSource.PlayOneShot(clip);
            bool characterDies = currentHealthPoints <= 0;

            if (characterDies)
            {
               StartCoroutine(KillCharacter());
            }
        }

        IEnumerator KillCharacter()
        {
            character.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<PlayerControl>();
            if (playerComponent && playerComponent.isActiveAndEnabled)
            {
                audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
                audioSource.Play();
                yield return new WaitForSecondsRealtime(audioSource.clip.length);

            }
            else
            {
                // todo: could be an enemy
                Destroy(gameObject, deathVanishSeconds);
            }
        }

        void UpdateHealthBar()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = healthAsPercentage;

                //healthBar.fillAmount = healthAsPercentage;
            }
        }

        void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }
    }
}