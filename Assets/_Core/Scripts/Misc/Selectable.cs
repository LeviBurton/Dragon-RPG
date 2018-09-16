using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class Selectable : MonoBehaviour
{
    [HideInInspector] public GameObject selectionCircle;
    [SerializeField] public GameObject selectionCirclePrefab;

    // TODO: consider removing this internal state.  
    // we don't really do anything with it, so why have it.
    public bool isSelected = false;

    public delegate void OnSelected();
    public event OnSelected onSelected;

    public delegate void OnDeselected();
    public event OnDeselected onDeselected;

    public delegate void OnHighlight();
    public event OnHighlight onHighlight;

    public delegate void OnDeHighlight();
    public event OnDeHighlight onDeHighlight;

    public void Select()
    {
        isSelected = true;

        if (onSelected != null)
            onSelected();
    }

    public void Deselect()
    {
        isSelected = false;
        if (onDeselected != null)
            onDeselected();
    }

    public void Highlight()
    {
        if (onHighlight != null)
            onHighlight();
    }

    public void Dehighlight()
    {
        if (onDeHighlight != null)
            onDeHighlight();
    }
}