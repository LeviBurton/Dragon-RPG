using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float ProjectileSpeed;

    float DamageCaused;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider Other)
    {
        Component DamageableComponent = Other.gameObject.GetComponent(typeof(IDamageable));
        if (DamageableComponent)
        {
            (DamageableComponent as IDamageable).TakeDamage(DamageCaused);
        }
    }

    public void SetDamage(float Damage)
    {
        DamageCaused = Damage;
    }
}
