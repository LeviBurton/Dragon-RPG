using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] float attackRadius = 4f;
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float secondsBetweenShots = 0.5f;
        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        [SerializeField]
        Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);

        [SerializeField]
        Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);

        bool isAttacking = false;
        float currentHealthPoints;
        AICharacterControl aiCharacterControl = null;
        GameObject player = null;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0) { Destroy(gameObject); }
        }

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            aiCharacterControl = GetComponent<AICharacterControl>();
            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                InvokeRepeating("FireProjectile", 0f, secondsBetweenShots); // TODO switch to coroutines
            }

            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacterControl.SetTarget(player.transform);
            }
            else
            {
                aiCharacterControl.SetTarget(transform);
            }
        }

        // TODO start separating out character firing logic into a separate class.
        void FireProjectile()
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetDamage(damagePerShot);
            projectileComponent.SetShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = attackSphereColor;
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            // Draw chase sphere 
            Gizmos.color = chaseSphereColor;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}