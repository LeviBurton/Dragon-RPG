using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.FSM
{
    public abstract class Decision : ScriptableObject
    {

        public abstract bool Decide(StateController controller);
    }
}
