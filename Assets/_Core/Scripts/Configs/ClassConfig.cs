using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Class")]
public class ClassConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;

    [TextArea(3, 30)]
    public string Description;

    public Sprite spriteIcon;

}