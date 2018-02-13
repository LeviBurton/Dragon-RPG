using System.Collections;
using System.Collections.Generic;
using RPG.Characters;
using UnityEngine;

namespace RPG.Core.FSM
{
    [CreateAssetMenu(menuName = "FSM/Decisions/Look")]
    public class LookDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            bool targetVisible = Look(controller);

            return targetVisible;
        }

        private bool Look(StateController controller)
        {
            RaycastHit hit;

            Debug.DrawRay(controller.eyes.position, controller.eyes.forward.normalized * 20.0f, Color.green);

            if (Physics.SphereCast(controller.eyes.position, 5.0f, controller.eyes.forward, out hit, 10.0f)
                && hit.collider.CompareTag("Player"))
            {
                controller.chaseTarget = hit.transform;
                return true;
            }

            return false;
        }
    }
}
