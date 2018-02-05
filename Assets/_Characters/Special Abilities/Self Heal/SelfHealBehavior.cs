using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;

        private void Start()
        {
            player = GetComponent<Player>();    
        }

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        void ISpecialAbility.Use(AbilityUseParams useParams)
        {
            player.AdjustHealth(-config.GetExtraHealth());

            PlayParticleEffect();

        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetPartcilePrefab(), transform.position, Quaternion.identity);
            var myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();

            Destroy(prefab, myParticleSystem.main.duration);
        }
    }
}
