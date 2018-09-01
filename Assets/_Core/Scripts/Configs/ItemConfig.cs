using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public enum EItemType
    {
        Weapon,
        Armor,
        Item,
        Consumable,
        Materials,
        Junk
    }

    public enum EItemBodySlot
    {
        None,
        Head,
        Neck,
        Body,
        Hands,
        Fingers,
        Waist,
        Legs,
        Feet,
        Weapon_Left,
        Weapon_Right,
        Weapon_Both
    }

   
    [CreateAssetMenu(menuName = "RPG/Item")]
    public class ItemConfig : ScriptableObject
    {
        [Header("Item")]
        [Space(10)]
        public string itemName;
        public EItemBodySlot bodySlot;
        public EItemType itemType;
        public Texture2D itemImage;
        public int cost;
        public int weight;
        public bool isVisibleToInventory = true;
    }
}