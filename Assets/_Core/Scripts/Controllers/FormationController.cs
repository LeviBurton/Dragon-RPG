using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    public List<FormationSlot> formationSlots;
    public List<Vector3> formationOffsets;
    public List<Transform> currentTransforms;
    public List<Vector3> potentialTransformPositions;

    private void Awake()
    {
        formationSlots = new List<FormationSlot>();


        foreach (Transform child in transform)
        {
            formationSlots.Add(child.GetComponent<FormationSlot>());
            formationOffsets.Add(child.localPosition);
        }
    }

    public void DisableAllProjectors()
    {
        foreach (var slot in formationSlots)
        {
            slot.SetProjectorDisabled();
        }
    }
}
