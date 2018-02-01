using UnityEngine;
using UnityEngine.Assertions;

// TODO consider rewiring.
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;  
        [SerializeField] float baseDamage = 10f;
        [SerializeField] Weapon weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] SpecialAbility[] abilities;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        Animator animator;
        float currentHealthPoints;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;
        AudioSource audioSource;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            ReduceHealth(damage);
            audioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.Play();

            bool playerDies = currentHealthPoints - damage <= 0;

            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }
        }

        IEnumerator KillPlayer()
        {
            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();

            // trigger death animation
            Debug.Log("death animation");

            // wait for animation to finish
            yield return new WaitForSecondsRealtime(audioSource.clip.length); // TODO use audio clip length later.

            // reload scene
            SceneManager.LoadScene(0);
        }
         
        void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            SetCurrentMaxHealth();
            RegisterForMouseClick();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            abilities[0].AttachComponentTo(gameObject);
        }

        void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip();
        }

        void PutWeaponInHand()
        {
            var weaponPrefab = weaponInUse.GetWeaponPrefab();

            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);
            weapon.transform.localPosition = weaponInUse.gripTransform.transform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.transform.localRotation;
        }

        GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No dominant hand found on player, please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple dominant hand scripts on player.  Only 1 allowed.");
            return dominantHands[0].gameObject;
        }

        void RegisterForMouseClick()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0, enemy);
            }
        }

        void AttemptSpecialAbility(int abilityIndex, Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            float abilityEnergyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyComponent.IsEnergyAvailable(abilityEnergyCost))
            {
                energyComponent.ConsumeEnergy(abilityEnergyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        void AttackTarget(Enemy enemy)
        {       
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                animator.SetTrigger("Attack");   // TODO make const
                enemy.TakeDamage(baseDamage);
                lastHitTime = Time.time;
            }
        }
    }
}