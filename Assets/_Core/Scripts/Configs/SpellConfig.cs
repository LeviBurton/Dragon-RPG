using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Spell")]
public class SpellConfig : ScriptableObject {
    public string Name;
    [TextArea(3, 30)]
    public string Description;
}
