using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Config
{
    [CreateAssetMenu(menuName = "RPG/Game")]
    public class GameConfig : ScriptableObject
    {
        public float MaxGameLength { get; set; }
    }
}