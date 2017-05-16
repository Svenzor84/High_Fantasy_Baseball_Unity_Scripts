using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitcher : MonoBehaviour {

	int throwing_power;
	string current_animation;
	GameObject ball_in_hand;
	Vector3 release_point;

	// Use this for initialization
	void Awake () {

		throwing_power = 50;
		release_point = transform.GetChild (4).transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		current_animation = this.GetComponent<Animator> ().GetCurrentAnimatorClipInfo (0) [0].clip.name;

		ball_in_hand = FindBall ();

		//if (Input.GetKeyDown (KeyCode.Q)) {
			
		//}

		//if (Input.GetKeyUp (KeyCode.Q) && ball_in_hand != null) {
			//this.GetComponent<Animator>().SetTrigger ("throw");
		//}

		if (current_animation == "throw_finish" && ball_in_hand != null) {
			ball_in_hand.GetComponent<NewBall>().Throw(release_point, GameObject.FindGameObjectWithTag("strike_zone").transform.position, throwing_power);
			//ball_in_hand.transform.position = GetHand ().transform.position;
			//ball_in_hand.transform.rotation = new Quaternion (0, 0, 0, 0);
		}
	}

	public GameObject GetHand() {

		GameObject pitcher_hand = GameObject.Find("pitching_rubber");

		Transform[] children = GetComponentsInChildren<Transform> ();

		foreach (Transform child in children) {
			if (child.CompareTag ("throw_hand")) {

				pitcher_hand = child.gameObject;
			}
		}
		return pitcher_hand;
	}

	public GameObject FindBall() {

		GameObject ball = null;
		Transform[] children = GetComponentsInChildren<Transform> ();
		foreach (Transform child in children) {
			if (child.CompareTag("Ball")) {
				ball = child.gameObject;
			}
		}
		return ball;
	}

	public void SetThrowingPower(int new_throwing_power)  {
		throwing_power = new_throwing_power;
	}

	public bool BallInHand () {

		if (ball_in_hand != null) {

			return true;
		}

		return false;
	}
}
