using RPG.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.ActionSystem
{
    public interface IAction
    {
        EActionType ActionType { get; set; }
        float AddedOn { get; set; }    // Time in seconds since the game started this was queued on
        float ExecuteOn { get; set; }   // Time to execute this action.
        GameObject Owner { get; set; }
        bool IsQueued { get; set; }
        void Execute();
        bool IsExeuting();
        bool IsComplete();
        bool CanExecuteMultiple();
        bool IsInterruptible();
    }

    public class MoveToAction : IAction
    {
        private bool atDestination = false;
        private bool isExecuting = false;   // Probably don't need this.
        private Vector3 target;

        public float AddedOn { get; set; }
        public float ExecuteOn { get; set; }
        public GameObject Owner { get; set; }

        public bool IsQueued
        {
            get;
            set;
        }

        public EActionType ActionType
        {
            get
            {
                return EActionType.Move;
            }

            set
            {
                ActionType = value;
            }
        }

        public MoveToAction(GameObject owner, Vector3 target)
        {
            this.target = target;
            this.Owner = owner;
        }

        public void Execute()
        {
            Character character = Owner.GetComponent<Character>();

            if (!isExecuting)
            {
                character.SetStopped(false);
                character.SetDestination(this.target);
                Debug.LogFormat("MoveTo: {0}", target);
                isExecuting = true;
            }

            if (Vector3.Distance(character.transform.position, target) <= character.StoppingDistance())
            {
                atDestination = true;
                character.SetStopped(true);
                Debug.LogFormat("At Destination");
            }

        }

        public bool IsComplete()
        {
            return atDestination;
        }

        public bool IsExeuting()
        {
            return isExecuting;
        }

        public bool IsInterruptible()
        {
            throw new NotImplementedException();
        }

        public bool CanExecuteMultiple()
        {
            return true;
        }
    }

}