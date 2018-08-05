using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        public delegate void OnDamage(float damageAmount);
        public event OnDamage onDamage;

        public delegate void OnHeal(float healAmount);
        public event OnHeal onHeal;

        [SerializeField] float maxHealthPoints = 100f;      // TODO: get from owner config (such as Enemy Config, Player Config)
        [SerializeField] float deathVanishSeconds = 2.0f;
        [SerializeField] Image healthBar;
        [SerializeField] Slider healthBarSlider;

        float currentHealthPoints;

        void Start()
        {
            currentHealthPoints = maxHealthPoints;
        }

        void LateUpdate()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = HealthAsPercentage;
            }
        }

        public float GetMaxHealth()
        {
            return maxHealthPoints;
        }

        public void SetMaxHealth(float maxHealth)
        {
            maxHealthPoints = maxHealth;
        }

        public void ResetHealthPoints()
        {
            currentHealthPoints = maxHealthPoints;
        }

        public bool IsAlive()
        {
            return currentHealthPoints > 0;
        }

        public float GetCurrentHealth()
        {
            return currentHealthPoints;
        }

        public float HealthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void Heal(float healAmount)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healAmount, 0f, maxHealthPoints);
            onHeal(healAmount);
        }

        public void Damage(float damageAmount)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damageAmount, 0f, maxHealthPoints);
            onDamage(damageAmount);
        }
    }
}