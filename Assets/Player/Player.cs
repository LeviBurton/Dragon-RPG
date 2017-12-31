using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float MaxHealthPoints;

    float CurrentHealthPoints = 100;

    public float HealthAsPercentage
    {
        get
        {
            return CurrentHealthPoints / (float)MaxHealthPoints;
        }
    }
}
