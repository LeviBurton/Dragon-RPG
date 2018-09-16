using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationModeCamera : MonoBehaviour
{
    public Transform cameraOffset;
    public Transform targetOffset;
    public Camera mainCamera;
    public Transform target;
    public float followingSpeed = 0.3f;
    public float rotationSpeed = 10.0f;
    public float zoomSpeed = 10.0f;

    Player rewiredPlayer;
    Vector3 positionDampVelocity;
    int rewiredPlayerId = 0;
    float inputCameraZoom;
    float inputCameraRotate;

    void Start ()
    {
        rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
    }

    void Update()
    {
        inputCameraZoom = rewiredPlayer.GetAxis("Camera Zoom");
        inputCameraRotate = rewiredPlayer.GetAxis("Camera Rotate");

        if (target != null)
        {
            FollowTarget();
            ZoomCamera();
            RotateAroundTarget();
        }
    }

    private void ZoomCamera()
    {
        if (Mathf.Abs(inputCameraZoom) > 0.1f)
        {
            cameraOffset.position += cameraOffset.forward * inputCameraZoom * zoomSpeed * Time.deltaTime;
        }
    }

    private void RotateAroundTarget()
    {
        if (Mathf.Abs(inputCameraRotate) > 0.1f)
        {
            transform.Rotate(inputCameraRotate * Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    void LookAtTarget()
    {
        //Vector3 direction = target.position - cameraOffset.position;
        //Quaternion rotation = Quaternion.LookRotation(direction);
        //cameraOffset.rotation = Quaternion.Slerp(transform.rotation, rotation, lookRotationSpeed * Time.deltaTime);
    }

    void FollowTarget()
    {
        if (Vector3.Distance(target.position, transform.position) > 0.01f)
        {
            var targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
            var sourcePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(sourcePosition, targetPosition, ref positionDampVelocity, followingSpeed);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
