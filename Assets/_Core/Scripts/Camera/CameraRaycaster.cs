using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using RPG.Characters;   // So we can detect by type.
using RPG.Controllers;

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour // TODO rename Cursor
    {
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D npcCursor = null;
        [SerializeField] Texture2D enemyCursor = null;
        [SerializeField] LayerMask enemyLayerMask = (1 << 9);
        [SerializeField] LayerMask walkableLayerMask = (1 << 8);
        [SerializeField] LayerMask npcLayerMask = (1 << 10);

        float maxRaycastDepth = 100f; // Hard coded value
        Rect currentScreenRect;

        public delegate void OnMouseOverNPC(GameObject NPC);
        public event OnMouseOverNPC onMouseOverNPC;

        public delegate void OnMouseOverPotentiallyWalkable(Vector3 destination);
        public event OnMouseOverPotentiallyWalkable onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(EnemyController enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        void Update()
        {
            currentScreenRect = new Rect(0, 0, Screen.width, Screen.height);

            // Check if pointer is over an interactable UI element
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // implement UI interaction
            }
            else
            {
                PerformRaycasts();
            }
        }

        void PerformRaycasts()
        {
            if (!currentScreenRect.Contains(Input.mousePosition))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (RaycastForNPC(ray))
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

        private bool RaycastForNPC(Ray ray)
        {
            RaycastHit hitInfo;
            bool enemyHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, npcLayerMask);
            if (enemyHit)
            {
                var gameObjectHit = hitInfo.collider.gameObject;
                Cursor.SetCursor(npcCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverNPC(gameObjectHit);
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
                var gameObjectHit = hitInfo.collider.gameObject;
                var enemy = gameObjectHit.GetComponent<EnemyController>();
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemy);
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
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                if (onMouseOverPotentiallyWalkable != null)
                {
                    onMouseOverPotentiallyWalkable(hitInfo.point);
                }

                return true;
            }

            return false;
        }
    }
}