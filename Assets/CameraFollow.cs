using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    GameObject Player = null;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        if (Player != null)
        {
            transform.position = Player.transform.position;
        }
    }
}
