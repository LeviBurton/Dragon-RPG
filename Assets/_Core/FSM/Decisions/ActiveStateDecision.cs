using System.Collections;
using System.Collections.Generic;
using RPG.Characters;
using UnityEngine;

namespace RPG.Core.FSM
{
    [CreateAssetMenu(menuName = "FSM/Decisions/ActiveState")]
    public class ActiveStateDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool chaseTargetIsActive = controller.chaseTarget.gameObject.activeSelf;
            return chaseTargetIsActive;
        }
    }
}