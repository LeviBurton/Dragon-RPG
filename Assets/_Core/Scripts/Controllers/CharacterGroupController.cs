﻿using RPG.Character;
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
        public List<CharacterSystem> characters;

        FormationController formationController;

        void Start()
        {
            formationController = GetComponentInChildren<FormationController>();
            characters = GetComponentsInChildren<CharacterSystem>().ToList();

            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                if (formationController)
                {
                    var transform = formationController.formationSlots[i].transform;
                    character.transform.position = transform.position;
                }
            }
        }

        public FormationController GetFormationController()
        {
            return formationController;
        }

        public void AddCharacter(CharacterSystem character)
        {
            // OnAddEnemy event
            characters.Add(character);
        }

        public void RemoveEnemy(CharacterSystem character)
        {
            // OnRemoveEnemy event
            characters.Remove(character);
        }

        // When a target location is selected, we take the current group center, calculate the offsets of each enemy from that, then use that offset for each
        // enemies move to command.

    }
}