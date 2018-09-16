using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QuickTools : MonoBehaviour
{
    [MenuItem("Quick Tools/Enable Enemy AI")]
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
    [MenuItem("Quick Tools/Disable Enemy AI")]
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
