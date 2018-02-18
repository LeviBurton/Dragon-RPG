using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar = null; 
        [SerializeField] float maxEnergyPoints = 100.0f;
        [SerializeField] float regenPointsPerSecond = 10.0f;
        [SerializeField] AudioClip outOfEnergySound;

        float currentEnergyPoints;
        AudioSource audioSource;

        float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;

            AttachInitialAbilities();
            UpdateEnergyBar();
        }

        void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyBar();
            }
        }

        public void ConsumeEnergy(float amount)
        {
            var newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        }

        public int GetNumberOfAbilitie()
        {
            return abilities.Length;
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyComponent = GetComponent<SpecialAbilities>();
            float energyCost = abilities[abilityIndex].GetEnergyCost();
            string abilityName = abilities[abilityIndex].name;

            if (energyCost <= currentEnergyPoints)
            {
                energyComponent.ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergySound);
            }
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        void UpdateEnergyBar()
        {
            if (energyBar == null)
                return;

            energyBar.fillAmount = energyAsPercent;
        }
    }
}