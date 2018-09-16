using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityHelpers
{
    public static int ToAbilityModifier(this int value)
    {
        return Convert.ToInt32(Mathf.Floor((value / 2) - 5));
    }
}