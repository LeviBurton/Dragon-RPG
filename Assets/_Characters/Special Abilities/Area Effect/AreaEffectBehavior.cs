using UnityEngine;

using RPG.Core;

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
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                config.GetRadius(),
                Vector3.up,
                config.GetRadius());

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
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