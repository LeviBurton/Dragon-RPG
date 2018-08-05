using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimeStats : MonoBehaviour {

    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI elapsedTimeText;

    private float elapsedTime = 0.0f;
    private int roundNumber = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        roundNumber = (int)elapsedTime / 6; // 6 seconds per round.
        elapsedTimeText.text = string.Format("Elapsed Time: {0:0.0}", elapsedTime);
        roundText.text = string.Format("Round: {0}", roundNumber + 1);
	}
}
