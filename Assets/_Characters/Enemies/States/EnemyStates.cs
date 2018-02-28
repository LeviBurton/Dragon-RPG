using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.StateMachine;
using RPG.Core;

namespace RPG.Characters
{
    public class Enemy_StartState : SingletonBase<Enemy_StartState>, IState<EnemyAI>
    {
        public void OnEnter(EnemyAI Entity)
        {
            Entity.GetStateMachine().ChangeState(Enemy_ChaseState.Instance);
        }

        public void OnExecute(EnemyAI Entity)
        {
        }

        public void OnExit(EnemyAI Entity)
        {
        }
    }

    public class Enemy_IdleState : SingletonBase<Enemy_IdleState>, IState<EnemyAI>
    {
        public void OnEnter(EnemyAI Entity)
        {
        }

        public void OnExecute(EnemyAI Entity)
        {
        }

        public void OnExit(EnemyAI Entity)
        {
        }
    }

    public class Enemy_PatrolState : SingletonBase<Enemy_PatrolState>, IState<EnemyAI>
    {
        public void OnEnter(EnemyAI Entity)
        {
        }

        public void OnExecute(EnemyAI Entity)
        {
        }

        public void OnExit(EnemyAI Entity)
        {
        }
    }

    public class Enemy_ChaseState : SingletonBase<Enemy_ChaseState>, IState<EnemyAI>
    {
        public void OnEnter(EnemyAI Entity)
        {
            Debug.LogFormat("Chasing {0}", Entity.currentTarget.name);
        }

        public void OnExecute(EnemyAI Entity)
        {
            if (Entity.GetWeaponSystem().IsTargetInRange(Entity.currentTarget))
            {
                Entity.GetStateMachine().ChangeState(Enemy_AttackState.Instance);
            }

            Entity.GetComponent<Character>().SetDestination(Entity.currentTarget.transform.position);
            Entity.transform.LookAt(Entity.currentTarget.transform);
        }

        public void OnExit(EnemyAI Entity)
        {
            Debug.LogFormat("Stopped Chasing {0}", Entity.currentTarget.name);
        }
    }

    public class Enemy_AttackState : SingletonBase<Enemy_AttackState>, IState<EnemyAI>
    {
        public void OnEnter(EnemyAI Entity)
        {
            Debug.LogFormat("Attacking {0}", Entity.currentTarget.name);

            if (Entity.currentTarget != null)
            {
                var targetHealthSystem = Entity.currentTarget.GetComponent<HealthSystem>();
                if (targetHealthSystem != null)
                {
                    if (targetHealthSystem.IsAlive())
                    {
                        Debug.LogFormat("AutoAttacking: {0} with {1}", Entity.currentTarget.name, Entity.GetWeaponSystem().name);
                        Entity.GetWeaponSystem().AutoAttack();
                    }
                    else
                    {
                        Entity.GetStateMachine().ChangeState(Enemy_PatrolState.Instance);
                    }
                }
            }
            else
            {
                Debug.Log("No target found, changing to Patrol");
                Entity.GetStateMachine().ChangeState(Enemy_PatrolState.Instance);
            }
        }

        public void OnExecute(EnemyAI Entity)
        {
            if (Entity.currentTarget != null)
            {
                Entity.transform.LookAt(Entity.currentTarget.transform);

                var targetHealthSystem = Entity.currentTarget.GetComponent<HealthSystem>();
                if (targetHealthSystem != null)
                {
                    if (!targetHealthSystem.IsAlive())
                    {
                        Entity.GetStateMachine().ChangeState(Enemy_IdleState.Instance);
                    }
                }

                if (!Entity.GetWeaponSystem().IsTargetInRange(Entity.currentTarget))
                {
                    Entity.GetStateMachine().ChangeState(Enemy_ChaseState.Instance);
                }
            }
            else
            {
                Entity.GetStateMachine().ChangeState(Enemy_PatrolState.Instance);
            }
        }

        public void OnExit(EnemyAI Entity)
        {
            Entity.GetWeaponSystem().StopAttacking();
            Debug.LogFormat("Stopped Attacking {0}", Entity.currentTarget.name);
        }
    }
}