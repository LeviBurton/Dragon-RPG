using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.Character;

public class RecoveryBar : MonoBehaviour {
    [SerializeField] RPG.Character.CharacterController character;
    [SerializeField] Image recoveryImage;

	// Use this for initialization
	void Start () {

	}

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
     
    }

    // Update is called once per frame
    void Update () {
        recoveryImage.fillAmount = character.RecoveryAsPercentage;
    }
}
