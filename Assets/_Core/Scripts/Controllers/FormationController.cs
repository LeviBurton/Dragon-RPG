using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    public List<Transform> formationTransforms;
    public List<Vector3> formationOffsets;
    public List<Transform> currentTransforms;
    public List<Vector3> potentialTransformPositions;

    private void Awake()
    {
        formationTransforms = new List<Transform>();

        foreach (Transform child in transform)
        {
            formationTransforms.Add(child);
            formationOffsets.Add(child.localPosition);
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}
}
