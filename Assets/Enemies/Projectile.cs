using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float DamageCaused;
    public float ProjectileSpeed;

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
}
