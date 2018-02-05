﻿using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
    public struct AbilityUseParams
    {
        public IDamageable target;
        public float baseDamage;

        public AbilityUseParams(IDamageable target, float baseDamage)
        {
            this.target = target;
            this.baseDamage = baseDamage;
        }
    }

    public interface ISpecialAbility
    {
        void Use(AbilityUseParams useParams);
    }

    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip audioClip = null;

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetAudioClip()
        {
            return audioClip;
        }

        protected ISpecialAbility behavior;

        abstract public void AttachComponentTo(GameObject gameObjectToAttachTo);

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public void Use(AbilityUseParams useParams)
        {
            behavior.Use(useParams);
        }
    }
}
