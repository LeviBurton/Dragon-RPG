using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Responsible for rendering just our character into the portrait layer.
// The attached camera uses a culling mask set to the portrait layer to only render objects in it.
// This sets our mesh and skinned renderers to the portrait layer on pre cull
// Then restores their previous layers on post render..
public class PortraitCamera : MonoBehaviour
{
    [SerializeField] RPG.Character.CharacterController characterController;
    int previousLayer;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();    
    }

    void OnPreCull()
    {
        previousLayer = characterController.gameObject.layer;

        // Move our subject to the "portrait" layer.
        characterController.gameObject.layer = LayerMask.NameToLayer("Portrait");
        foreach (var renderer in characterController.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.gameObject.layer = LayerMask.NameToLayer("Portrait");
        }
        foreach (var renderer in characterController.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.gameObject.layer = LayerMask.NameToLayer("Portrait");
        }
    }

    void OnPostRender()
    {
        // Move our subject back to its original layer.
        characterController.gameObject.layer = previousLayer;
        foreach (var renderer in characterController.GetComponentsInChildren<MeshRenderer>())
        {
            renderer.gameObject.layer = previousLayer;
        }
        foreach (var renderer in characterController.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            renderer.gameObject.layer = previousLayer;
        }
    }
}
