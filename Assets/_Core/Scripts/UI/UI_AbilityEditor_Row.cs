using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_AbilityEditor_Row : MonoBehaviour {

    public AbilityData abilityData;
    public TextMeshProUGUI abilityNameText;
    public TextMeshProUGUI abilityValueText;

	// Use this for initialization
	void Start ()
    {
        abilityNameText.text = abilityData.abilityConfig.Name;
        abilityValueText.text = abilityData.value.ToString();
	}
	
}
