using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class Selectable : MonoBehaviour
    {
        [SerializeField] public GameObject selectionCirclePrefab;
        [HideInInspector] public GameObject selectionCircle;

        public void Select()
        {
            selectionCircle = Instantiate(selectionCirclePrefab);
            selectionCircle.transform.SetParent(this.transform, false);
        }

        public void Deselect()
        {
            if (selectionCircle != null)
            {
                Destroy(selectionCircle.gameObject);
                selectionCircle = null;
            }
        }
    }
}
