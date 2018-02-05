using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Area Affect")]
    public class AreaEffectConfig : AbilityConfig
    {
        [Header("Area Effect Attack Specific")]
        [SerializeField] float damageToEachTarget = 15f;
        [SerializeField] float radius = 5f;

        public float GetRadius()
        {
            return radius;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }

        public override AbilityBehavior GetBehaviorComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaEffectBehavior>();
        }
    }
}