using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBall : MonoBehaviour {

	private Vector3 throw_end;
	private Vector3 break_target;
	private float break_change;
	private float ball_speed;
	private float throw_distance;
	private float remaining_distance;
	private int ballFlight;
	private bool thrown;
	private float height_mod;

	private bool hit;
	private bool caught;
	private bool hit_to_left;
	private bool hit_by_batter;

	private Rigidbody rb;

	private float current_distance;

	private string throw_description;

	// Use this for initialization
	void Awake () {

		ballFlight = 0;
		thrown = false;
		hit = false;
		caught = false;
		rb = GetComponent<Rigidbody> ();
		current_distance = 0;
		hit_by_batter = false;
		throw_description = "";
	}

	// Update is called once per frame
	void Update () {


		if (thrown && !hit) {

			GetComponent<TrailRenderer> ().time = ball_speed / 1000;

			remaining_distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(throw_end.x - transform.position.x), 2) + Mathf.Pow(Mathf.Abs(throw_end.z - transform.position.z), 2));

			if (hit_by_batter) {
				current_distance = throw_distance - remaining_distance;
			}

			/* This is the cool weird ball path (think trick shots?) also requires height_mod of 100-800 to work
			if (remaining_distance >= (throw_distance / 2)) {
				ballFlight++;
			} else {
				ballFlight--;
			}
			break_target = new Vector3 (throw_end.x, throw_end.y + (break_change * ballFlight), throw_end.z);
			if (break_target.y < throw_end.y) {
				break_target = new Vector3 (throw_end.x, throw_end.y, throw_end.z);
			}
			*/

			transform.LookAt (throw_end);

			//it will have backward relative torque (spin)
			transform.GetChild(0).transform.Rotate (Vector3.back * 100f);

			//next guide the ball to its target through the break target

			break_target = new Vector3 (throw_end.x, throw_end.y + throw_distance / height_mod, throw_end.z);

			//increment the ball flight counter
			ballFlight++;

			if (break_target.y > throw_end.y) {

				break_target.y -= (break_change * ballFlight);
				if (break_target.y < throw_end.y) {
					break_target.y = throw_end.y;
				}
			} else if (break_target.y < throw_end.y) {
				
				break_target.x += (break_change * ballFlight);
				if (break_target.y > throw_end.y) {
					break_target.y = throw_end.y;
				}
			}

			if (break_target.x > throw_end.x) {
				
				break_target.x -= (break_change * ballFlight);
				if (break_target.x < throw_end.x) {
					break_target.x = throw_end.x;
				}
			} else if (break_target.x < throw_end.x) {
				
				break_target.x += (break_change * ballFlight);
				if (break_target.x > throw_end.x) {
					break_target.x = throw_end.x;
				}
			}

			if (break_target.z > throw_end.z) {
				
				break_target.z -= (break_change * ballFlight);
				if (break_target.z < throw_end.z) {
					break_target.z = throw_end.z;
				}
			} else if (break_target.z < throw_end.z) {
				
				break_target.z += (break_change * ballFlight);
				if (break_target.z > throw_end.z) {
					break_target.z = throw_end.z;
				}
			}
				
			transform.position = Vector3.MoveTowards(transform.position, break_target, ball_speed * Time.deltaTime);

		}
			
		if (remaining_distance < throw_distance / 2 && !caught) {
			rb.isKinematic = false;
		}

		if ((transform.position == throw_end || transform.position.y < 1f) && thrown) {

		//if (remaining_distance < throw_distance / 2) {
			/*ballFlight = 0;
			ball_speed = ball_speed / 2f;
			height_mod = height_mod * 7;
			//throw_end = new Vector3(throw_end.x + (transform.forward.x * throw_distance / 35), 0, throw_end.z + (transform.forward.z * throw_distance / 35));
			throw_end = new Vector3(throw_end.x + (transform.forward.x * throw_distance / 15), 0, throw_end.z + (transform.forward.z * throw_distance / 15));
			*/
			//ball_speed = rb.velocity.magnitude;
			//rb.isKinematic = false;

			Throw_Over ();
			//hit = true;
			//thrown = false;
			//rb.AddForce (new Vector3 (transform.forward.x, 0, transform.forward.z) * (ball_speed * ball_speed));
		}

		if (hit) {

			GetComponent<TrailRenderer> ().time = rb.velocity.magnitude / 100;

			if (transform.position.y < 0.6f && rb.velocity.magnitude < 10) {
				hit = false;
				//rb.isKinematic = true;
			}
		}
	}

	void FixedUpdate () {
		if (hit && !thrown) {

			/*
			remaining_distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(throw_end.x - transform.position.x), 2) + Mathf.Pow(Mathf.Abs(throw_end.z - transform.position.z), 2));

			transform.LookAt (throw_end);

			//it will have backward relative torque (spin)
			transform.GetChild (0).transform.Rotate (Vector3.back * 100f);

			//next guide the ball to its target through the break target

			break_target = new Vector3 (throw_end.x, throw_end.y + throw_distance / height_mod, throw_end.z);

			//increment the ball flight counter
			ballFlight++;

			if (break_target.y > throw_end.y) {

				break_target.y -= (break_change * ballFlight);
				if (break_target.y < throw_end.y) {
					break_target.y = throw_end.y;
				}
			} else if (break_target.y < throw_end.y) {

				break_target.x += (break_change * ballFlight);
				if (break_target.y > throw_end.y) {
					break_target.y = throw_end.y;
				}
			}

			if (break_target.x > throw_end.x) {

				break_target.x -= (break_change * ballFlight);
				if (break_target.x < throw_end.x) {
					break_target.x = throw_end.x;
				}
			} else if (break_target.x < throw_end.x) {

				break_target.x += (break_change * ballFlight);
				if (break_target.x > throw_end.x) {
					break_target.x = throw_end.x;
				}
			}

			if (break_target.z > throw_end.z) {

				break_target.z -= (break_change * ballFlight);
				if (break_target.z < throw_end.z) {
					break_target.z = throw_end.z;
				}
			} else if (break_target.z < throw_end.z) {

				break_target.z += (break_change * ballFlight);
				if (break_target.z > throw_end.z) {
					break_target.z = throw_end.z;
				}
			}


			rb.AddForce (break_target * ball_speed);
		}

		//if (transform.position == throw_end) {
		if (ball_speed < 15) {
			thrown = false;
			rb.isKinematic = true;
			*/
		}

	}

	public void Throw(Vector3 source, Vector3 target, int throw_strength, bool random_height = false, string description = "") {

		ballFlight = 0;

		if (random_height) {
			height_mod = Random.Range (1, 8);
		} else {
			height_mod = 4;
		}
		transform.position = source;
		throw_end = target;
		ball_speed = 5f * throw_strength;
		//break_change = ((0.00625f * ball_speed) * 4) / height_mod;
		break_change = ((0.005f * ball_speed) * 4) / height_mod;
		throw_distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(target.x - source.x), 2) + Mathf.Pow(Mathf.Abs(target.z - source.z), 2));
		transform.parent = null;
		thrown = true;
		hit = false;
		caught = false;
		//rb.isKinematic = false;
		throw_description = description;
	}

	public void PhysicsFlight(Vector3 source, Vector3 target, int hit_strength, bool random_height = false) {

		ballFlight = 0;

		if (random_height) {
			height_mod = Random.Range (1, 8);
		} else {
			height_mod = 4;
		}
		transform.position = source;
		throw_end = target;
		ball_speed = 5f * hit_strength;
		//break_change = ((0.00625f * ball_speed) * 4) / height_mod;
		break_change = ((0.005f * ball_speed) * 4) / height_mod;
		throw_distance = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(target.x - source.x), 2) + Mathf.Pow(Mathf.Abs(target.z - source.z), 2));
		transform.parent = null;
		hit = true;
		thrown = false;
		caught = false;
		rb.isKinematic = false;
	}

	public void Throw_Over() {
		hit = true;
		thrown = false;
		rb.AddForce (new Vector3 (transform.forward.x, 0, transform.forward.z) * (ball_speed * (ball_speed / 2)));
		//rb.AddForce (new Vector3 (throw_end.x, throw_end.y, throw_end.z));
		//rb.AddForce (transform.forward * (ball_speed * 2));
		//hit_by_batter = false;
		throw_description += Mathf.Round(((current_distance / 10) * 2.5f)) + " feet - " + Mathf.Round((ball_speed / 3)) + " mph";
	}

	public bool Thrown {

		get {
			return thrown;
		}

		set {
			thrown = value;
		}
	}

	public float GetSpeed() {

		return ball_speed;
	}

	public bool Hit {

		get {
			return hit;
		}

		set {
			hit = value;
		}
	}

	public bool Hit_to_left {

		get {
			return hit_to_left;
		}

		set {
			hit_to_left = value;
		}
	}

	public bool Caught {

		get {
			return caught;
		}

		set {
			caught = value;
		}
	}

	public float Current_distance {

		get {
			return current_distance;
		}
	}

	public bool Hit_by_batter {

		get {
			return hit_by_batter;
		}

		set {
			hit_by_batter = value;
		}
	}

	public string Throw_description {
		get {
			return throw_description;
		}
		set {
			throw_description = value;
		}
	}
}
