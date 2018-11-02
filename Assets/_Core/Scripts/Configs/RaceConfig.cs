using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Race")]
public class RaceConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;

    [TextArea(3, 30)]
    public string Description;

    public int BaseMovementSpeed;

    public Sprite spriteIcon;


}