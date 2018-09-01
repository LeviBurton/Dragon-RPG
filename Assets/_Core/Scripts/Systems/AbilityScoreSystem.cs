using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum EAbilityScoreType
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdon,
    Charisma
}

[Serializable]
public class AbilityScore
{
    public EAbilityScoreType abilityScoreType;
    public int value;

    public AbilityScore(EAbilityScoreType abilityScoreType, int value = 0)
    {
        this.abilityScoreType = abilityScoreType;
        this.value = value;
    }
}

[Serializable]
public class AbilityScoreSet
{
    public AbilityScore Strength;
    public AbilityScore Dexterity;
    public AbilityScore Constitution;
    public AbilityScore Intelligence;
    public AbilityScore Wisdom;
    public AbilityScore Charisma;

    public AbilityScoreSet()
    {
        Strength = new AbilityScore(EAbilityScoreType.Strength, 8);
        Dexterity = new AbilityScore(EAbilityScoreType.Dexterity, 8); 
        Constitution = new AbilityScore(EAbilityScoreType.Constitution, 8);
        Intelligence = new AbilityScore(EAbilityScoreType.Intelligence, 8);
        Wisdom = new AbilityScore(EAbilityScoreType.Wisdon, 8);
        Charisma = new AbilityScore(EAbilityScoreType.Charisma, 8);
    }

    public AbilityScoreSet(int str = 8, int dex = 8, int con = 8, int intel = 8, int wis = 8, int cha = 8)
    {
        Strength = new AbilityScore(EAbilityScoreType.Strength, str);
        Dexterity = new AbilityScore(EAbilityScoreType.Dexterity, dex);
        Constitution = new AbilityScore(EAbilityScoreType.Constitution, con);
        Intelligence = new AbilityScore(EAbilityScoreType.Intelligence, intel);
        Wisdom = new AbilityScore(EAbilityScoreType.Wisdon, wis);
        Charisma = new AbilityScore(EAbilityScoreType.Charisma, cha);
    }
}

public class AbilityScoreSystem : MonoBehaviour
{
    public AbilityScoreSet abilityScoreSet;

    private void Awake()
    {
           
    }

    private void Start()
    {
        
    }

    public AbilityScoreSet GetAbilityScoreSet()
    {
        return abilityScoreSet;
    }

    public void SetAbilityScoreSet(AbilityScoreSet newAbilityScoreSet)
    {
        abilityScoreSet = newAbilityScoreSet;
    }
}
