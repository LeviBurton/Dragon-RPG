using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: figure out how to only render the character stored in characterController.
public class PortraitCamera : MonoBehaviour
{
    [SerializeField] RPG.Character.CharacterController characterController;
    int previousLayer;

    // callback to be called before any camera starts rendering
    public void MyPreRender(Camera cam)
    {
        if (cam.name == "PortraitCamera")
        {
            foreach (var renderer in characterController.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.gameObject.layer = 13;
            }
        }
      
    }

    public void MyPostRender(Camera cam)
    {
        foreach (var renderer in characterController.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.gameObject.layer = 11;
        }
    }

    public void OnEnable()
    {
        // register the callback when enabling object
        Camera.onPreRender += MyPreRender;
        Camera.onPostRender += MyPostRender;
    }

    public void OnDisable()
    {
        // remove the callback when disabling object
        Camera.onPreRender -= MyPreRender;
        Camera.onPostRender -= MyPostRender;
    }
}
