using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Batter : MonoBehaviour {

	private bool swinging;
	private bool swung;
	private bool ball_in_zone;
	private bool ball_through_zone;
	private bool batter_late;
	private int hitting_power;
	private int weapon_power_bonus;
	private float power_meter;
	private float meter_time;
	private int accuracy;
	private Vector3 batter_target;
	private GameObject ball;

	//field targets
	private Vector3 left_field_post;
	private Vector3 left_field_gap;
	private Vector3 center_field_target;
	private Vector3 right_field_gap;
	private Vector3 right_field_post;
	private Vector3 home_plate;

	private string hit_description;

	// Use this for initialization
	void Awake () {

		swinging = false;
		swung = false;
		ball_in_zone = false;
		ball_through_zone = false;
		hitting_power = 50;
		accuracy = 50;
		power_meter = 0;
		meter_time = 0;
		weapon_power_bonus = 0;

		left_field_post = GameObject.Find ("left_field_post").transform.position;
		left_field_gap = GameObject.Find ("left_field_gap").transform.position;
		center_field_target = GameObject.Find ("center_field_target").transform.position;
		right_field_gap = GameObject.Find ("right_field_gap").transform.position;
		right_field_post = GameObject.Find ("right_field_post").transform.position;
		home_plate = GameObject.Find ("home_plate").transform.position;

		batter_target = new Vector3 (center_field_target.x, center_field_target.y, center_field_target.z);

		hit_description = "";
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.A)) {
			meter_time = Time.time;
		}

		if (Input.GetKey(KeyCode.A)) {
			if (Time.time - meter_time >= 0.15f && Time.time - meter_time <= 0.65f) {
				power_meter = (Time.time - meter_time) - 0.15f;
			} else if (Time.time - meter_time > 0.65f){
				power_meter = 1 - (Time.time - meter_time) - 0.15f;
			}
		}

		if (power_meter < 0) {
			power_meter = 0;
		}
			
	}

	void OnGUI() {

		if (power_meter < 0.15) {
			GUI.color = Color.red;
		} else if (power_meter < 0.35) {
			GUI.color = Color.yellow;
		} else {
			GUI.color = Color.green;
		}
		//GUI.Box (new Rect(Screen.width / 2, 0, Screen.width / 30, Screen.height / 30), power_meter.ToString("F2"), GUI.skin.button);
		GUI.DrawTexture(new Rect((Screen.width / 2) - ((Screen.width / 2) * (power_meter / 0.5f)), 0, Screen.width * (power_meter / 0.5f), (Screen.height / 20) * (power_meter / 0.5f)), GameController.control.white);
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.CompareTag("attack")) {
			//swung = false;
			swinging = true;
			//print ("swing = " + swinging);

			if (ball_in_zone || ball_through_zone) {
				batter_late = true;
			} else {
				batter_late = false;
			}
		}

		if (other.gameObject.name == "ball_holder(Clone)") {
			//ball_through_zone = false;
			ball_in_zone = true;
			//print ("ball in zone");
			if (!swinging) {
				ball = other.gameObject;
			}
		}

		if (swinging && ball_in_zone) {

			if (power_meter == 0.5) {
				hit_description = "<color=green>Perfect</color> ";
			} else if (power_meter < 0.15) {
				hit_description = "<color=red>Weak</color> ";
			} else if (power_meter < 0.35) {
				hit_description = "<color=yellow>Normal</color> ";
			} else {
				hit_description = "<color=green>Strong</color> ";
			}

			int shift_dir = Random.Range (0, 2);
			float shift_mag = Random.Range (0f, 1f);
			//float shift_mag = Random.Range (0f, 1f) - (Random.Range(0, accuracy + 1) / 100f);
			//if (shift_mag < 0.0f) {
			//	shift_mag = 0.0f;
			//}

			hit_description += "contact to ";

			if (batter_late && ball != null) {

				ball.GetComponent<NewBall>().Hit_to_left = false;

				batter_target = new Vector3 (right_field_gap.x, right_field_gap.y, right_field_gap.z);

				if (shift_dir == 0) {

					float shift_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (right_field_post.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (right_field_post.z - batter_target.z), 2));

					//print (shift_max + " x " + shift_mag + " = " + shift_max * shift_mag);

					batter_target = Vector3.MoveTowards (batter_target, right_field_post, shift_max * shift_mag);
					//print ("HIT TO RIGHT CORNER!!");
					hit_description += "Right Field\n";

				} else {

					float shift_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (center_field_target.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (center_field_target.z - batter_target.z), 2));

					//print (shift_max + " x " + shift_mag + " = " + shift_max * shift_mag);

					batter_target = Vector3.MoveTowards (batter_target, center_field_target, shift_max * shift_mag);
					//print ("HIT TO RIGHT CENTER!!");
					hit_description += "Center Field\n";
				}

				float dist_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (home_plate.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (home_plate.z - batter_target.z), 2));
				float dist_shift = 1 - (((0.5f + power_meter) * (hitting_power / 100f)) + power_meter);
				batter_target = Vector3.MoveTowards (batter_target, home_plate, dist_max * dist_shift);

				//ball.GetComponent<NewBall> ().Throw (other.transform.position, batter_target, hitting_power, true, hit_description);
				ball.GetComponent<NewBall> ().Throw (other.transform.position, batter_target, Mathf.RoundToInt(((hitting_power + weapon_power_bonus) * power_meter) / 0.5f), true, hit_description);
				ball.GetComponent<NewBall> ().Hit_by_batter = true;
				//print ("stuff");
				GameController.control.ReportStat ("Power Meter TOTAL", 200 * power_meter);
				string pm_avg = (GameController.control.Mode_metrics["Power Meter TOTAL"] / (GameController.control.Mode_metrics["Hit Total"] + 1)).ToString().Substring(0, 4);
				GameController.control.ReportStat("Power Meter Avg", Mathf.Round(float.Parse(pm_avg)), true);
				//ball.GetComponent<NewBall> ().PhysicsFlight (other.transform.position, batter_target, hitting_power, true);
				//ball.AddComponent<TrailRenderer> ();

			} else {
				
				other.gameObject.GetComponent<NewBall> ().Hit_to_left = true;

				batter_target = new Vector3(left_field_gap.x, left_field_gap.y, left_field_gap.z);

				if (shift_dir == 0) {

					float shift_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (left_field_post.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (left_field_post.z - batter_target.z), 2));

					//print (shift_max + " x " + shift_mag + " = " + shift_max * shift_mag);

					batter_target = Vector3.MoveTowards (batter_target, left_field_post, shift_max * shift_mag);
					//print ("HIT TO LEFT CORNER!!");
					hit_description += "Left Field\n";

				} else {

					float shift_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (center_field_target.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (center_field_target.z - batter_target.z), 2));

					//print (shift_max + " x " + shift_mag + " = " + shift_max * shift_mag);

					batter_target = Vector3.MoveTowards (batter_target, center_field_target, shift_max * shift_mag);
					//print ("HIT TO LEFT CENTER!!");
					hit_description += "Center Field\n";
				}

				float dist_max = Mathf.Sqrt (Mathf.Pow (Mathf.Abs (home_plate.x - batter_target.x), 2) + Mathf.Pow (Mathf.Abs (home_plate.z - batter_target.z), 2));
				float dist_shift = 1 - (((0.5f + power_meter) * (hitting_power / 100f)) + power_meter);
				batter_target = Vector3.MoveTowards (batter_target, home_plate, dist_max * dist_shift);

				//other.GetComponent<NewBall> ().Throw (other.transform.position, batter_target, hitting_power, true, hit_description);
				other.GetComponent<NewBall> ().Throw (other.transform.position, batter_target, Mathf.RoundToInt(((hitting_power + weapon_power_bonus) * power_meter) / 0.5f), true, hit_description);
				other.GetComponent<NewBall> ().Hit_by_batter = true;
				//print ("stuff");
				GameController.control.ReportStat ("Power Meter TOTAL", 200 * power_meter);
				string pm_avg = (GameController.control.Mode_metrics["Power Meter TOTAL"] / (GameController.control.Mode_metrics["Hit Total"] + 1)).ToString().Substring(0, 4);
				GameController.control.ReportStat("Power Meter Avg", Mathf.Round(float.Parse(pm_avg)), true);
				//other.GetComponent<NewBall> ().PhysicsFlight (other.transform.position, batter_target, hitting_power, true);
				//other.gameObject.AddComponent<TrailRenderer> ();
			}
			power_meter = 0;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("attack")) {
			swinging = false;
			//print ("swing = " + swinging);

			swung = true;
		}

		if (other.gameObject.name == "ball_holder(Clone)") {
			ball_in_zone = false;
			//print ("ball out of zone");
			ball_through_zone = true;
			ball = null;
		}
	}

	public void SetHittingPower(int new_hitting_power)  {
		hitting_power = new_hitting_power;
		//print (hitting_power);
	}

	public void SetWeapPowerBonus(int weapon_bonus) {

		weapon_power_bonus = weapon_bonus;
	}

	public void SetAccuracy (int new_accuracy) {
		accuracy = new_accuracy;
	}

	public bool BallHit () {
		if (ball_in_zone && swinging) {
			return true;
		}
		return false;
	}

	public GameObject GetHand() {

		GameObject batter_hand = GameObject.Find("home_plate");

		Transform[] children = GetComponentsInChildren<Transform> ();

		foreach (Transform child in children) {
			if (child.CompareTag ("bat_hand")) {

				batter_hand = child.gameObject;
			}
		}
		return batter_hand;
	}

	public string Hit_description {
		get {
			return hit_description;
		}
		set {
			hit_description = value;
		}
	}

	public bool Swung {
		get {
			return swung;
		}
		set {
			swung = value;
		}
	}

	public bool Ball_through_zone {
		get {
			return ball_through_zone;
		}
		set {
			ball_through_zone = value;
		}
	}

	public bool Batter_late {
		get {
			return batter_late;
		}
		set {
			batter_late = value;
		}
	}

	public float Power_meter {
		get {
			return power_meter;
		}
		set {
			power_meter = value;
		}
	}
}
