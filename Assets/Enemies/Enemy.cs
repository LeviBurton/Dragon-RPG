using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField]
    float MaxHealthPoints = 100;
    float CurrentHealthPoints = 100;

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

    AICharacterControl AICharacterControl = null;
    GameObject Player = null;

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
    }

    private void Start()
    {
        AICharacterControl = GetComponent<AICharacterControl>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        float DistanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        if (DistanceToPlayer <= AttackRadius)
        {

            SpawnProjectile();
        }
        else
        {
            AICharacterControl.SetTarget(transform);
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
        Vector3 DirectionToPlayer = (Player.transform.position - ProjectileSocket.transform.position).normalized;
        float Speed = ProjectileComponent.ProjectileSpeed;

        ProjectileComponent.DamageCaused = DamagePerShot;
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
