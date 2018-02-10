using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2.0f;
        [SerializeField] Color chaseSphereColor = new Color(0, 1.0f, 0, .5f);
        [SerializeField] Color attackSphereColor = new Color(1.0f, 1.0f, 0, .5f);

        PlayerControl player;
        Character character;
        WeaponSystem weaponSystem;

        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;

        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;

        void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }

        void Update()
        {
            weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (player == null || player.GetComponent<HealthSystem>().healthAsPercentage <= 0)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }

            else if (distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }

            else if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }

            else if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines();
                state = State.attacking;
                weaponSystem.AttackTarget(player.gameObject);
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;

            while (patrolPath != null)
            {
                Vector3 nextWayPointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWayPointPos);

                CycleWaypointWhenClose(nextWayPointPos);

                yield return new WaitForSeconds(0.5f);  
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypoint)
        {
            if (Vector3.Distance(transform.position, nextWaypoint) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;

            while (distanceToPlayer >= chaseRadius)
            {
                character.SetDestination(player.transform.position);

                yield return new WaitForEndOfFrame();
            }

            yield return null;
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            return distanceToTarget <= currentWeaponRange;
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = attackSphereColor;
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            // Draw chase sphere 
            Gizmos.color = chaseSphereColor;
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }
    }
}