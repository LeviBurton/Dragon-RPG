﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : AbilityBehavior
    {
        Player player = null;
      
        private void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            player.Heal((config as SelfHealConfig).GetExtraHealth());

            PlayAbilitySound();
            PlayParticleEffect();
        }
    }
}
