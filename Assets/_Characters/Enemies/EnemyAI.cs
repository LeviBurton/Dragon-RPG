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
        public GameObject currentTarget;

        WeaponSystem weaponSystem;
        Selectable selectable;

        float currentWeaponRange;
        float distanceToTarget;
        int nextWaypointIndex;

        bool isAttacking = false;
        bool isAiming = false;

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
            weaponSystem.target = currentTarget;
        }

        void Update()
        {
            // todo think about what to do about the code below and whether we should do that every frame.
            // weaponSystem = GetComponent<WeaponSystem>();
            
            // todo this is just test code here.  we don't do any input normally here.
            if (Input.GetKeyDown(KeyCode.A))
            {
                weaponSystem.SetAiming(true);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                weaponSystem.Reload();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                weaponSystem.SetAiming(weaponSystem.GetCurrentWeapon().GetAimWeapon());
                weaponSystem.AutoAttack();
            }

            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            if (!character.IsAlive())
            {
                weaponSystem.StopAttacking();
            }

            transform.LookAt(currentTarget.transform);

            return;

            if (currentTarget != null)
            {
                distanceToTarget = Vector3.Distance(currentTarget.transform.position, transform.position);

                bool inWeaponCircle = distanceToTarget <= currentWeaponRange;
                bool inPlayerThreatCircle = distanceToTarget <= threatRange;
                bool inChaseRing = distanceToTarget >= currentWeaponRange &&
                                     distanceToTarget <= chaseRadius;
                bool outsideChaseRing = distanceToTarget > chaseRadius;

                if (outsideChaseRing)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChasePlayer());
                }

                else if (inChaseRing)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChasePlayer());
                }

                else if (inWeaponCircle)
                {
                    StopAllCoroutines();

                    if (preferRangedAttack)
                    {
                        character.SetDestination(transform.position);
                    }

                    state = State.attacking;
                    //weaponSystem.AttackTarget(currentTarget.gameObject);
          
                }
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

            while (distanceToTarget >= currentWeaponRange)
            {
                character.SetDestination(currentTarget.transform.position);

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