using UnityEngine;

using RPG.CameraUI;

using System.Collections;
using System.Collections.Generic;
using RPG.Characters;
using UnityEngine.EventSystems;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        bool actionPaused = false;

        // TODO: this is ready for some refactoring.  
        void Update()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                FindWhatsUnderMouse();

                if (Input.GetMouseButtonDown(0))
                {
                    isSelecting = true;
                    mousePosition = Input.mousePosition;

                    if (isMouseOverEnemy)
                    {
                        Debug.LogFormat("Clicked on Enemy {0}", objectUnderMouseCursor.name);
                    }

                    else if (isMouseOverFriendly)
                    {
                        Debug.LogFormat("Clicked on Player {0}", objectUnderMouseCursor.name);
                        var selectable = objectUnderMouseCursor.GetComponent<Selectable>();

                        selectable.Select();

                        // TODO: consider changing this to getting a Hero component.
                        var character = selectable.GetComponent<CharacterController>();

                        if (!selectedPlayers.Contains(character))
                        {
                            selectedPlayers.Add(character);
                        }
                    }
                    else
                    {
                        foreach (var character in selectedPlayers)
                        {
                            character.GetComponent<Selectable>().Deselect();
                        }

                        selectedPlayers.Clear();
                    }
                    
                }

                else if (Input.GetMouseButtonDown(1))
                {
                    if (isMouseOverPotentiallyWalkable)
                    {
                        Debug.LogFormat("clicked on {0}", potentiallyWalkableClickedPosition);
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    foreach (var selectable in FindObjectsOfType<Selectable>())
                    {
                        if (IsWithinSelectionBounds(selectable.gameObject))
                        {
                            selectable.Select();

                            var character = selectable.GetComponent<CharacterController>();

                            if (!selectedPlayers.Contains(character))
                            {
                                selectedPlayers.Add(character);
                            }
                        }
                    }

                    isSelecting = false;
                }

                // Highlight all objects within the selection box
                if (isSelecting)
                {
                    foreach (var selectableObject in FindObjectsOfType<Selectable>())
                    {
                        if (IsWithinSelectionBounds(selectableObject.gameObject))
                        {
                            selectableObject.Highlight();
                        }
                        else
                        {
                            selectableObject.Dehighlight();
                        }
                    }
                }
            }
        }

        void OnGUI()
        {
            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = Utils.GetScreenRect(mousePosition, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }

        #region Cursor Affordance
        bool isMouseOverEnemy = false;
        bool isMouseOverFriendly = false;
        bool isMouseOverPotentiallyWalkable = false;

        [SerializeField] LayerMask enemyLayerMask = (1 << 9);
        [SerializeField] LayerMask walkableLayerMask = (1 << 8);
        [SerializeField] LayerMask friendlyMask = (1 << 10);

        float maxRaycastDepth = 100f; // Hard coded value
        GameObject objectUnderMouseCursor;
        Vector3 potentiallyWalkableClickedPosition;

        void FindWhatsUnderMouse()
        {
            isMouseOverEnemy = false;
            isMouseOverFriendly = false;
            isMouseOverPotentiallyWalkable = false;
            potentiallyWalkableClickedPosition = Vector3.positiveInfinity;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Prioritized list of things to look for.  As soon as one is found, stop looking.
            if (RaycastForFriendly(ray))
            {
                return;
            }

            if (RaycastForEnemy(ray))
            {
                return;
            }

            if (RaycastForPotentiallyWalkable(ray))
            {
                return;
            }
        }

        private bool RaycastForFriendly(Ray ray)
        {
            RaycastHit hitInfo;
            bool friendlyHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, friendlyMask);
            if (friendlyHit)
            {
                objectUnderMouseCursor = hitInfo.collider.gameObject;
                Cursor.SetCursor(npcCursor, cursorHotspot, CursorMode.Auto);
                isMouseOverFriendly = true;
                return true;
            }

            return false;
        }

        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            bool enemyHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, enemyLayerMask);
            if (enemyHit)
            {
                objectUnderMouseCursor = hitInfo.collider.gameObject;
                var enemy = objectUnderMouseCursor.GetComponent<EnemyController>();
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                isMouseOverEnemy = true;
                return true;
            }

            return false;
        }

        private bool RaycastForPotentiallyWalkable(Ray ray)
        {
            RaycastHit hitInfo;

            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, walkableLayerMask);

            if (potentiallyWalkableHit)
            {
                isMouseOverPotentiallyWalkable = true;
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                potentiallyWalkableClickedPosition = hitInfo.point;
                return true;
            }

            return false;
        }
        #endregion

        #region Cursors
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D npcCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        #endregion

        #region Selection
        bool isSelecting = false;
        public List<CharacterController> selectedPlayers;
        public List<CharacterController> selectedEnemies;
        public CharacterController selectedCharacter;
        private Vector3 mousePosition;
        public bool IsWithinSelectionBounds(GameObject gameObject)
        {
            if (!isSelecting)
                return false;

            var camera = Camera.main;
            var viewportBounds = Utils.GetViewportBounds(camera, mousePosition, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }

        #endregion

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
    }
}


