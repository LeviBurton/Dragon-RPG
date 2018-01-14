using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter ThirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster CameraRaycaster;
    Vector3 CurrentDestination;
    Vector3 ClickPoint;

    [SerializeField]
    float WalkMoveStopRadius = 0.2f;

    [SerializeField]
    float AttackMoveStopRadius = 5.0f;

    bool bIsInDirectMovementMode = false;  

    private void Start()
    {
        CameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        CurrentDestination = transform.position;
    }

    private void ProcessDirectMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 CamForword = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 Move = v * CamForword + h * Camera.main.transform.right;

        ThirdPersonCharacter.Move(Move, false, false);
    }

    //private void ProcessMouseMovement()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        ClickPoint = CameraRaycaster.RaycastHit.point;

    //        switch (CameraRaycaster.CurrentLayerHit)
    //        {
    //            case ELayer.Walkable:
    //                CurrentDestination = ShortDestination(ClickPoint, WalkMoveStopRadius);
    //                break;

    //            case ELayer.Enemy:
    //                CurrentDestination = ShortDestination(ClickPoint, AttackMoveStopRadius);
    //                break;

    //            default:
    //                CurrentDestination = transform.position;
    //                return;
    //        }
    //    }

    //    WalkToDestination();
    //}

    private void WalkToDestination()
    {
        var PlayerToClickPoint = CurrentDestination - transform.position;

        // TODO: bug here -- this only takes WalkMoveStopRadius into account.  Needs AttackMoveStopRadius as well
        if (PlayerToClickPoint.magnitude > WalkMoveStopRadius)
        {
           
            ThirdPersonCharacter.Move(PlayerToClickPoint, false, false);
        }
        else
        {
            ThirdPersonCharacter.Move(Vector3.zero, false, false);
        }
    }

    Vector3 ShortDestination(Vector3 Destination, float Scale)
    {
        Vector3 ScaledVector = (Destination - transform.position).normalized * Scale;
        return Destination - ScaledVector;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, CurrentDestination);
        Gizmos.DrawSphere(CurrentDestination, 0.1f);
        Gizmos.DrawSphere(ClickPoint, 0.15f);

        // Draw attack sphere
        // Gizmos.color = new Color(255f, 255f, 0f, .2f);
       // Gizmos.DrawWireSphere(transform.position + Vector3.up * 1.1f, AttackMoveStopRadius);
    }
}

