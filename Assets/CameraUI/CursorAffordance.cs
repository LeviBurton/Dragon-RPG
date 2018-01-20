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
    [SerializeField]
    const int walkableLayerNumber = 8;
    [SerializeField]
    const int enemyLayerNumber = 9;

    CameraRaycaster CameraRaycaster;

    // Use this for initialization  
    void Start()
    {
        CameraRaycaster = GetComponent<CameraRaycaster>();
        CameraRaycaster.notifyLayerChangeObservers += OnLayerChanged;
    }

    void OnLayerChanged(int NewLayer)
    {
        switch (NewLayer)
        {
            case walkableLayerNumber:
                Cursor.SetCursor(WalkCursor, CursorHotSpot, CursorMode.Auto);
                break;

            case enemyLayerNumber:
                Cursor.SetCursor(TargetCursor, CursorHotSpot, CursorMode.Auto);
                break;

            default:
                Cursor.SetCursor(UnknownCursor, CursorHotSpot, CursorMode.Auto);
                break;
        }
    }

    // TODO consider de-registering OnLayerChanged on leaving all game scenes
}