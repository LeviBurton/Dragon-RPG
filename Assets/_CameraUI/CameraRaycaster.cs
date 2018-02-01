using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

using RPG.Characters;   // So we can detect by type.

namespace RPG.CameraUI
{
    public class CameraRaycaster : MonoBehaviour // TODO rename Cursor
    {
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D enemyCursor = null;

        const int POTENTIALLY_WALKABLE_LAYER = 8;
        float maxRaycastDepth = 100f; // Hard coded value
        Rect screenRectAtStartPlay = new Rect(0, 0, Screen.width, Screen.height);

        public delegate void OnMouseOverPotentiallyWalkable(Vector3 destination);
        public event OnMouseOverPotentiallyWalkable onMouseOverPotentiallyWalkable;

        public delegate void OnMouseOverEnemy(Enemy enemy);
        public event OnMouseOverEnemy onMouseOverEnemy;

        void Update()
        {
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
            if (!screenRectAtStartPlay.Contains(Input.mousePosition))
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (RaycastForEnemy(ray))
            {
                return;
            }

            if (RaycastForPotentiallyWalkable(ray))
            {
                return;
            }
            
        }

        private bool RaycastForEnemy(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<Enemy>();
        
            if (enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }

            return false;
        }

        private bool RaycastForPotentiallyWalkable(Ray ray)
        {
            RaycastHit hitInfo;
            LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
            bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, potentiallyWalkableLayer);

            if (potentiallyWalkableHit)
            {
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverPotentiallyWalkable(hitInfo.point);
                return true;
            }

            return false;
        }
    }
}