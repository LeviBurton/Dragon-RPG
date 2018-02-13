using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.FSM
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Act(StateController controller);
    }
}
