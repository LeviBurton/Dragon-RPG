using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.Character;

public class RecoveryBar : MonoBehaviour {
    [SerializeField] RPG.Character.CharacterController character;
    [SerializeField] Image recoveryImage;

    // TODO: I don't like polling the character for their recovery.  Consider having a 
    // CharacterController event -- onRecoveryChanged()
    void Update ()
    {
        recoveryImage.fillAmount = character.RecoveryAsPercentage;
    }
}
