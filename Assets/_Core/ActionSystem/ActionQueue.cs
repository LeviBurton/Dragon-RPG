using RPG.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.ActionSystem
{
    // EActionTypes will define the different types of actions we can have in the queue.
    public enum EActionType
    {
        Move,
        Attack,
        Ability
    }

    public class ActionQueue : MonoBehaviour
    {
        public List<IAction> actionList;
        private GameObject owner;

        public void Start()
        {
            actionList = new List<IAction>();
        }

        public void Update()
        {
        }

        public void AddAction(IAction action, bool queueAction)
        { 
            action.AddedOn = Time.time;
            action.IsQueued = queueAction;
            actionList.Add(action);
        }
    }
}