﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        protected AbilityConfig config;
        const float PARTICLE_CLEAN_UP_DELAY = 20.0f;

        public abstract void Use(AbilityUseParams useParams);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
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
