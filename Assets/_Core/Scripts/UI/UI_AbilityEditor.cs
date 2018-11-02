using RPG.Character;
using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_AbilityEditor : MonoBehaviour
{
    public GameObject rowPrefab;

    void Start ()
    {
        var gt = FindObjectOfType<GameController>();
        var c = gt.GetComponent<CharacterSystem>();

		foreach (var ability in gt.abilityConfigs.OrderBy(x => x.Value.SortOrder))
        {
            var go = Instantiate(rowPrefab, transform);
            var row = go.GetComponent<UI_AbilityEditor_Row>();
            row.abilityData = new AbilityData(ability.Value.Type, ability.Value.Name, ability.Value, 12);
        }
	}
}
