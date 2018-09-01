using RPG.Character;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [Serializable]
    public class HeroData
    {
        public string name;
        public ECharacterType characterType;
        public ECharacterSize characterSize;
        public float maxHealth;
        public float currentHealth;
        public float currentXP;
        public AbilityScoreSet abilityScores;
    }

    [CreateAssetMenu(menuName = "RPG/Hero")]
    public class HeroConfig : ScriptableObject
    {
        
    }
}
