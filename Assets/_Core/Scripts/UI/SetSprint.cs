using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSprint : MonoBehaviour {
    RPG.Character.CharacterController[] chars;

    // Use this for initialization
    void Start()
    {
        chars = FindObjectsOfType<RPG.Character.CharacterController>();
    }

    public void OnClick()
    {
        foreach (var c in chars)
        {
            c.SetSprinting();
        }
    }
}
