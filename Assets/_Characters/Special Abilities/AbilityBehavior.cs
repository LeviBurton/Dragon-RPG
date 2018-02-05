using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        AbilityConfig config;

        public abstract void Use(AbilityUseParams useParams);


    }
}
