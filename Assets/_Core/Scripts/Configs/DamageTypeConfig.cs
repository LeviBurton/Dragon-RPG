using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Config
{
    [CreateAssetMenu(menuName = "RPG/Damage Type")]
    public class DamageTypeConfig : ScriptableObject
    {
        [SerializeField] string name;
        int selectionMask;
    }
}