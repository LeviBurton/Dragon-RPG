using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyBar = null; 
        [SerializeField] float maxEnergyPoints = 100.0f;
        [SerializeField] float regenPointsPerSecond = 10.0f;

        public float currentEnergyPoints;

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            var newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();
        }

        void Start()
        {
            currentEnergyPoints = maxEnergyPoints;
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

        void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        float EnergyAsPercent()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }

        void UpdateEnergyBar()
        {
            energyBar.fillAmount = EnergyAsPercent();
        }
    }
}