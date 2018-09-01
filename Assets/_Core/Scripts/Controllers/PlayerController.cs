using UnityEngine;

using RPG.CameraUI;

using System.Collections;
using System.Collections.Generic;
using RPG.Characters;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Linq;

namespace RPG.Character
{
    public enum EMouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayerMask = (1 << 9);
        [SerializeField] LayerMask walkableLayerMask = (1 << 8);
        [SerializeField] LayerMask friendlyMask = (1 << 10);
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D npcCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] Transform axisToolPrefab =  null;

        Transform axisToolInstance = null;

        // TODO: NOte that this list maintains our party order.
        // Also -- this list needs to come from somewhere.  When a player creates a character,
        // this list is populated with the character they created, so its stats need saved, etc.
        // 
        public List<HeroController> playerHeroes;

        public List<HeroController> selectedHeroes;
        public List<EnemyController> selectedEnemies;
        public EnemyController selectedEnemy;
        public Transform mouseWorldTransform;

        bool isMouseOverEnemy = false;
        bool isMouseOverFriendly = false;
        bool isMouseOverPotentiallyWalkable = false;
        bool isSelecting = false;
        bool isFindingMoveToPosition;

        bool actionPaused = false;
        bool slowMotion = false;

        Vector3 mousePosition;
        float maxRaycastDepth = 100f; // Hard coded value
        GameObject objectUnderMouseCursor;
        Vector3 walkablePosition;
        Transform walkToClickTransform;
        Quaternion moveToRotation;
        Vector3 walkToClickRotationPosition;

        [SerializeField] CharacterGroupController heroGroupController;

        void Start()
        {
            playerHeroes = FindObjectsOfType<HeroController>().ToList();
            selectedHeroes = new List<HeroController>();

            axisToolInstance = Instantiate(axisToolPrefab, this.transform);
            axisToolInstance.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }

        // TODO: this is ready for some refactoring.  
        void Update()
        {
            HandlePause();

            if (!EventSystem.current.IsPointerOverGameObject())
            {
                FindWhatsUnderMouse();

                if (Input.GetMouseButtonUp(1))
                {
                    if (isMouseOverPotentiallyWalkable)
                    {
                        isFindingMoveToPosition = false;
                        var formationController = heroGroupController.GetFormationController();
                        formationController.DisableAllProjectors();
                        axisToolInstance.gameObject.SetActive(false);

                        for (int i = 0; i < selectedHeroes.Count; i++)
                        {
                            var character = selectedHeroes[i].GetComponent<CharacterController>();
                            var formationSlot = formationController.formationSlots[i];
                            character.SetTargetCursorWorldPosition(formationSlot.transform.position);
                            character.GetComponent<CommandSystem>().QueueCommand(new Command(ECommandType.MoveToTargetCursor), true);
                        }
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    isSelecting = true;
                    isFindingMoveToPosition = false;

                    mousePosition = Input.mousePosition;

                    if (isMouseOverEnemy)
                    {
                        if (selectedEnemy)
                        {
                            selectedEnemy.GetComponent<Selectable>().Deselect();
                        }

                        var selectable = objectUnderMouseCursor.GetComponent<Selectable>();
                        if (selectable)
                        {
                            selectedEnemy = selectable.GetComponent<EnemyController>();
                            selectable.Select();
                        }
                    }
                    else if (isMouseOverFriendly)
                    {
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
                    if (isMouseOverEnemy)
                    {
                        foreach (var hero in selectedHeroes)
                        {
                            var character = hero.GetComponent<CharacterController>();
                            character.SetTarget(objectUnderMouseCursor);
                            character.SetTargetCursorWorldPosition(objectUnderMouseCursor.transform.position);
                            var targetCollider = objectUnderMouseCursor.GetComponent<CapsuleCollider>();
                            var heroCollider = hero.GetComponent<CapsuleCollider>();

                            if (targetCollider && heroCollider)
                            {
                                character.targetRadius = targetCollider.radius;
                               // character.GetComponent<NavMeshAgent>().stoppingDistance = (character.targetRadius + heroCollider.radius);
                            }
                            else
                            {
                                //character.GetComponent<NavMeshAgent>().stoppingDistance = 0.1f;
                            }

                            character.GetComponent<CommandSystem>().QueueCommand(new Command(ECommandType.MoveAttack), true);
                        }

                        if (selectedEnemy)
                        {
                            selectedEnemy.GetComponent<Selectable>().Deselect();
                        }

                        var selectable = objectUnderMouseCursor.GetComponent<Selectable>();
                        if (selectable)
                        {
                            selectable.Select();
                            selectedEnemy = selectable.GetComponent<EnemyController>();
                        }
                    }

                    else if (isMouseOverPotentiallyWalkable)
                    {
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
                        isFindingMoveToPosition = true;
                        axisToolInstance.position = walkablePosition;
                        axisToolInstance.gameObject.SetActive(true);
                        var formationController = heroGroupController.GetFormationController();

                        // orient based on 1st selected character
                        if (selectedHeroes.Count > 0)
                        { 
                            var p1 = selectedHeroes[0].transform.position;
                            var p2 = walkablePosition;

                            moveToRotation = Quaternion.LookRotation(p2 - p1);
                            axisToolInstance.transform.rotation = moveToRotation;

                            formationController.transform.rotation = moveToRotation;
                            formationController.transform.position = axisToolInstance.position;
                            axisToolInstance.Translate(0, 0, -1.5f, Space.Self);
                            formationController.transform.Translate(0, 0, -1.5f, Space.Self);
                        }
                    }
                }

                else if (Input.GetMouseButtonUp(0))
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

                // If we are selecting, highlight all selectable heros within the selection box
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

                // We are holding a mouse button down and picking a position for our formation to move to.
                if (isFindingMoveToPosition)
                {
                    axisToolInstance.gameObject.SetActive(true);

                    var formationController = heroGroupController.GetFormationController();
                    for (int i = 0; i < selectedHeroes.Count; i++)
                    {
                        var character = selectedHeroes[i].GetComponent<CharacterController>();
                        var formationSlot = formationController.formationSlots[i];
                        formationSlot.SetProjectorEnabled();

                        //Debug.DrawLine(character.transform.position, formationSlot.transform.position, Color.green);
                    }

                    moveToRotation = Quaternion.LookRotation(walkablePosition - axisToolInstance.position);
                    axisToolInstance.transform.rotation = moveToRotation;
                    formationController.transform.rotation = moveToRotation;
                    formationController.transform.position = axisToolInstance.position;
                }
            }

            // This will turn of selecting if our mouse is over a UI element. 
            // It's kind of jarring, so consider something else.  But as of right now,
            // if we mouse over a UI element while a selection rectangle is being drawn, it kind of goofs up.
            //else
            //{
            //    if (isSelecting)
            //    {
            //        foreach (var selectableObject in FindObjectsOfType<Selectable>())
            //        {
            //            selectableObject.Dehighlight();
            //        }
            //    }
            //    isSelecting = false;
            //}
        }

        private void HandlePause()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                actionPaused = !actionPaused;
                if (actionPaused)
                {
                    Time.timeScale = 0.0f;
                }
                else
                {
                    Time.timeScale = 1.0f;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                slowMotion = !slowMotion;
                if (slowMotion)
                {
                    Time.timeScale = 0.35f;
                }
                else
                {
                    Time.timeScale = 1.0f;
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
            // TODO: not sure about these booleans.  
            // Consider a generic "interactable" component.
            isMouseOverEnemy = false;
            isMouseOverFriendly = false;
            isMouseOverPotentiallyWalkable = false;
            //walkablePosition = Vector3.positiveInfinity;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Prioritized list of things to look for.  As soon as one is found, stop looking.
            if (RaycaseForHero(ray))
            {
                return;
            }

            if (RaycastForEnemy(ray))
            {
                return;
            }

            //var formationController = heroGroupController.GetComponentInChildren<FormationController>();
            //if (formationController)
            //{
            //    for (int i = 0; i < formationController.formationOffsets.Count; i++)
            //    {
            //        var offset = formationController.formationOffsets[i];
            //        var position = offset + worldMousePosition;
            //        var screenPoint = Camera.main.WorldToScreenPoint(position);

            //        RaycastHit hitInfo;
            //        Ray offsetRay = Camera.main.ScreenPointToRay(screenPoint);
            //        bool potentiallyWalkableHit = Physics.Raycast(offsetRay, out hitInfo, maxRaycastDepth, walkableLayerMask);
            //        Vector3 walkablePositon = hitInfo.point;
            //        formationController.formationTransforms[i].position = walkablePositon;
            //    }
            //}

            if (RaycastForPotentiallyWalkable(ray))
            {
                return;
            }
        }

        private bool RaycaseForHero(Ray ray)
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
               // mouseWorldTransform.position = walkablePosition;

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


