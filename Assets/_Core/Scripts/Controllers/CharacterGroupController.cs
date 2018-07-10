using RPG.Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Characters
{
    // This class is responsible for controlling a group of characters.
    // Responsibilite include group formation, movement, coordination of goals, group directives
    // Behavior will be defined by a selected BT script.
    public class CharacterGroupController : MonoBehaviour
    {
        // Note that we may not need this -- since we need a parent owner of this component anyway, 
        // we might as well just use the children of that as the list of enemies.
        public List<Controllers.CharacterController> characters;

        // Use this for initialization
        void Start()
        {
            characters = GetComponentsInChildren<Controllers.CharacterController>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void AddCharacter(Controllers.CharacterController character)
        {
            // OnAddEnemy event
            characters.Add(character);
        }

        public void RemoveEnemy(Controllers.CharacterController character)
        {
            // OnRemoveEnemy event
            characters.Remove(character);
        }

        // When a target location is selected, we take the current group center, calculate the offsets of each enemy from that, then use that offset for each
        // enemies move to command.

    }
}