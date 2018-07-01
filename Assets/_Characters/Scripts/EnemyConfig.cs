using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Enemy/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        // Going to need a stats style data structure.
        // Seconds
        [SerializeField] float recoveryTimeSeconds = 1.0f;

        // Meters/Second
        [SerializeField] float movementSpeed = 1.0f;

        public float GetMovementSpeed()
        {
            return movementSpeed;
        }

        public float GetRecoveryTime()
        {
            return recoveryTimeSeconds;
        }

    }
}
