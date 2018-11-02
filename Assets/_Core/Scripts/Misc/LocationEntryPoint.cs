using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELocationEntryType
{
    Enter,
    Exit,
    Both
}

public class LocationEntryPoint : MonoBehaviour
{
    public float startCameraRotationAngle = 130.0f;
    public float startCameraZoom = 15.0f;

    public Event_LocationEntryEnter onLocationEnter;
    public Event_LocationEntryExit onLocationExit;

    public bool isStartEntryPoint = false;
    public LocationEntryPoint toLocationEntryPoint;
    public LocationEntryPoint fromLocationEntryPoint;

    public BoxCollider bounds;
    public Color sceneViewLabelColor = Color.green;
    public bool enableSceneViewLabel = false;
    public ELocationEntryType entryType = ELocationEntryType.Both;

    void Start()
    {
        // Test.
        onLocationEnter.Invoke(this);
    }

}
