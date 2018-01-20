using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float ProjectileSpeed;

    float DamageCaused;
    float TimeToLive = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        TimeToLive -= Time.deltaTime;
        if (TimeToLive <= 0)
            Destroy(gameObject);
	}

    private void OnCollisionEnter(Collision collision)
    {
        Component DamageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
        if (DamageableComponent)
        {
            (DamageableComponent as IDamageable).TakeDamage(DamageCaused);
        }

        Destroy(gameObject);
    }


    public void SetDamage(float Damage)
    {
        DamageCaused = Damage;
    }
}
