using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster CameraRaycaster;
    Vector3 CurrentClickTarget;

    [SerializeField] float walkMoveStopRadius = 0.2f;

    private void Start()
    {
        CameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        Character = GetComponent<ThirdPersonCharacter>();
        CurrentClickTarget = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
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

