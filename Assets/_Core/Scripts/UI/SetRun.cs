using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRun : MonoBehaviour {
    CharacterSystem[] chars;

    // Use this for initialization
    void Start()
    {
        chars = FindObjectsOfType<CharacterSystem>();
    }

    public void OnClick()
    {
        foreach (var c in chars)
        {
            c.SetRunning();
        }
    }
}
