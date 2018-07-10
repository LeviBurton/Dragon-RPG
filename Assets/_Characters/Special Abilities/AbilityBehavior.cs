using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        protected AbilityConfig config;
        const float PARTICLE_CLEAN_UP_DELAY = 20.0f;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";

        public abstract void Use(GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayAbilityAnimation()
        {
            //var abilityAnimation = config.GetAbilityAnimation();
            //var animatorOverrideController = GetComponent<Character>().GetOverrideController();
            //var animator = GetComponent<Animator>();

            //animator.runtimeAnimatorController = animatorOverrideController;
            //animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimation();
            //animator.SetTrigger(ATTACK_TRIGGER);
        }

        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAbilitySound(); 
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }

        protected void PlayParticleEffect()
        {
            var particlePrefab = config.GetParticlePrefab();
            var particleObject = Instantiate(
                particlePrefab, 
                transform.position, 
                particlePrefab.transform.rotation);

            particleObject.transform.parent = transform;    // set world space in prefab if required
            particleObject.GetComponent<ParticleSystem>().Play();

            StartCoroutine(DestroyParticleWhenFinished(particleObject));
        }

        IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
        {
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }

            Destroy(particlePrefab);

            yield return new WaitForEndOfFrame();
        }
    }
}
