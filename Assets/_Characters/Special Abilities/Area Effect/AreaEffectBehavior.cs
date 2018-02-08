using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehavior : AbilityBehavior
    {
        public override void Use(GameObject target)
        {
            DealRadialDamage();
            PlayAbilitySound();
            PlayParticleEffect();
        }

        private void DealRadialDamage()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                (config as AreaEffectConfig).GetRadius(),
                Vector3.up,
                (config as AreaEffectConfig).GetRadius());

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerMovement>();

                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}