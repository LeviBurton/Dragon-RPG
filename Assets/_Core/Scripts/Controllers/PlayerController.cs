using UnityEngine;

using RPG.CameraUI;

using System.Collections;
using System.Collections.Generic;
using RPG.Characters;

namespace RPG.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayer;

        bool actionPaused;

        void Start()
        {
            actionPaused = false;
            RegisterForMouseEvents();
        }


        void Update()
        {
            ScanKeyboard();

            if (!actionPaused)
            {
                ScanForAbilityKeydown();
            }
        }

        void RegisterForMouseEvents()
        {
            var cameraRaycaster = FindObjectOfType<CameraRaycaster>();

            // We will probably handle a lot more of events like these.
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            cameraRaycaster.onMouseOverNPC += OnMouseOverNPC;
        }


        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            // if left click, add move to destination command
            if (Input.GetMouseButtonDown(0))
            {
                // TODO: queue command to selected characters
                // character.SetDestination(destination);
            }
        }

        void OnMouseOverNPC(GameObject NPC)
        {

        }

        void OnMouseOverEnemy(EnemyController enemy)
        {

        }

        void ScanControllerForInput()
        {

        }
        
        void ScanKeyboard()
        {
        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                actionPaused = !actionPaused;
                if (actionPaused)
                {
                    Time.timeScale = 0.25f;
                }
                else
                {
                    Time.timeScale = 1.0f;
                }
            }
        }

        void ScanForAbilityKeydown()
        {
            // TODO: move this somewhere else.
            //for (int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilitie(); keyIndex++)
            //{
            //    if (Input.GetKeyDown(keyIndex.ToString()))
            //    {
            //        abilities.AttemptSpecialAbility(keyIndex);
            //    }
            //}
        }

        void ScanKeyboardForPause()
        {
         
        }
    }
}


