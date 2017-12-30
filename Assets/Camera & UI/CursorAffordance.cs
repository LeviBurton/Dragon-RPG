using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour
{
    [SerializeField]
    Texture2D WalkCursor = null;

    [SerializeField]
    Texture2D TargetCursor = null;

    [SerializeField]
    Texture2D UnknownCursor = null;

    [SerializeField]
    Vector2 CursorHotSpot = new Vector2(0, 0);

    CameraRaycaster CameraRaycaster;

    // Use this for initialization
    void Start()
    {
        CameraRaycaster = GetComponent<CameraRaycaster>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (CameraRaycaster.CurrentLayerHit)
        {
            case ELayer.Walkable:
                Cursor.SetCursor(WalkCursor, CursorHotSpot, CursorMode.Auto);
                break;

            case ELayer.Enemy:
                Cursor.SetCursor(TargetCursor, CursorHotSpot, CursorMode.Auto);
                break;

            case ELayer.RaycastEndStop:
                Cursor.SetCursor(UnknownCursor, CursorHotSpot, CursorMode.Auto);
                break;

            default:
                break;
        }
    }
}