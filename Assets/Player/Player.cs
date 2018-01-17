using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField]
    int EnemyLayer = 9;

    [SerializeField]
    float DamagePerHit = 10;

    [SerializeField]
    float MinTimeBetweenHits = 0.5f;

    [SerializeField]
    float MaxAttackRange = 2f;

    [SerializeField]
    float MaxHealthPoints = 100;

    GameObject CurrentTarget;
    CameraRaycaster CameraRayCaster;
    float CurrentHealthPoints = 100;
    float LastHitTime = 0f;

    private void Start()
    {
        CameraRayCaster = FindObjectOfType<CameraRaycaster>();
        CameraRayCaster.notifyMouseClickObservers += OnMouseClick;
        CurrentHealthPoints = MaxHealthPoints;
    }

    void OnMouseClick(RaycastHit RaycastHit, int LayerHit)
    {
        if (LayerHit == EnemyLayer)
        {
            var Enemy = RaycastHit.collider.gameObject;
            var EnemyComponent = Enemy.GetComponent<Enemy>();
      
            if (Vector3.Distance(Enemy.transform.position, transform.position) > MaxAttackRange)
            {
                return;
            }

            CurrentTarget = Enemy;

            if (Time.time - LastHitTime > MinTimeBetweenHits)
            {
                EnemyComponent.TakeDamage(DamagePerHit);
                LastHitTime = Time.time;
            }
        }
    }

    public void TakeDamage(float Damage)
    {
        CurrentHealthPoints = Mathf.Clamp(CurrentHealthPoints - Damage, 0, MaxHealthPoints);
    }

    public float healthAsPercentage
    {
        get
        {
            return CurrentHealthPoints / (float)MaxHealthPoints;
        }
    }
}
