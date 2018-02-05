using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehavior : AbilityBehavior
    {
        PowerAttackConfig config;

        public void SetConfig(PowerAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
            var myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();

            Destroy(prefab, myParticleSystem.main.duration);
        }

        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + config.GetExtraDamage();
            useParams.target.TakeDamage(damageToDeal);
        }
    }
}