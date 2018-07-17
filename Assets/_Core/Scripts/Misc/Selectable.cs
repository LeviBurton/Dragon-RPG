using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class Selectable : MonoBehaviour
{
    [HideInInspector] public GameObject selectionCircle;
    [SerializeField] public GameObject selectionCirclePrefab;

    private bool isSelected = false;

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
        onSelected();
    }

    public void Deselect()
    {
        isSelected = false;
        onDeselected();
    }

    public void Highlight()
    {
        onHighlight();
    }

    public void Dehighlight()
    {
        onDeHighlight();
    }
}