using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        float projectileSpeed;

        [SerializeField]
        GameObject shooter;

        const float DESTROY_DELAY = 0.01f;
        float damageCaused;

        float timeToLive = 5.0f;

        private void Update()
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive <= 0)
            {
                Destroy(gameObject, DESTROY_DELAY);
            }
        }

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }

        void OnCollisionEnter(Collision collision)
        {
            var layerCollidedWith = collision.gameObject.layer;

            if (shooter && layerCollidedWith != shooter.layer)
            {
                DamageIfDamageable(collision);
            }
        }

        void DamageIfDamageable(Collision collision)
        {
            // Todo reimplement this.
            //Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamageable));

            //if (damagableComponent)
            //{
            //    (damagableComponent as IDamageable).TakeDamage(damageCaused);
            //}

            Destroy(gameObject, DESTROY_DELAY);
        }
    }
}