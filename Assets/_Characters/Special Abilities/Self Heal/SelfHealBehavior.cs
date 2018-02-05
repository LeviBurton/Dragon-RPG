using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config = null;
        Player player = null;
        AudioSource audioSource = null;

        private void Start()
        {
            player = GetComponent<Player>();
            audioSource = GetComponent<AudioSource>();
        }

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        void ISpecialAbility.Use(AbilityUseParams useParams)
        {
            player.Heal(config.GetExtraHealth());

            audioSource.clip = config.GetAudioClip();
            audioSource.Play();
                
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetPartcilePrefab(), transform.position, Quaternion.identity);
            prefab.transform.parent = transform;

            var myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();

            Destroy(prefab, myParticleSystem.main.duration);
        }
    }
}
