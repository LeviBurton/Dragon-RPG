using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Config
{
    [CreateAssetMenu(menuName = "RPG/Damage Type")]
    public class DamageTypeConfig : ScriptableObject
    {
        public int SortOrder;
        public string Name;
        public string ShortName;

        [TextArea(3, 30)]
        public string Description;

    }
}