using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Dice")]
public class DiceConfig : ScriptableObject
{
    public GameObject prefab;
    public Sprite iconSprite;
    public int SortOrder;
    public string Name;
    public int sides;
}
