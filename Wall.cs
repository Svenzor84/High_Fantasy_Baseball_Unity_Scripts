using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	void OnTriggerEnter (Collider other) {

		if (other.name == "ball_holder(Clone)") {
			//print ("Collision with Wall");
			if (other.GetComponent<NewBall> ().Thrown) {
				other.GetComponent<NewBall> ().Throw_Over ();
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
