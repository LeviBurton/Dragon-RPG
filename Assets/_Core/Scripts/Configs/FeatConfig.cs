using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Put these requirement classes into their own file at some point -- they should not live 
// here since we can use them throughout the project.
[Serializable]
public class SkillRequirement
{
    public SkillConfig skill;

    [Range(0, 30)]
    public int ranks;
}

[Serializable]
public class AbilityRequirement
{
    public AbilityConfig ability;

    [Range(0, 30)]
    public int value;
}

[Serializable]
public class FeatRequirement
{
    [Reorderable]
    public List<SkillRequirement> requiredSkills;

    [Reorderable]
    public List<AbilityRequirement> requiredAbilities;

    [Reorderable]
    public List<FeatConfig> otherFeatRequirements;
}

[CreateAssetMenu(menuName = "RPG/Feat")]
public class FeatConfig : ScriptableObject
{
    public bool enabled = true;
    public int SortOrder;

    public FeatConfig parentFeatConfig;

    public string Name;
    public string ShortName;

    [TextArea(3, 30)]
    public string Description;

    public FeatRequirement featRequirements;
}
