using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkillData
{
    [NonSerialized]
    public readonly SkillConfig skillConfig;

    [NonSerialized]
    public readonly AbilityConfig primaryAbility;

    public string name;
    public int ranks;

    public SkillData(string name, SkillConfig skillConfig, AbilityConfig primaryAbility)
    {
        this.name = name;
        this.skillConfig = skillConfig;
        this.primaryAbility = primaryAbility;
    }

    public void OnExecute(AbilityData abilityData)
    {
        Debug.LogFormat("{0} check for {1}: {2}, modifier {3}",
            skillConfig.Name, primaryAbility.Name, abilityData.value, abilityData.value.ToAbilityModifier());
    }
}

[CreateAssetMenu(menuName = "RPG/Skill")]
public class SkillConfig : ScriptableObject
{
    public bool enabled = true;
    public SkillConfig parentConfig;

    public string Name;
    [TextArea(3, 30)]
    public string Description;
    public bool canUseUntrained;
    public AbilityConfig primaryAbility;

    public Sprite iconSprite;

}
