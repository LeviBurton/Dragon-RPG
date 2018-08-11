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

        // Units per second.  Can be changed in the owner component instance.
        [SerializeField] float maxForwardMoveSpeed = 10.0f;
        [SerializeField] float maxSideMoveSpeed = 5.0f;
        [SerializeField] float maxRotationSpeed;

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

        public float GetMaxForwardSpeed()
        {
            return maxForwardMoveSpeed;
        }

        public float GetMaxSideSpeed()
        {
            return maxSideMoveSpeed;
        }

        public float GetMaxRotationSpeed()
        {
            return maxRotationSpeed;
        }
      
        public float GetRecoveryTime()
        {
            return recoveryTimeSeconds;
        }
    }
}
