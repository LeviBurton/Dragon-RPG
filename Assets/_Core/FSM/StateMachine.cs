using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.StateMachine
{
    public interface IState<T>
    {
        void OnEnter(T Entity);
        void OnExecute(T Entity);
        void OnExit(T Entity);
    }

    public class StateMachine<T>
    {
        private T Owner;
        public IState<T> CurrentState;
        public IState<T> PreviousState;
        public IState<T> GlobalState;

        public StateMachine(T Owner)
        {
            this.Owner = Owner;
            CurrentState = null;
            PreviousState = null;
            GlobalState = null;
        }

        public void Update()
        {
            if (GlobalState != null)
            {
                GlobalState.OnExecute(Owner);
            }

            if (CurrentState != null)
            {
                CurrentState.OnExecute(Owner);
            }
        }

        public void ChangeState(IState<T> NewState)
        {
            if (NewState == null)
                return;

            PreviousState = CurrentState;

            if (CurrentState != null)
            {
                CurrentState.OnExit(Owner);
            }

            CurrentState = NewState;
            CurrentState.OnEnter(Owner);
        }

        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }
    }
}