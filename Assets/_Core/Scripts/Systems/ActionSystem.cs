using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public List<ActionConfig> actions = new List<ActionConfig>();

    void Start ()
    {
        // Testing out the ActionSystem actions.
        foreach (var action in actions)
        {
            action.Execute(gameObject);
        }
    }
}
