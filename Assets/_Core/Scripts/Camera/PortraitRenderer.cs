using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitRenderer : MonoBehaviour
{
    [SerializeField] RPG.Character.CharacterController characterController;
    private RawImage rawImage;

    void Start ()
    {
        rawImage = GetComponent<RawImage>();
        rawImage.texture = characterController.portraitTexture;
	}
}
