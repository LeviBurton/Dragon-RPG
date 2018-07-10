using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class WaypointContainer : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Vector3 fistPosition = transform.GetChild(0).position;
            Vector3 previousPosition = fistPosition;

            foreach (Transform waypoint in transform)
            {
                Gizmos.DrawSphere(waypoint.position, 0.2f);
                Gizmos.DrawLine(previousPosition, waypoint.position);
                previousPosition = waypoint.position;
            }

            Gizmos.DrawLine(previousPosition, fistPosition);
        }
    }
}
