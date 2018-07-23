using RPG.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Character")]
    public class CharacterConfig : ScriptableObject
    {
        // Character size
        [SerializeField] ECharacterSize size = ECharacterSize.Medium;
        [SerializeField] Vector3 characterCenter;
        [SerializeField] Vector3 characterSize;

        // Going to need a stats style data structure.
        // Seconds
        [SerializeField] float recoveryTimeSeconds = 1.0f;
        [SerializeField] float health = 10;

        // Units per Second?
        [SerializeField] float walkSpeed = 1.35f;
        [SerializeField] float runSpeed = 6.0f;
        [SerializeField] float sprintSpeed = 6.0f;
        [SerializeField] float rotationSpeed = 40f;

        public Vector3 GetCharacterCenter()
        {
            return characterCenter;
        }

        public Vector3 GetCharacterSize()
        {
            return characterSize;
        }

        public ECharacterSize GetSize()
        {
            return size;
        }

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
