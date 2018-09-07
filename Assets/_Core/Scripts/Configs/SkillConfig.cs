using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
