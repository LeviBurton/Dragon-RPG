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
    public bool isStartEntryPoint = false;
    public LocationEntryPoint toLocationEntryPoint;
    public LocationEntryPoint fromLocationEntryPoint;

    public BoxCollider bounds;
    public Color sceneViewLabelColor = Color.green;
    public bool enableSceneViewLabel = false;
    public ELocationEntryType entryType = ELocationEntryType.Both;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
