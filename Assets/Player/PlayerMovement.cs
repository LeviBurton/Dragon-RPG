using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster CameraRaycaster;
    Vector3 CurrentClickTarget;

    [SerializeField]
    float walkMoveStopRadius = 0.2f;

    bool bIsInDirectMovementMode = false;   // Consider making this static

    private void Start()
    {
        CameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        Character = GetComponent<ThirdPersonCharacter>();
        CurrentClickTarget = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G)) // G for gamepad.  
        {
            bIsInDirectMovementMode = !bIsInDirectMovementMode;
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

        Character.Move(Move, false, false);
    }

    private void ProcessMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            switch (CameraRaycaster.LayerHit)
            {
                case ELayer.Walkable:
                    CurrentClickTarget = CameraRaycaster.Hit.point;
                    break;

                case ELayer.Enemy:
                    break;

                default:
                    return;
            }
        }

        var PlayerToClickPoint = CurrentClickTarget - transform.position;

        if (PlayerToClickPoint.magnitude > walkMoveStopRadius)
        {
            Character.Move(PlayerToClickPoint, false, false);
        }
        else
        {
            Character.Move(Vector3.zero, false, false);
        }
    }
}

