using RPG.Character;
using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAbilities
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma
}

public class AbilitySystem : MonoBehaviour
{
    [Reorderable]
    public List<AbilityConfig> abilityConfigs;

    [Reorderable]
    public List<AbilityData> abilityData = new List<AbilityData>();

    [Reorderable]
    public List<EAbilities> abilities;

    void Awake()
    {
        foreach (var config in abilityConfigs)
        {
            var data = new AbilityData(config.Type, config.name, config, UnityEngine.Random.Range(9, 18));
            abilityData.Add(data);
        }
    }
}
