using UnityEngine;

using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaEffectBehavior : MonoBehaviour, ISpecialAbility
    {
        AreaEffectConfig config;

        public void SetConfig(AreaEffectConfig configToSet)
        {
            this.config = configToSet;
        }

        void ISpecialAbility.Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetPartcilePrefab(), transform.position, Quaternion.identity);
            var myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();

            Destroy(prefab, myParticleSystem.main.duration);
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                config.GetRadius(),
                Vector3.up,
                config.GetRadius());

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();

                if (damageable != null && !hitPlayer)
                {
                    if (damageable as Enemy)
                    {
                        float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                        damageable.TakeDamage(damageToDeal);
                    }
                }
            }
        }
    }
}