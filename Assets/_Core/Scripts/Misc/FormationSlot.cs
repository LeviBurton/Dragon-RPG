using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationSlot : MonoBehaviour
{
    [SerializeField] GameObject objectInSlot;
    [SerializeField] Projector slotProjector;

    public void Start()
    {
        SetProjectorDisabled();
    }

    public void SetProjectorEnabled()
    {
        slotProjector.enabled = true;        
    }

    public void SetProjectorDisabled()
    {
        slotProjector.enabled = false;
    }

    public void SetObjectInSlot(GameObject objectToSet)
    {
        objectInSlot = objectToSet;
    }

    public GameObject GetObjectInSlot()
    {
        return objectInSlot;
    }
}
