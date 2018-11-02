using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour {

    public bool isMyTurn;

    private void OnEnable()
    {
        isMyTurn = false;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
