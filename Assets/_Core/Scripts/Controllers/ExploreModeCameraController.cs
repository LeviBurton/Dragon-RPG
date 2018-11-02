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
    public float zoomControlSensitivity = 0.25f;

    Player rewiredPlayer;
    Vector3 positionDampVelocity;
    Vector3 zoomDampVelocity;

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
    }

    void LateUpdate()
    {
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
        if (Mathf.Abs(inputCameraZoom) < zoomControlSensitivity)
            return;

        var distanceToTargetOffset = Vector3.Distance(mainCamera.transform.position, targetLookAtOffset.position);

        // check to see if we are within the min/max extents, or at the extents.
        // when at minZoom allow zooming out, when at maxZoom allow zooming in, when between, allow zoom in/out.
        bool bCanZoom = distanceToTargetOffset <= minZoom && inputCameraZoom < 0 ||
                        distanceToTargetOffset >= maxZoom && inputCameraZoom > 0 ||
                        distanceToTargetOffset < maxZoom && distanceToTargetOffset > minZoom;

        if (bCanZoom)
        {
            mainCamera.transform.position += mainCamera.transform.forward * inputCameraZoom * zoomSpeed * Time.unscaledDeltaTime;
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
