using System;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AICharacterControl))]
public class PlayerMovement : MonoBehaviour
{
    ThirdPersonCharacter ThirdPersonCharacter= null;
    AICharacterControl AiCharacterControl = null;
    CameraRaycaster CameraRaycaster = null;
    Vector3 CurrentDestination;
    Vector3 ClickPoint;

    [SerializeField]
    const int walkableLayerNumber = 8;
    [SerializeField]
    const int enemyLayerNumber = 9;

    GameObject WalkTarget = null;

    private void Start()
    {
        CameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        AiCharacterControl = GetComponent<AICharacterControl>();
        ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();

        CameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
        CurrentDestination = transform.position;
        WalkTarget = new GameObject("WalkTarget");
    }
        
    void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
    {
        switch (layerHit)
        {
            case enemyLayerNumber:
                // Navigate to enemy
                GameObject enemy = raycastHit.collider.gameObject;
                AiCharacterControl.SetTarget(enemy.transform);
                break;

            case walkableLayerNumber:
                // Navigate to point on ground
                WalkTarget.transform.position = raycastHit.point;
                AiCharacterControl.SetTarget(WalkTarget.transform);
                break;

            default:
                Debug.LogWarning("Don't know how to handle mouse click for player movement");
                return;

        }
    }



    // TODO make this get called again.
    bool bIsInDirectMovementMode = false;
    private void ProcessDirectMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 CamForword = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 Move = v * CamForword + h * Camera.main.transform.right;

        ThirdPersonCharacter.Move(Move, false, false);
    }
}

