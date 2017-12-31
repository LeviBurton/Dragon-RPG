using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter ThirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster CameraRaycaster;
    Vector3 CurrentClickTarget;

    [SerializeField]
    float walkMoveStopRadius = 0.2f;

    bool bIsInDirectMovementMode = false;  

    private void Start()
    {
        CameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        CurrentClickTarget = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G)) // G for gamepad.  
        {
            bIsInDirectMovementMode = !bIsInDirectMovementMode;
            CurrentClickTarget = transform.position;    // clear the click target
        }

        if (bIsInDirectMovementMode)
        {

            ProcessDirectMovement();
        }
        else
        {
            ProcessMouseMovement();
        }
    }

    private void ProcessDirectMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 CamForword = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 Move = v * CamForword + h * Camera.main.transform.right;

        ThirdPersonCharacter.Move(Move, false, false);
    }

    private void ProcessMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            switch (CameraRaycaster.CurrentLayerHit)
            {
                case ELayer.Walkable:
                    CurrentClickTarget = CameraRaycaster.RaycastHit.point;
                    break;

                case ELayer.Enemy:
                    CurrentClickTarget = transform.position;
                    break;

                default:
                    CurrentClickTarget = transform.position;
                    return;
            }
        }

        var PlayerToClickPoint = CurrentClickTarget - transform.position;

        if (PlayerToClickPoint.magnitude > walkMoveStopRadius)
        {
            ThirdPersonCharacter.Move(PlayerToClickPoint, false, false);
        }
        else
        {
            ThirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }
}

