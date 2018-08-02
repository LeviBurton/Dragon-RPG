using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Image healthBar;

   
    void OnDamage(float damageAmount)
    {
        Debug.LogFormat("{0} OnDamage {1}", name, damageAmount);
        healthBar.fillAmount = healthSystem.HealthAsPercentage;
    }

    void OnHeal(float healAmount)
    {

    }

	// Use this for initialization
	void Start () {

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

    // Update is called once per frame
    void Update () {
		
	}
}
