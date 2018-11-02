using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityData
{
    [NonSerialized]
    public readonly AbilityConfig abilityConfig;
    public string name;
    public int value;
    public EAbilities Type;

    public AbilityData(EAbilities type, string name, AbilityConfig config, int value)
    {
        this.abilityConfig = config;
        this.Type = type;
        this.name = name;
        this.value = value;
    }
}

[CreateAssetMenu(menuName = "RPG/Ability")]
public class AbilityConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;
    public string ShortName;
    public EAbilities Type;

    [TextArea(3, 30)]
    public string Description;

    public string assetPath;
}
