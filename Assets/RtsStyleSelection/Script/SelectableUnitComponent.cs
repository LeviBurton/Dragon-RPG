using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class SelectableUnitComponent : MonoBehaviour
{
    [HideInInspector] public GameObject selectionCircle;
    [SerializeField] public GameObject selectionCirclePrefab;
}