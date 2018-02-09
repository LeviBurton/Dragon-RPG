﻿using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab ;
        [SerializeField] AudioClip[] audioClips;
        [SerializeField] AnimationClip abilityAnimation;

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetRandomAbilitySound()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }

        protected AbilityBehavior behavior;

        public abstract AbilityBehavior GetBehaviorComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo(GameObject objectToAttachTo)
        {
            AbilityBehavior behaviorComponent = GetBehaviorComponent(objectToAttachTo);
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public void Use(GameObject target)
        {
            behavior.Use(target);
        }
    }
}
