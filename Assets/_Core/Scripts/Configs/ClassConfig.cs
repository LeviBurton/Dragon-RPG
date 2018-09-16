using SubjectNerd.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClassData
{
    [NonSerialized]
    public readonly ClassConfig classConfig;
    public string name;
    public int level;
}

[CreateAssetMenu(menuName = "RPG/Class")]
public class ClassConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;

    [TextArea(10, 80)]
    public string Description;

    [Reorderable]
    public SkillConfig[] classSkills;

    public DiceConfig hitDice;

    public Sprite spriteIcon;
}