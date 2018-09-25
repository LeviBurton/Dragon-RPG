using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class QuickTools : MonoBehaviour
{
    [MenuItem("Quick Tools/Encounters/Hide All")]
    static void HideEncounters()
    {
        var encounters = FindObjectsOfType<EncounterSystem>();
        foreach (var encounter in encounters)
        {
            encounter.enableDebugView = false;
        }
    }
    [MenuItem("Quick Tools/Encounters/Show All")]
    static void ShowEncounters()
    {
        var encounters = FindObjectsOfType<EncounterSystem>();
        foreach (var encounter in encounters)
        {
            encounter.enableDebugView = true;
        }
    }

    //[MenuItem("Quick Tools/Locations/Hide All")]
    //static void HideLocations()
    //{
    //    var encounters = FindObjectsOfType<LocationEntryPoint>();
    //    foreach (var encounter in encounters)
    //    {
    //        encounter.enableDebugView = false;
    //    }
    //}
    //[MenuItem("Quick Tools/Locations/Show All")]
    //static void ShowEncounters()
    //{
    //    var encounters = FindObjectsOfType<LocationEntryPoint>();
    //    foreach (var encounter in encounters)
    //    {
    //        encounter.enableDebugView = true;
    //    }
    //}

    [MenuItem("Quick Tools/Heroes/Arm Heroes")]
    static void ArmHeroes()
    {
        var heroes = FindObjectsOfType<HeroController>();

        var weaponAssetGuids = AssetDatabase.FindAssets("t:WeaponConfig");

        foreach (var hero in heroes)
        {
            var weaponSystem = hero.GetComponent<WeaponSystem>();
            var weapon = (WeaponConfig)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(weaponAssetGuids[Random.Range(0, weaponAssetGuids.Length - 1)]), typeof(WeaponConfig));

            weaponSystem.EquipWeapon(weapon);
            weaponSystem.PutWeaponInHand(weapon, weapon.GetUseOtherHand());
        }
    }

    [MenuItem("Quick Tools/Heroes/Disarm Heroes")]
    static void DisarmHeroes()
    {
        var heroes = FindObjectsOfType<HeroController>();

        var weaponAssetGuids = AssetDatabase.FindAssets("t:WeaponConfig");

        foreach (var hero in heroes)
        {
            var weaponSystem = hero.GetComponent<WeaponSystem>();

            weaponSystem.UnequipWeapon(EHand.Two);
            weaponSystem.RemoveWeaponFromHand();
        }
    }

    [MenuItem("Quick Tools/Enemies/Enable AI")]
    static void EnableEnemyAI()
    {
        var enemyAI = FindObjectsOfType<EnemyController>().ToList();

        foreach (var enemy in enemyAI)
        {
            var bt = enemy.GetComponent<Panda.BehaviourTree>();
            if (bt)
                bt.enabled = true;
        }
    }

    [MenuItem("Quick Tools/Enemies/Disable AI")]
    static void DisableEnemyAI()
    {
        var enemyAI = FindObjectsOfType<EnemyController>().ToList();
        
        foreach (var enemy in enemyAI)
        {
            var bt = enemy.GetComponent<Panda.BehaviourTree>();
            if (bt)
                bt.enabled = false;
        }
    }
}

#endif
