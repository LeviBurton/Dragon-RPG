using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Character/Config")]
    public class CharacterConfig : ScriptableObject
    {
        // Going to need a stats style data structure.
        // Seconds
        [SerializeField] float recoveryTimeSeconds = 1.0f;
        [SerializeField] float health = 10;

        // Units per Second?
        [SerializeField] float walkSpeed = 1.35f;
        [SerializeField] float runSpeed = 6.0f;
        [SerializeField] float sprintSpeed = 6.0f;
        [SerializeField] float rotationSpeed = 40f;

        public float GetHealth()
        {
            return health;
        }

        public float GetWalkSpeed()
        {
            return walkSpeed;
        }

        public float GetRunSpeed()
        {
            return runSpeed;
        }

        public float GetSprintSpeed()
        {
            return sprintSpeed;
        }

        public float GetRotationSpeed()
        {
            return rotationSpeed;
        }

      
        public float GetRecoveryTime()
        {
            return recoveryTimeSeconds;
        }

    }
}
