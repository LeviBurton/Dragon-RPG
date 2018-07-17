﻿using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : AbilityBehavior
    {
        public override void Use(GameObject target)
        {
            DealDamage(target);
            PlayAbilitySound();
            PlayParticleEffect();
            PlayAbilityAnimation();
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().Damage(damageToDeal);
        }
    }
}