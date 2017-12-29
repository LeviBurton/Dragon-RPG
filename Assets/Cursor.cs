using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    CameraRaycaster CameraRaycaster;

	// Use this for initialization
	void Start () {
        CameraRaycaster = GetComponent<CameraRaycaster>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print(CameraRaycaster.layerHit);
        }
	}
}
