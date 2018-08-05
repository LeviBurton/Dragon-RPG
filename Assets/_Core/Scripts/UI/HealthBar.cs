using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthValues;

    void OnDamage(float damageAmount)
    {
        healthBar.fillAmount = healthSystem.HealthAsPercentage;
        if (healthValues)
        {
            healthValues.SetText(string.Format("{0}/{1}", healthSystem.GetCurrentHealth(), healthSystem.GetMaxHealth()));
        }
    }

    void OnHeal(float healAmount)
    {
    }

    private void OnEnable()
    {
        healthSystem.onDamage += OnDamage;
        healthSystem.onHeal += OnHeal;
    }

    private void OnDisable()
    {
        healthSystem.onDamage -= OnDamage;
        healthSystem.onHeal -= OnHeal;
    }
}
