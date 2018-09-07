using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Class")]
public class ClassConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;

    [TextArea(10, 80)]
    public string Description;

    [Reorderable]
    public SkillConfig[] classSkills;

    public Sprite spriteIcon;

}