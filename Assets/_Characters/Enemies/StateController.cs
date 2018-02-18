using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core.FSM;
using UnityEngine.AI;

namespace RPG.Characters
{
    public class StateController : MonoBehaviour
    {
        public State currentState;
        public State remainState;
        public Transform eyes;

        [HideInInspector] public NavMeshAgent navMeshAgent;
        [HideInInspector] public List<Transform> wayPointList;
        [HideInInspector] public int nextWayPoint;
        public Transform chaseTarget;
        [HideInInspector] public float stateTimeElapsed;

        private bool aiActive = true;

        void Awake()
        {
            
        }

         void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (!aiActive)
                return;

            currentState.UpdateState(this);
        }

        public void TransitionToState(State nextState)
        {
            if (nextState != remainState)
            {
                currentState = nextState;
            }
        }

        void OnDrawGizmos()
        {
            if (currentState != null && eyes != null)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere(eyes.position, 1.0f);
            }
        }
    }
}
