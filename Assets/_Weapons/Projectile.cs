using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        float projectileSpeed;

        [SerializeField]
        GameObject shooter;

        const float DESTROY_DELAY = 0.01f;
        float damageCaused;

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

        private void DamageIfDamageable(Collision collision)
        {
            Component damagableComponent = collision.gameObject.GetComponent(typeof(IDamageable));

            if (damagableComponent)
            {
                (damagableComponent as IDamageable).TakeDamage(damageCaused);
            }

            Destroy(gameObject, DESTROY_DELAY);
        }
    }
}