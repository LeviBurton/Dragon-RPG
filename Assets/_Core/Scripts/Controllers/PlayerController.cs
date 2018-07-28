using UnityEngine;

using RPG.CameraUI;

using System.Collections;
using System.Collections.Generic;
using RPG.Characters;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Character
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayerMask = (1 << 9);
        [SerializeField] LayerMask walkableLayerMask = (1 << 8);
        [SerializeField] LayerMask friendlyMask = (1 << 10);
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D npcCursor = null;
        [SerializeField] Texture2D enemyCursor = null;

        public List<HeroController> selectedHeroes;
        public List<EnemyController> selectedEnemies;
        public EnemyController selectedEnemy;

        public CharacterController selectedCharacter;
        public Transform mouseWorldTransform;

        bool isMouseOverEnemy = false;
        bool isMouseOverFriendly = false;
        bool isMouseOverPotentiallyWalkable = false;
        bool isSelecting = false;
        bool actionPaused = false;

        Vector3 mousePosition;
        float maxRaycastDepth = 100f; // Hard coded value
        GameObject objectUnderMouseCursor;
        Vector3 walkablePosition;

        [SerializeField] CharacterGroupController heroGroupController;

        void Start()
        {
           // mouseWorldTransform = Instantiate(mouseWorldTransform, Vector3.zero, Quaternion.identity);
            NavMesh.avoidancePredictionTime = 5.0F;
        }

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
                        if (selectedEnemy)
                        {
                            selectedEnemy.GetComponent<Selectable>().Deselect();
                        }

                        var selectable = objectUnderMouseCursor.GetComponent<Selectable>();
                        selectedEnemy = selectable.GetComponent<EnemyController>();
                        selectable.Select();
                    }

                    else if (isMouseOverFriendly)
                    {
                        Debug.LogFormat("Clicked on Player {0}", objectUnderMouseCursor.name);
                        var selectable = objectUnderMouseCursor.GetComponent<Selectable>();

                        // Only do this if we aren't holding down the SHIFT key, which adds to the selection.
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            foreach (var hero in selectedHeroes)
                            {
                                hero.GetComponent<Selectable>().Deselect();
                            }

                            selectedHeroes.Clear();
                        }

                        selectable.Select();

                        var character = selectable.GetComponent<HeroController>();

                        if (!selectedHeroes.Contains(character))
                        {
                            selectedHeroes.Add(character);
                        }
                    }
                    else
                    {
                        foreach (var character in selectedHeroes)
                        {
                            character.GetComponent<Selectable>().Deselect();
                        }

                        if (selectedEnemy)
                        {
                            selectedEnemy.GetComponent<Selectable>().Deselect();
                            selectedEnemy = null;
                        }
                        selectedHeroes.Clear();
                    }
                    
                }

                else if (Input.GetMouseButtonDown(1))
                {
                    if (isMouseOverPotentiallyWalkable)
                    {
                        Debug.LogFormat("clicked on {0}", walkablePosition);

                        // Here we find all the selected heros, foreach hero we find their formation position,
                        // then off set our click point by that position.  This then becomes the 
                        // heros target.  Note that this requires putting game objects out there --
                        // we already have these in our formation controller, so use those....

                        // Move formation positions to this point, then offset by their local position.  
                        // Then set the targetObject to the formation position game object.

                        // The problem here is that the nav mesh agent path will be a path to the clicked
                        // point, not the formation position point.  also, if each formation point has a different
                        // elevation, this may not work -- so we need to take the y of the formation position 
                        // into consideration.

                        foreach (var hero in selectedHeroes)
                        {
                            var character = hero.GetComponent<CharacterController>();
                            character.SetMovementTarget(walkablePosition);
                          
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    foreach (var selectable in FindObjectsOfType<Selectable>())
                    {
                        if (IsWithinSelectionBounds(selectable.gameObject))
                        {
                            var hero = selectable.GetComponent<HeroController>();

                            if (hero != null)
                            {
                                if (!selectedHeroes.Contains(hero))
                                {
                                    selectable.Select();
                                    selectedHeroes.Add(hero);
                                }
                            }
                        }
                    }

                    isSelecting = false;
                }

                // Highlight all selectable heros within the selection box
                if (isSelecting)
                {
                    foreach (var selectableObject in FindObjectsOfType<Selectable>())
                    {
                        if (IsWithinSelectionBounds(selectableObject.gameObject))
                        {
                            var hero = selectableObject.GetComponent<HeroController>();
                            if (hero != null)
                            {
                                selectableObject.Highlight();
                            }
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
      
        void FindWhatsUnderMouse()
        {
            isMouseOverEnemy = false;
            isMouseOverFriendly = false;
            isMouseOverPotentiallyWalkable = false;
            //walkablePosition = Vector3.positiveInfinity;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Prioritized list of things to look for.  As soon as one is found, stop looking.
            if (RaycastForFriendly(ray))
            {
                return;
            }

            if (RaycastForEnemy(ray))
            {
                return;
            }

            var formationController = heroGroupController.GetComponentInChildren<FormationController>();
            if (formationController)
            {
                for (int i = 0; i < formationController.formationOffsets.Count; i++)
                {
                    var offset = formationController.formationOffsets[i];
                    var position = offset + worldMousePosition;
                    var screenPoint = Camera.main.WorldToScreenPoint(position);

                    RaycastHit hitInfo;
                    Ray offsetRay = Camera.main.ScreenPointToRay(screenPoint);
                    bool potentiallyWalkableHit = Physics.Raycast(offsetRay, out hitInfo, maxRaycastDepth, walkableLayerMask);
                    Vector3 walkablePositon = hitInfo.point;
                    formationController.formationTransforms[i].position = walkablePositon;
                }
            }

            //if (RaycastForPotentiallyWalkable(ray))
            //{
            //    return;
            //}
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
            // TODO: think about if we should handle formations here, or somewhere else.  Here would be 
            // convenient since we can handle terrain elevation when providing a click position in the world.
            RaycastHit hitInfo;

            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, walkableLayerMask);
         
            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                isMouseOverPotentiallyWalkable = true;
                walkablePosition = hitInfo.point;

                mouseWorldTransform.position = walkablePosition;

                // Get formation controller
                var formationController = heroGroupController.GetComponentInChildren<FormationController>();

                if (formationController)
                {
                    for (int i = 0; i < formationController.formationTransforms.Count; i++)
                    {
                        var transform = formationController.formationTransforms[i];
                        var offset = formationController.formationOffsets[i];
                        transform.position = walkablePosition + offset; 
                    }
                }

                return true;
            }

            return false;
        }
        #endregion


        #region Selection
      
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


