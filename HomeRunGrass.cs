using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeRunGrass : MonoBehaviour {

	void OnTriggerEnter (Collider other) {

		if (other.transform.name == "ball_holder(Clone)") {
			print ("Homer!!!");
			GameController.control.ReportStat ("Home Runs", 1);
			//if (other.GetComponent<NewBall> ().Thrown) {
				other.GetComponent<NewBall> ().Throw_Over ();
				other.GetComponent<NewBall> ().Hit = false;
			//}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
