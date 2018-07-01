using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleProjector : MonoBehaviour
{
    public Color circleColor;
    public Material projetorMaterial;

    private Material instanceMaterial;
    private Projector projector;

    [ExecuteInEditMode]
    void Start()
    {
        projector = GetComponent<Projector>();
 
        projector.material = new Material(projetorMaterial);
    }

    [ExecuteInEditMode]
    void Update () {

        projector.material.SetColor("_Color", circleColor);
       
    }
}
