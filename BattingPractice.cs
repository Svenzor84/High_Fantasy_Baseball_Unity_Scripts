using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattingPractice : MonoBehaviour {

	private Player_Character batter;
	private Player_Character pitcher;
	private GameObject batter_mesh;
	private GameObject pitcher_mesh;
	private GameObject catcher_mesh;
	private Animator batter_anim;
	private Animator pitcher_anim;
	private Animator catcher_anim;
	private GameObject ball;
	private GameObject bat;
	private Transform batter_target;
	private float batter_speed;
	private string current_batter_anim;
	private string current_pitcher_anim;
	private GameObject pitcher_hand;
	private GameObject batter_hand;
	private bool pitcher_ready;

	private GameObject left_batters_box;
	private Transform left_batters_box_inner;
	private Transform left_batters_box_outer;
	private GameObject right_batters_box;
	private GameObject pitching_rubber;
	private GameObject second_base;
	private GameObject catcher_pos;
	private GameObject left_field;
	private GameObject center_field;
	private GameObject right_field;

	private Camera main_cam;
	private Camera pitcher_cam;
	private Camera ball_cam;

	private GameObject strike_zone;

	//mode variables - 2P mode not fully implemented yet
	private string mode;

	//button variables
	private float pressed;
	private bool mid_swing;

	private float reset_timer;

	private bool paused;

	private Texture testure;

	//UI variables
	private Vector2 hits_scroll_pos;
	private int selected_hit;
	private string[] hit_list;
	private int selected_session_stat;
	private string[] session_stat_array;

	//GUI content variables
	private string content;

	//session stat tracking variables
	//private IDictionary<string, float> session_stats;

	//modal variables
	private bool exit_modal_open;
	private Rect modal_rect;

	// Use this for initialization
	void Awake () {

		mode = "1P";

		paused = false;

		pitcher_ready = true;

		pitcher_cam = GameObject.Find ("Pitcher_Camera").GetComponent<Camera> ();
		ball_cam = GameObject.Find ("Ball_Camera").GetComponent<Camera> ();
		main_cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		pitcher_cam.enabled = false;
		ball_cam.enabled = false;

		this.gameObject.transform.position = new Vector3 (530.51f, 20.01f, 531.04f);
		this.gameObject.transform.rotation = Quaternion.Euler (11.4f, -135.58f, 0f);

		ball_cam.gameObject.transform.position = new Vector3 (530.51f, 20.01f, 531.04f);
		ball_cam.gameObject.transform.rotation = Quaternion.Euler (11.4f, -135.58f, 0f);

		batter = GameController.control.GetPlayerById (GameController.control.GetBatterId ());
		pitcher = GameController.control.GetPlayerById (GameController.control.GetPitcherId ());

		left_batters_box = GameObject.Find ("left_batters_box");
		left_batters_box_inner = GameObject.Find ("left_batters_box_inner").transform;
		left_batters_box_outer = GameObject.Find ("left_batters_box_outer").transform;

		right_batters_box = GameObject.Find ("right_batters_box");
		pitching_rubber = GameObject.Find ("pitching_rubber");
		second_base = GameObject.Find ("second_base");

		catcher_pos = GameObject.Find ("catcher_pos");

		left_field = GameObject.Find ("left_field_gap");
		center_field = GameObject.Find ("center_field_target");
		right_field = GameObject.Find ("right_field_gap");

		//spawn the batter at the plate and the pitcher on the mound
		batter_mesh = SpawnPlayer(batter, new Vector3 (574.3f, 0.5f, 478.5f), Quaternion.Euler(0, -42.2f, 0));
		batter_mesh.transform.localScale = new Vector3 (13, 13, 13);
		batter_mesh.AddComponent<Batter> ().SetHittingPower(batter.GetStats()["Hitting Power"]);
		batter_mesh.GetComponent<Batter> ().SetAccuracy (batter.GetStats()["Accuracy"]);
		batter_mesh.GetComponent<Batter> ().SetWeapPowerBonus (batter.GetAtts()[GameController.control.GetWeapon(batter.Equipped_weapon).Att] * GameController.control.GetWeapon(batter.Equipped_weapon).Multiplier);
		print (GameController.control.GetWeapon(batter.Equipped_weapon).Att + " " + batter.GetAtts()[GameController.control.GetWeapon(batter.Equipped_weapon).Att] + " x " + GameController.control.GetWeapon(batter.Equipped_weapon).Multiplier);

		//get a reference to the batter's hand and put a weapon in it
		batter_hand = batter_mesh.GetComponent<Batter> ().GetHand ();
		if (batter_hand != null && batter.Equipped_weapon > 0) {
			
			bat = Instantiate (GameController.control.WeaponMeshes [batter.Equipped_weapon - 1], batter_hand.transform);
			bat.transform.localPosition = new Vector3 (-0.0084f, 0.0078f, 0.0328f);
			bat.transform.localScale = new Vector3 (1.3f, 1.3f, 1.3f);
			bat.transform.localRotation = Quaternion.Euler (2.401f, -3.949f, -19.557f);
		}

		//grab the height of the batter mesh
		float batter_height = batter_mesh.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>().bounds.extents.y;

		//find the strike zone object and set its height dependent on the batter
		strike_zone = GameObject.FindGameObjectWithTag("strike_zone");
		strike_zone.transform.position = new Vector3 (strike_zone.transform.position.x, batter_height / 2, strike_zone.transform.position.z);

		pitcher_mesh = SpawnPlayer (pitcher, pitching_rubber.transform.position, Quaternion.Euler(0, 42, 0));
		pitcher_mesh.transform.localScale = new Vector3 (13, 13, 13);
		pitcher_mesh.AddComponent<Pitcher> ().SetThrowingPower(pitcher.GetStats()["Throwing Power"]);

		catcher_mesh = SpawnPlayer (pitcher, catcher_pos.transform.position, Quaternion.Euler(0, 225, 0));
		catcher_mesh.transform.localScale = new Vector3 (13, 13, 13);
		catcher_mesh.AddComponent<Fielder> ().Throwing_Power = pitcher.GetStats()["Throwing Power"];

		pitcher_hand = pitcher_mesh.GetComponent<Pitcher> ().GetHand ();
		//ball = Instantiate (GameController.control.the_ball, pitcher_hand.transform);
		//ball_cam.transform.parent = ball.transform;
		//ball.AddComponent<NewBall> ();
		//ball.transform.localPosition = new Vector3 (-0.01f, 0.003f, 0.062f);
		//ball.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);

		batter_anim = batter_mesh.GetComponent<Animator> ();
		pitcher_anim = pitcher_mesh.GetComponent<Animator> ();
		catcher_anim = catcher_mesh.GetComponent<Animator> ();

		catcher_anim.SetTrigger ("squat");

		batter_target = left_batters_box.transform;
		mid_swing = false;
		pressed = 0;
		reset_timer = 0;

		hit_list = new string[0];

		//session_stats = new Dictionary<string, float> {
		GameController.control.Mode_metrics = new Dictionary<string, float> {
			{ "Pitches Seen", 0 },
			{ "Swing Total", 0 },
			{ "Hit Total", 0 },
			{ "Swing and Miss", 0},
			{ "Hits To Left", 0 },
			{ "Hits To Center", 0 },
			{ "Hits To Right", 0 },
			{ "Home Runs", 0 },
			{ "Power Meter Avg", 0 },
			{ "Exit Velocity Avg", 0 },
			{ "Distance Avg", 0 },
			{ "Distance Total", 0 },
			{ "Power Meter TOTAL", 0 },
			{ "Exit Velocity TOTAL", 0 },
		};

		exit_modal_open = false;
	}

	void OnGUI () {

		//set up GUI Styles
		GUI.skin = GameController.control.gui_skins[0];
		//GUIStyle center_text = new GUIStyle ("box");
		//center_text.wordWrap = true;
		//center_text.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.fontSize = Screen.width / 120;
		GUI.skin.button.fontSize = Screen.width / 120;
		GUI.skin.label.fontSize = Screen.width / 120;
		//center_text.padding = new RectOffset (7, 7, 7, 7);
		GUI.skin.button.fontStyle = FontStyle.Bold;

		GUI.skin.window.fontSize = Screen.width / 50;

		if (ball != null) {
			content = "" + Mathf.Round (((ball.GetComponent<NewBall> ().Current_distance / 10) * 2.5f)) + " feet";
			//GUI.skin.box.fontSize = Screen.width / 50;

		} else {
			content = "";
		}

		GUI.skin.customStyles [4].fontSize = Screen.width / 70;
		GUI.Label (new Rect (Screen.width - (Screen.width / 4), Screen.height - (Screen.height / 12), Screen.width / 5, Screen.height / 18), content, GUI.skin.customStyles[4]);

		if ((!ball_cam.enabled && !pitcher_cam.enabled) || paused) {

			GUI.skin.window.fontSize = Screen.width / 50;
			GUIContent content = new GUIContent ("", testure, "This panel shows a summary of all hits in the current Batting Practice Session.");
			//GUI.Box (new Rect(Screen.width - (Screen.width / 4), Screen.height / 30, Screen.width / 5, Screen.height / 2), content, GUI.skin.customStyles[3]);
			GUI.Box (new Rect(Screen.width - (Screen.width / 4), Screen.height / 20, Screen.width / 5, Screen.height / 1.45f), content, GUI.skin.customStyles[3]);

			//debug
			//hit_list = new string[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"};

			if (hit_list.Length > 0) {

				GUI.skin.label.fontSize = Screen.width / 100;
				//hits_scroll_pos = GUI.BeginScrollView (new Rect (Screen.width - (Screen.width / 4), Screen.height / 27, Screen.width / 5.05f, Screen.height / 2.05f), hits_scroll_pos, 
				hits_scroll_pos = GUI.BeginScrollView (new Rect (Screen.width - (Screen.width / 4), Screen.height / 17, Screen.width / 5.05f, Screen.height / 1.5f), hits_scroll_pos, 
					//new Rect (Screen.width - (Screen.width / 3.75f), Screen.height / 29f, Screen.width / 5.3f, (Screen.height / (17.0f / hit_list.Length))));
					new Rect (Screen.width - (Screen.width / 3.75f), Screen.height / 19f, Screen.width / 5.3f, (Screen.height / (17.0f / hit_list.Length))));
				selected_hit = GUI.SelectionGrid (new Rect (Screen.width - (Screen.width / 3.75f), Screen.height / 19f, Screen.width / 5.3f, (Screen.height / (17.0f / hit_list.Length))), selected_hit, hit_list, 1, GUI.skin.label);
				GUI.EndScrollView ();

			} else {

				GUI.skin.label.fontSize = Screen.width / 75;
				//GUI.Label (new Rect(Screen.width - (Screen.width / 4), Screen.height / 30, Screen.width / 5, Screen.height / 2), "No Hits Recorded");
				GUI.Label (new Rect(Screen.width - (Screen.width / 4), Screen.height / 20, Screen.width / 5, Screen.height / 1.45f), "No Hits Recorded");
			}

		}

		if (paused) {

			content = "Session Statistics";
			GUI.skin.customStyles[3].fontSize = Screen.width / 75;
			GUI.Box (new Rect(Screen.width / 25, Screen.height / 20, Screen.width / 1.45f, Screen.height / 1.45f), content, GUI.skin.customStyles[3]);

			GUI.Box (new Rect((Screen.width / 25) + (Screen.width / 45), (Screen.height / 20) + (Screen.height / 20), Screen.width / 2.9f, (Screen.height / 1.45f) - (Screen.height / 11)), "", GUI.skin.customStyles[3]);

			GUI.skin.label.fontSize = Screen.width / 100;
			//selected_session_stat = GUI.SelectionGrid (new Rect(Screen.width / 25, (Screen.height / 20) + (Screen.height / 20), Screen.width / 1.45f, (Screen.height / 1.45f) - (Screen.height / 20)), selected_session_stat, session_stat_array, 3, GUI.skin.label);
			selected_session_stat = GUI.SelectionGrid (new Rect((Screen.width / 25) + (Screen.width / 45), (Screen.height / 20) + (Screen.height / 20), Screen.width / 2.9f, (Screen.height / 1.45f) - (Screen.height / 11)), selected_session_stat, session_stat_array, 2, GUI.skin.label);

			GUI.Box (new Rect((Screen.width / 25) + (Screen.width / 45) + (Screen.width / 2.75f), (Screen.height / 20) + (Screen.height / 20), Screen.width / 3.5f, (Screen.height / 2.9f) - (Screen.height / 11)), "Batter", GUI.skin.customStyles[3]);

			string[] temp_strings = new string[] { batter.GetName(), "Hitting Power", };
			GUI.SelectionGrid (new Rect((Screen.width / 25) + (Screen.width / 45) + (Screen.width / 2.75f), (Screen.height / 20) + (Screen.height / 10), Screen.width / 3.5f, (Screen.height / 2.9f) - (Screen.height / 4f)), 0, temp_strings, 1, GUI.skin.label);

			if (( batter.GetStats()["Hitting Power"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((batter.GetStats()["Hitting Power"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect((Screen.width / 25) + (Screen.width / 44) + (Screen.width / 2.7f), Screen.height / 3.7f, Screen.width / (3.7f / (batter.GetStats()["Hitting Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);
			GUI.color = Color.white;
			
			GUI.Box (new Rect((Screen.width / 25) + (Screen.width / 45) + (Screen.width / 2.75f), (Screen.height / 20) + (Screen.height / 20) + ((Screen.height / 2.9f)), Screen.width / 3.5f, (Screen.height / 2.9f) - (Screen.height / 11)), "Pitcher", GUI.skin.customStyles[3]);

			temp_strings = new string[] { pitcher.GetName(), "Throwing Power", };
			GUI.SelectionGrid (new Rect ((Screen.width / 25) + (Screen.width / 45) + (Screen.width / 2.75f), (Screen.height / 20) + (Screen.height / 10) + (Screen.height / 2.9f), Screen.width / 3.5f, (Screen.height / 2.9f) - (Screen.height / 4f)), 0, temp_strings, 1, GUI.skin.label);

			if (( pitcher.GetStats()["Throwing Power"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((pitcher.GetStats()["Throwing Power"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect((Screen.width / 25) + (Screen.width / 44) + (Screen.width / 2.7f), (Screen.height / 3.7f) + (Screen.height / 2.9f), Screen.width / (3.7f / (pitcher.GetStats()["Throwing Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);
			GUI.color = Color.white;

			GUI.color = Color.red;
			if (GUI.Button(new Rect(Screen.width / 25, Screen.height - (Screen.height / 13), Screen.width / 3, Screen.height / 20), "Quit")) {

				exit_modal_open = true;

			}

			if (exit_modal_open) {

				GUI.color = Color.white;
				GUI.skin.window.fontSize = Screen.width / 100;
				modal_rect = new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2);
				modal_rect = GUI.ModalWindow (0, modal_rect, Modal, "Are you sure you wish to quit?");

			}
		}

		if (batter_mesh.GetComponent<Batter> ().Ball_through_zone && batter_mesh.GetComponent<Batter> ().Swung) {

			if (!ball.GetComponent<NewBall> ().Hit && !ball.GetComponent<NewBall> ().Hit_by_batter) {

				if (batter_mesh.GetComponent<Batter> ().Batter_late) {
					
					content = "Late";

				} else {
					
					content = "Early";
				}

			} else {

				content = batter_mesh.GetComponent<Batter> ().Hit_description;
				if (content.Length > 0) {
					content = content.Substring (0, content.Length - 1);
				}
			}

			//GUI.Box (new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2), content);
		} else {
			
			content = "";
		}

		GUI.skin.customStyles [4].fontSize = Screen.width / 70;
		GUI.color = Color.white;
		GUI.Box (new Rect (Screen.width - (Screen.width / 4), Screen.height - (Screen.height / 7), Screen.width / 5, Screen.height / 18), content, GUI.skin.customStyles[4]);
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Escape) && (ball == null || !ball.GetComponent<NewBall>().Thrown)) {

			if (!paused) {

				//set up session stat string array from session_stat IDictionary for the GUI
				UpdateSessionStats();

				paused = true;
				Time.timeScale = 0;

			} else {
				paused = false;
				Time.timeScale = 1;
			}
		}

		current_batter_anim = batter_anim.GetCurrentAnimatorClipInfo (0) [0].clip.name;
		current_pitcher_anim = pitcher_anim.GetCurrentAnimatorClipInfo (0) [0].clip.name;

		//pitcher controls
		if (reset_timer == 0 && !pitcher_mesh.GetComponent<Pitcher> ().BallInHand () && (ball == null || (!ball.GetComponent<NewBall> ().Thrown && !ball.GetComponent<NewBall>().Hit))) {
			reset_timer = Time.time;
		}

		if (mode == "2P") {
			if (Input.GetKeyDown (KeyCode.Q) && (reset_timer != 0) && (Time.time - reset_timer >= 1.15f)) {
				batter_mesh.GetComponent<Batter> ().Swung = false;
				batter_mesh.GetComponent<Batter> ().Ball_through_zone = false;
				batter_mesh.GetComponent<Batter> ().Power_meter = 0;

				reset_timer = 0;
				ball_cam.enabled = false;
				//Destroy (ball);
				ball = Instantiate (GameController.control.the_ball, pitcher_hand.transform);
				ball.AddComponent<NewBall> ();
				ball.transform.localPosition = new Vector3 (-0.01f, 0.003f, 0.062f);
				pitcher_ready = true;
			}

			if (Input.GetKeyUp (KeyCode.Q) && pitcher_ready && current_batter_anim.Contains("batting_stance")) {
				pitcher_mesh.GetComponent<Animator> ().SetTrigger ("throw");
				//session_stats ["Pitches Seen"]++;
				GameController.control.ReportStat("Pitches Seen", 1);
				pitcher_ready = false;
			}
		}

		if (mode == "1P") {

			if ((reset_timer != 0) && (Time.time - reset_timer >= 1.25f)) {
				
				batter_mesh.GetComponent<Batter> ().Swung = false;
				batter_mesh.GetComponent<Batter> ().Ball_through_zone = false;
				batter_mesh.GetComponent<Batter> ().Power_meter = 0;

				reset_timer = 0;
				ball_cam.enabled = false;
				//Destroy (ball);
				ball = Instantiate (GameController.control.the_ball, pitcher_hand.transform);
				ball.AddComponent<NewBall> ();
				ball.transform.localPosition = new Vector3 (-0.01f, 0.003f, 0.062f);
				pitcher_ready = true;
			}

			if (pitcher_ready && current_batter_anim.Contains("batting_stance")) {
				pitcher_mesh.GetComponent<Animator> ().SetTrigger ("throw");
				//session_stats ["Pitches Seen"]++;
				GameController.control.ReportStat("Pitches Seen", 1);
				pitcher_ready = false;
			}
		}

		//batter controls
		batter_speed = batter.GetStats () ["Speed"] * Time.deltaTime;
		batter_mesh.transform.position = Vector3.MoveTowards (batter_mesh.transform.position, batter_target.position, batter_speed);

		if (current_batter_anim.Contains("batting_stance")) {
			
			mid_swing = false;

		} else if (!current_batter_anim.Contains("batting_step") && !current_batter_anim.Contains("swing_followthrough")){

			if (batter_mesh.transform.position != batter_target.position) {
				batter_anim.SetBool ("run", true);

				Vector3 target_dir = batter_target.position - batter_mesh.transform.position;
				Vector3 new_dir = Vector3.RotateTowards (batter_mesh.transform.forward, target_dir, batter_speed, 0.0f);
				batter_mesh.transform.rotation = Quaternion.LookRotation (new_dir);

			} else {
				batter_anim.SetBool ("run", false);

				if (batter_mesh.transform.position == left_batters_box.transform.position) {
					Vector3 target_dir = right_batters_box.transform.position - batter_mesh.transform.position;
					Vector3 new_dir = Vector3.RotateTowards (batter_mesh.transform.forward, target_dir, batter_speed, 0.0f);
					batter_mesh.transform.rotation = Quaternion.LookRotation (new_dir);
					batter_anim.SetTrigger ("batting_stance");
				}
			}

		}

		if (current_batter_anim.Contains("batting_stance") && Input.GetKeyDown (KeyCode.A) && pressed == 0 && !ball_cam.enabled) {
			batter_anim.SetTrigger ("batting_step");
			pressed = Time.time;
		} else if (Input.GetKeyUp (KeyCode.A)) {
			if ((Time.time - pressed) > 0.15f && (Time.time - pressed) < 1.15f) { 
				batter_anim.SetTrigger ("swing");
				mid_swing = true;
				//session_stats ["Swing Total"]++;
				GameController.control.ReportStat("Swing Total", 1);
			}
			pressed = 0;
		}

		if (!mid_swing) {
			if (Input.GetKey (KeyCode.RightArrow)) {
				batter_target.position = Vector3.MoveTowards (batter_target.position, left_batters_box_inner.position, batter_speed);
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				batter_target.position = Vector3.MoveTowards (batter_target.position, left_batters_box_outer.position, batter_speed);
			}
		}

		if (ball != null) {
			if (!ball.GetComponent<NewBall> ().Thrown && ball.GetComponent<NewBall> ().Hit_by_batter) {

				RegisterHit (ball.GetComponent<NewBall>().Throw_description);
			}

		}

		//camera controls
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (pitcher_cam.enabled) {
				pitcher_cam.enabled = false;
			} else {
				pitcher_cam.enabled = true;
			}
		}

		if (batter_mesh.GetComponent<Batter> ().BallHit ()) {
			//ball_cam.transform.parent = ball.transform;
			//ball_cam.transform.position = new Vector3 (510, 136, 514);
			//ball_cam.transform.rotation = Quaternion.Euler (90, -135, 0);
			ball_cam.enabled = true;
		}
		if (ball_cam.enabled) {
			ball_cam.transform.LookAt (ball.transform.position);

			if (ball.GetComponent<NewBall> ().Hit_to_left) {
				
				ball_cam.transform.position = Vector3.MoveTowards (ball_cam.transform.position, new Vector3(left_field.transform.position.x, 200, left_field.transform.position.z), (ball.GetComponent<NewBall>().GetSpeed() / 2) * Time.deltaTime);
			} else {

				ball_cam.transform.position = Vector3.MoveTowards (ball_cam.transform.position, new Vector3(right_field.transform.position.x, 200, right_field.transform.position.z), (ball.GetComponent<NewBall>().GetSpeed() / 2) * Time.deltaTime);
			}
			//ball_cam.transform.position = Vector3.MoveTowards (ball_cam.transform.position, new Vector3(center_field.transform.position.x, 100, center_field.transform.position.z), (ball.GetComponent<NewBall>().GetSpeed() / 2) * Time.deltaTime);
		} else {
			ball_cam.gameObject.transform.position = new Vector3 (530.51f, 20.01f, 531.04f);
			ball_cam.gameObject.transform.rotation = Quaternion.Euler (11.4f, -135.58f, 0f);
		}
	}

	GameObject SpawnPlayer(Player_Character player, Vector3 location, Quaternion rotation) {

		Material[] new_mesh_mats = new Material[2];
		GameObject player_mesh = new GameObject();

		switch (player.GetRace ().GetName()) {
		case "Human":
			Destroy (player_mesh);
			player_mesh = Instantiate (GameController.control.Human [player.GetClass ().GetId()], location, rotation);
			new_mesh_mats = player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
			new_mesh_mats[0] = GameController.control.HumanSkin[player.GetSkin()];
			break;

		case "Orc":
			Destroy (player_mesh);
			player_mesh = Instantiate (GameController.control.Orc [player.GetClass ().GetId()], location, rotation);
			new_mesh_mats = player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
			//new_mesh_mats[1] = GameController.control.OrcSkin[player.GetSkin()];
			new_mesh_mats[0] = GameController.control.OrcSkin[player.GetSkin()];
			break;

		default:
			break;
		}



		switch (player.GetClass ().GetName ()) {
		case "Monk":
			new_mesh_mats [2] = GameController.control.MonkUniforms [player.GetUni ()];
			break;

		case "Rogue":
			new_mesh_mats [1] = GameController.control.RogueUniforms [player.GetUni ()];
			break;

		default:
			break;

		}
		player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = new_mesh_mats;
		player_mesh.transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [player.GetEyes()];
		player_mesh.transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [player.GetEyes()];

		return player_mesh;
	}

	private void RegisterHit(string hit_data) {

		if (hit_list.Length > 0) {
			string[] temp_list = hit_list;
			hit_list = new string[hit_list.Length + 1];
			for (int i = 0; i < temp_list.Length; i++) {
				hit_list [i] = temp_list [i];
			}
		} else {
			hit_list = new string[1];
		}

		hit_list [hit_list.Length - 1] = hit_list.Length + ". " + hit_data;
		ball.GetComponent<NewBall> ().Hit_by_batter = false;

		//register the hit session data
		//session_stats ["Hit Total"]++;
		GameController.control.ReportStat("Hit Total", 1);

		if (hit_data.Contains ("Left")) {
			//session_stats ["Hits To Left"]++;
			GameController.control.ReportStat("Hits To Left", 1);
		} else if (hit_data.Contains ("Right")) {
			//session_stats ["Hits To Right"]++;
			GameController.control.ReportStat("Hits To Right", 1);
		} else {
			//session_stats ["Hits to Center"]++;
			GameController.control.ReportStat("Hits To Center", 1);
		}
			
		char[] delims = new char[] { ' ',
									 '\n',};
		string[] hit_data_array = hit_data.Split (delims);
		//print (hit_data_array [5]);
		//print (hit_data_array [8]);
		GameController.control.ReportStat ("Distance Total", Mathf.Round(float.Parse(hit_data_array[5])));
		GameController.control.ReportStat ("Distance Avg", Mathf.Round(GameController.control.Mode_metrics["Distance Total"] / GameController.control.Mode_metrics["Hit Total"]), true);

		GameController.control.ReportStat ("Exit Velocity TOTAL", Mathf.Round(float.Parse(hit_data_array[8])));
		GameController.control.ReportStat ("Exit Velocity Avg", Mathf.Round(GameController.control.Mode_metrics["Exit Velocity TOTAL"] / GameController.control.Mode_metrics["Hit Total"]), true);
	}

	private void UpdateSessionStats() {

		//update the Swing and Miss metric
		GameController.control.ReportStat ("Swing and Miss", GameController.control.Mode_metrics ["Swing Total"] - GameController.control.Mode_metrics ["Hit Total"], true);

		IDictionary<string, float> current_metrics = GameController.control.Mode_metrics;
		string extra_content;
		int i = 0;
		//session_stat_array = new string[session_stats.Count]; 
		session_stat_array = new string[(current_metrics.Count - 2) * 2]; 
		foreach (KeyValuePair<string, float> keyValue in current_metrics) {
			if (!keyValue.Key.Contains ("TOTAL")) {
				if (keyValue.Key == "" || keyValue.Key == " ") {
					session_stat_array [i] = "";
				} else {

					if (keyValue.Key == "Home Runs" || keyValue.Key == "Hits To Left" || keyValue.Key == "Hits To Center" || keyValue.Key == "Hits To Right") {
						if (current_metrics["Hit Total"] > 0) {
							extra_content = " (" + Mathf.Round(((keyValue.Value / current_metrics ["Hit Total"]) * 100)) + "%)";
						} else {
							extra_content = " (0%)";
						}
					} else if (keyValue.Key == "Swing and Miss" || keyValue.Key == "Hit Total") {
						if (current_metrics["Swing Total"] > 0) {
							extra_content = " (" + Mathf.Round(((keyValue.Value / current_metrics ["Swing Total"]) * 100)) + "%)";
						} else {
							extra_content = " (0%)";
						}
					} else if (keyValue.Key == "Distance Avg" || keyValue.Key == "Distance Total") {
						extra_content = " feet";
					} else if (keyValue.Key == "Power Meter Avg") {
						extra_content = "%";
					} else if (keyValue.Key == "Exit Velocity Avg") {
						extra_content = " mph";
					} else {
						
						extra_content = "";
					}
						
					session_stat_array [i] = keyValue.Key;
					session_stat_array [i + 1] = keyValue.Value + extra_content;
				}
				i++;
				i++;
			}
		}
	}

	//MODALS
	void Modal (int windowID) {

		GUI.skin.textField.fontSize = Screen.width / 75;
		GUI.skin.button.fontSize = Screen.width / 100;

		switch (windowID) {

		case 0:
			GUI.color = Color.green;
			if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 9, Screen.width / 6, Screen.height / 8), "Yes")) {

				gameObject.AddComponent<PlayMenu> ();
				Destroy (batter_mesh);
				Destroy (pitcher_mesh);
				Destroy (catcher_mesh);
				Destroy (ball);
				Time.timeScale = 1;
				Destroy (this);
			}

			GUI.color = Color.red;
			if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 7f + (Screen.height / 8), Screen.width / 6, Screen.height / 8), "No")) {

				exit_modal_open = false;
			}

			//GUI.color = Color.black;
			//GUI.Label (new Rect (Screen.width / 12, Screen.height / 7f, Screen.width / 6, Screen.height / 8), "All unsaved data will be lost");
			//GUI.Label (new Rect (Screen.width / 12, Screen.height / 5.75f + (Screen.height / 8), Screen.width / 6, Screen.height / 8), "Return to Team creation");
			break;

		default:
			break;
		}
	}
}
