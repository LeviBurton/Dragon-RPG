using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAffordance : MonoBehaviour
{
    [SerializeField]
    Texture2D WalkCursor = null;

    [SerializeField]
    Texture2D TargetCursor = null;

    [SerializeField]
    Texture2D UnknownCursor = null;

    [SerializeField]
    Vector2 CursorHotSpot = new Vector2(96, 96);

    CameraRaycaster CameraRaycaster;

    // Use this for initialization
    void Start()
    {
        CameraRaycaster = GetComponent<CameraRaycaster>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (CameraRaycaster.LayerHit)
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