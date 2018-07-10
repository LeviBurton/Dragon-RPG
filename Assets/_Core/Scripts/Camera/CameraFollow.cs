using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] GameObject gameObjectToFollow;

        // Update is called once per frame
        void LateUpdate()
        {
            if (gameObjectToFollow != null)
            {
                transform.position = gameObjectToFollow.transform.position;
            }
        }
    }
}
