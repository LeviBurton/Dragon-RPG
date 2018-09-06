﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public string abilityAssetPath;
    public int abilityValue;
}

[CreateAssetMenu(menuName = "RPG/Ability")]
public class AbilityConfig : ScriptableObject
{
    public int SortOrder;
    public string Name;
    public string ShortName;

    [TextArea(3, 30)]
    public string Description;
}
