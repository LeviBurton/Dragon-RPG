using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test_moveto : MonoBehaviour {

    public Transform target;
    NavMeshAgent aiAgent;

	// Use this for initialization
	void Start () {
        aiAgent = GetComponent<NavMeshAgent>();
     
	}
	
	// Update is called once per frame
	void Update () {
        aiAgent.destination = target.position;

    }
}
