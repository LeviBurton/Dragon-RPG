using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreModeCameraController : MonoBehaviour
{
    public Transform targetLookAtOffset;
    public Camera mainCamera;
    public Transform target;
    public float followingSpeed = 0.3f;
    public float followingMaxSpeed = 30.0f;
    public float rotationSpeed = 10.0f;
    public float zoomSpeed = 10.0f;
    public float maxZoom = 50.0f;
    public float minZoom = 2.0f;

    Player rewiredPlayer;
    Vector3 positionDampVelocity;
    Vector3 zoomDampVelocity;

    int rewiredPlayerId = 0;
    float inputCameraZoom;
    float inputCameraRotate;
    float zoomPos = 0; //value in range (0, 1) used as t in Matf.Lerp

    void Start ()
    {
        rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);
        zoomPos = 0.5f;
    }

    void Update()
    {
        inputCameraZoom = rewiredPlayer.GetAxis("Camera Zoom");
        inputCameraRotate = rewiredPlayer.GetAxis("Camera Rotate");

        if (target != null)
        {
            FollowTarget();
            RotateAroundTarget();
            LookAtTarget();
            ZoomCamera();
        }
    }

    private void ZoomCamera()
    {
        var cameraZoom = rewiredPlayer.GetAxis("Camera Zoom");
        var distanceToTargetOffset = Vector3.Distance(mainCamera.transform.position, targetLookAtOffset.position);

        // TODO: lerp these values.

        // We are at the minimum extents but want to zoom out.
        if (distanceToTargetOffset <= minZoom && cameraZoom < 0)
        {
            mainCamera.transform.position += mainCamera.transform.forward * cameraZoom;
        }

        // We are at the maximum extents but want to zoom in.
        if (distanceToTargetOffset >= maxZoom && cameraZoom > 0)
        {
            mainCamera.transform.position += mainCamera.transform.forward * cameraZoom;
        }

        // We are in between the min/max extents and can zoom in/out freely.
        if (distanceToTargetOffset < maxZoom && distanceToTargetOffset > minZoom)
        {
            mainCamera.transform.position += mainCamera.transform.forward * cameraZoom;
        }
    }

    private void RotateAroundTarget()
    {
        if (Mathf.Abs(inputCameraRotate) > 0.1f)
        {
            transform.Rotate(inputCameraRotate * Vector3.up * rotationSpeed * Time.unscaledDeltaTime);
        }
    }

    void LookAtTarget()
    {
        mainCamera.transform.LookAt(transform.position + transform.InverseTransformPoint(targetLookAtOffset.position));

        //Vector3 direction = target.position - cameraOffset.position;
        //Quaternion rotation = Quaternion.LookRotation(direction);
        //cameraOffset.rotation = Quaternion.Slerp(transform.rotation, rotation, lookRotationSpeed * Time.unscaledDeltaTime);
    }

    void FollowTarget()
    {
        if (Vector3.Distance(target.position, transform.position) > 0.01f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref positionDampVelocity, followingSpeed, followingMaxSpeed, Time.unscaledDeltaTime);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
