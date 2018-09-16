using RPG.Character;
using SubjectNerd.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AbilitySystem))]
public class SkillSystem : MonoBehaviour
{
    AbilitySystem abilitySystem;

    [Reorderable]
    public List<SkillConfig> skillConfigs;
    public List<SkillData> skillData = new List<SkillData>();
    
    void Awake()
    {
        foreach (var skillConfig in skillConfigs)
        {
            var data = new SkillData(skillConfig.name, skillConfig, skillConfig.primaryAbility);
            data.ranks = 1;
            skillData.Add(data);
        }
    }

    void Start()
    {
        abilitySystem = GetComponent<AbilitySystem>();

        RunSkillChecks();
    }

    public void RunSkillChecks()
    {
        foreach (var skill in skillData)
        {
            // This works fine.  Note that we don't enforce name uniqueness
            var abilityData = abilitySystem.abilityData.Single(x => x.abilityConfig.name == skill.primaryAbility.name);
            var abilityValue = abilityData.value;
            var modifier = abilityData.value.ToAbilityModifier();
            var ranks = skill.ranks;
          
            Debug.LogFormat("skill_check: {0}, ranks: {1}, ability: {2} {3}, modifier: {4}", skill.skillConfig.name, ranks, skill.primaryAbility.name, abilityValue, modifier);
        }
    }
}
