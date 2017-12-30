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
        transform.position = Player.transform.position;
    }
}
