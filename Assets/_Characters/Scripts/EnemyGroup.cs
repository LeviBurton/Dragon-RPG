using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    // This class is responsible for controlling a group of enemies.
    // Responsibilite include group formation, movement, coordination of goals, group directives
    // Behavior will be defined by a selected BT script.
    public class EnemyGroup : MonoBehaviour
    {
        // Note that we may not need this -- since we need a parent owner of this component anyway, 
        // we might as well just use the children of that as the list of enemies.
        public List<EnemyAI> enemies;
     
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddEnemy(EnemyAI enemy)
        {
            // OnAddEnemy event
            enemies.Add(enemy);
        }

        public void RemoveEnemy(EnemyAI enemy)
        {
            // OnRemoveEnemy event
            enemies.Remove(enemy);
        }


    }
}