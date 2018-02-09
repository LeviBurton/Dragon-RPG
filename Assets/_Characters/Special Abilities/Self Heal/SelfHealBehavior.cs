using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : AbilityBehavior
    {
        PlayerControl player = null;
      
        private void Start()
        {
            player = GetComponent<PlayerControl>();
        }

        public override void Use(GameObject target)
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetExtraHealth());

            PlayAbilitySound();
            PlayParticleEffect();
            PlayAbilityAnimation();
        }
    }
}
