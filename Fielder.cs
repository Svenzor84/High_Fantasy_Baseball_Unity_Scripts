using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fielder : MonoBehaviour {

	private NewBall ball;
	private GameObject catch_hand;
	private GameObject throw_hand;

	private int throwing_power;
	private Vector3 release_point;

	private Animator fielder_anim;

	// Use this for initialization
	void Awake () {

		throwing_power = 50;
		release_point = transform.GetChild (4).transform.position;

		fielder_anim = GetComponent<Animator> ();

		Transform[] children = GetComponentsInChildren<Transform> ();

		foreach (Transform child in children) {
			if (child.CompareTag ("throw_hand")) {

				throw_hand = child.gameObject;
			}

			if (child.CompareTag ("catch_hand")) {

				catch_hand = child.gameObject;
			}
		}

		GetComponent<BoxCollider> ().center = new Vector3 (-0.00719654f, 1.171652f, 0.057f);
		//GetComponent<BoxCollider> ().size = new Vector3 (1.955224f, 2.3065561f, 0.452277f);
		GetComponent<BoxCollider> ().size = new Vector3 (1.955224f, 2.3065561f, 1.76674f);
	}

	void OnTriggerEnter(Collider other) {
		if (other.name == "ball_holder(Clone)") {

			ball = other.GetComponent<NewBall> ();

			StartCoroutine (CatchBall ());
		}

	}


	// Update is called once per frame
	void Update () {
		
	}

	public int Throwing_Power {

		get {
			return throwing_power;
		}

		set {
			throwing_power = value;
		}
	}

	private IEnumerator CatchBall() {

		fielder_anim.SetTrigger ("catch_prep_upper");

		if (ball.Thrown) {
			ball.Throw_Over ();
		}

		if (ball.Hit) {
			ball.Hit = false;
		}

		ball.Caught = true;

		ball.transform.parent = catch_hand.transform;
		ball.transform.localPosition = new Vector3 (-0.01f, 0.003f, 0.062f);

		ball.GetComponent<Rigidbody> ().isKinematic = true;

		yield return new WaitForSeconds (0.15f);

		fielder_anim.SetTrigger ("catch_finish_upper");

		yield return new WaitForSeconds (0.15f);

		ball.transform.parent = throw_hand.transform;
		ball.transform.localPosition = new Vector3 (-0.01f, 0.003f, 0.062f);
	}
}
