using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    float MaxHealthPoints = 100;

    [SerializeField]
    GameObject ProjectileToUse;

    [SerializeField]
    GameObject ProjectileSocket;

    [SerializeField]
    float AttackRadius = 5;

    [SerializeField]
    float ChaseRadius = 10;

    [SerializeField]
    float DamagePerShot = 10;

    [SerializeField]
    float SecondsBetweenShots = 0.5f;

    [SerializeField]
    Vector3 AimOffset = new Vector3(0, 1, 0);

    AICharacterControl AICharacterControl = null;
    GameObject Player = null;
    float CurrentHealthPoints = 0;
    bool bIsAttacking = false;

    public float HealthAsPercentage
    {
        get
        {
            return CurrentHealthPoints / (float)MaxHealthPoints;
        }
    }

    public void TakeDamage(float Damage)
    {
        CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - Damage, 0, MaxHealthPoints);
        if (CurrentHealthPoints <= 0) { Destroy(gameObject); }
    }

    private void Start()
    {
        AICharacterControl = GetComponent<AICharacterControl>();
        Player = GameObject.FindGameObjectWithTag("Player");
        CurrentHealthPoints = MaxHealthPoints;
    }

    private void Update()
    {
        float DistanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        if (DistanceToPlayer <= AttackRadius && !bIsAttacking)
        {
            bIsAttacking = true;

            // TODO switch to coroutines
            InvokeRepeating("SpawnProjectile", 0f, SecondsBetweenShots);
        }

        if (DistanceToPlayer > AttackRadius)
        {
            bIsAttacking = false;
            CancelInvoke();
        }

        if (DistanceToPlayer <= ChaseRadius)
        {
            AICharacterControl.SetTarget(Player.transform);
        }
        else
        {
            AICharacterControl.SetTarget(transform);
        }
    }

    private void SpawnProjectile()
    {
        GameObject NewProjectile = Instantiate(ProjectileToUse, ProjectileSocket.transform.position, Quaternion.identity);
        Projectile ProjectileComponent = NewProjectile.GetComponent<Projectile>();
        Vector3 DirectionToPlayer = (Player.transform.position + AimOffset - ProjectileSocket.transform.position).normalized;
        float Speed = ProjectileComponent.ProjectileSpeed;

        ProjectileComponent.SetDamage(DamagePerShot);

        NewProjectile.GetComponent<Rigidbody>().velocity = DirectionToPlayer * Speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ChaseRadius);

    }
}
