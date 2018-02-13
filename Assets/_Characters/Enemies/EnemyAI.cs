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
        [SerializeField] bool preferRangedAttack = false;
        [SerializeField] float threatRange = 10.0f;
        [SerializeField] float waypointWaitTime = 2.0f;
        
        PlayerControl player;
        Character character;
        WeaponSystem weaponSystem;
        Selectable selectable;

        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;

        // todo this style of state machine gets unweildy quick.
        // consider changing to FSM.
        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;

        void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
            selectable = GetComponent<Selectable>();
            weaponSystem = GetComponent<WeaponSystem>();

            weaponSystem.onWeaponHit += OnWeaponHit;
        }

        void Update()
        {
            // todo think about what to do about the code below and whether we should do that every frame.
            // weaponSystem = GetComponent<WeaponSystem>();

            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
            bool inPlayerThreatCircle = distanceToPlayer <= threatRange;
            bool inChaseRing = distanceToPlayer >= currentWeaponRange && 
                                 distanceToPlayer <= chaseRadius;
            bool outsideChaseRing = distanceToPlayer > chaseRadius;

            //if (inPlayerThreatCircle && preferRangedAttack)
            //{
            //    StopAllCoroutines();
            //    weaponSystem.StopAttacking();

            //    var Direction = transform.TransformDirection(transform.forward);
            //    var targetPosition = Vector3.Reflect(Direction, Vector3.up) * 100.0f;
            //    character.SetDestination(targetPosition);
            //}

            if (outsideChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
            
                StartCoroutine(Patrol());
            }

            if (inChaseRing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();

     
                StartCoroutine(ChasePlayer());
            }

            if (inWeaponCircle)
            {
                StopAllCoroutines();

                if (preferRangedAttack)
                {
                    character.SetDestination(transform.position);
                }

                state = State.attacking;
                weaponSystem.AttackTarget(player.gameObject);
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;
            player.GetComponent<Selectable>().Deselect();
            while (patrolPath != null)
            {
                Vector3 nextWayPointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWayPointPos);

                CycleWaypointWhenClose(nextWayPointPos);

                yield return new WaitForSecondsRealtime(waypointWaitTime);  
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

            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);

                yield return new WaitForEndOfFrame();
            }
 
            character.SetDestination(transform.position);

            yield return null;
        }

        bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            return distanceToTarget <= currentWeaponRange;
        }

        void OnWeaponHit(WeaponSystem weaponSystem, GameObject hitObject, float damage)
        {
            Debug.LogFormat("{0} hit {1} with {2} for {3} damage", weaponSystem.name, hitObject.name, weaponSystem.GetCurrentWeapon().name, damage);
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