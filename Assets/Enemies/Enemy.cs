﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float maxHealthPoints = 100;

    float currentHealthPoints = 100;

    [SerializeField]
    float AttackRadius = 5;

    AICharacterControl AICharacterControl = null;
    Transform OriginalPosition;
    GameObject Player = null;

    private void Start()
    {
        AICharacterControl = GetComponent<AICharacterControl>();
        Player = GameObject.FindGameObjectWithTag("Player");

        OriginalPosition = AICharacterControl.transform;
    }

    private void Update()
    {
        float DistanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        if (DistanceToPlayer <= AttackRadius)
        {
            AICharacterControl.SetTarget(Player.transform);
        }
        else
        {
            AICharacterControl.SetTarget(OriginalPosition);
        }
    }


    public float healthAsPercentage
    {
        get
        {
            return currentHealthPoints / (float)maxHealthPoints;
        }
    }
}
