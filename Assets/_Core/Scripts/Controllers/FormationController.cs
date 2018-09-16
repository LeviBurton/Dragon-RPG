using RPG.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationController : MonoBehaviour
{
    public List<FormationSlot> formationSlots;
    public List<Vector3> formationOffsets;
    public List<Transform> currentTransforms;
    public List<Vector3> potentialTransformPositions;

    float updatePathTimer = 1.0f;

    private void Awake()
    {
        formationSlots = new List<FormationSlot>();

        foreach (Transform child in transform)
        {
            formationSlots.Add(child.GetComponent<FormationSlot>());
            formationOffsets.Add(child.localPosition);
        }

    }

    void Update()
    {
        updatePathTimer -= Time.deltaTime;

        if (updatePathTimer <= 0)
        {
            updatePathTimer = 1.0f;

            int objIndex = 0;
            var leader = formationSlots[0].GetObjectInSlot();

            transform.position = leader.transform.position;
            transform.rotation = leader.transform.rotation;

            for (objIndex = 1; objIndex < formationSlots.Count; objIndex++)
            {
                var slot = formationSlots[objIndex];

                if (slot == null)
                    continue;

                var objectInSlot = slot.GetObjectInSlot();
                if (objectInSlot == null)
                    continue;

                // TODO: here is a prime example of why we need a simple "Mover" component.
                // The reason is that we have now bound our formation system to the character system, when really
                // the only thing we need to bind to is a "moving" system here.
                var character = objectInSlot.GetComponent<CharacterSystem>();

                if (character)
                {
                    var distance = Vector3.Distance(slot.transform.position, character.transform.position);

                    if (distance > 1.0f)
                    {
                        character.SetDestination(slot.transform.position);
                        character.SetTargetCursorWorldPosition(slot.transform.position);

                        if (distance <= 5.0f)
                        {
                            character.SetWalking();
                        }
                        else if (distance >= 5.0f && distance <= 10.0f)
                        {
                            character.SetRunning();
                        }
                        else
                        {
                            character.SetSprinting();
                        }
                    }
                }
            }
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
