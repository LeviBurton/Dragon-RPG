using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = "RPG/Special Ability/Area Affect")]
    public class AreaEffectConfig : SpecialAbility
    {
        [Header("Area Effect Attack Specific")]
        [SerializeField] float damageToEachTarget = 15f;
        [SerializeField] float radius = 5f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            var behaviorComponent = gameObjectToAttachTo.AddComponent<AreaEffectBehavior>();
            behaviorComponent.SetConfig(this);
            behavior = behaviorComponent;
        }

        public float GetRadius()
        {
            return radius;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
    }
}