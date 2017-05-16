using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour {

	private IList<Player_Race> races;
	private IList<Player_Class> classes;

	private int current_race;
	private int current_class;
	private int points_spent;
	private bool average_att;
	private bool save_modal_open;
	private bool exit_modal_open;
	private IDictionary<string, int>  stat_points;

	private string char_name;
	private int char_str;
	private int char_dex;
	private int char_int;
	private int char_con;

	private int current_skin;
	private int new_skin;

	private int current_uniform;
	private int new_uniform;

	private int current_eyes;
	private int new_eyes;

	//controls
	private float mouse_scroll;

	private GameObject player_mesh;
	private Material[] player_skins;
	private Material[] player_uniforms;
	private Material[] player_mesh_mats;
	private Animator player_mesh_anim;

	private GUIContent content;

	public Texture testure;

	public GUISkin gui_skin;

	public Rect modal_rect;

	// Use this for initialization
	void Awake () {

		save_modal_open = false;
		exit_modal_open = false;

		//Initialization of lists and variables
		races = new List<Player_Race>();
		classes = new List<Player_Class>();

		current_race = 0;
		current_class = 0;

		races.Add (new Player_Race ());
		races.Add (new Player_Race ("Orc", "Orcs are the strongest of all the humanoid races. " +
										   "Their ferocity in battle transfers to impressive offensive numbers on the field. \n" +
										   "Orcs tend to excel playing at First and Third Base.", 3, 2, 1, 2));

		classes.Add (new Player_Class ());

		string[] class_pos = new string[2] {"2B", "SS"};
		classes.Add (new Player_Class("Rogue", "Rogues are quick, stealthy, and cunning. " +
											   "They dominate the basepaths by relying on their speed and misdirection techniques. " +
											   "Lightening reflexes allow the Rogue to dominate defensively and achieve high OBP.", class_pos, -2, 3, 0, -1, 1));

		Roll ();
		SpawnCharModel ();
	}
		
	void Update() {

		mouse_scroll = Input.GetAxis ("Mouse ScrollWheel");

		if (mouse_scroll > 0) {
			GameController.control.MoveCameraZ(0.15f, -1);
			GameController.control.MoveCameraY (0.015f, 1.58f);
		}

		if (mouse_scroll < 0) {
			GameController.control.MoveCameraZ (-0.15f, -3);
			GameController.control.MoveCameraY (-0.015f, 1.43f);
		}

		if (Input.GetKey("a")) {

			player_mesh.transform.Rotate (0, 10, 0);

		} else if (Input.GetKey("d")) {

			player_mesh.transform.Rotate (0, -10, 0);
		}

		if (Input.GetKeyDown ("1")) {

			player_mesh_anim.SetBool ("run", true);

		} else if (Input.GetKeyUp ("1")) {

			player_mesh_anim.SetBool ("run", false);
		}

		if (Input.GetKey ("2")) {

			player_mesh_anim.SetBool ("catch", true);


		} else if (Input.GetKeyUp ("2")) {

			player_mesh_anim.SetBool ("catch", false);
		}

		if (Input.GetKeyDown ("3")) {

			if (player_mesh_anim.GetBool ("run")) {

				player_mesh_anim.SetTrigger ("throw_upper");

			} else {
				
				player_mesh_anim.SetTrigger ("throw");
			}
		}

		if (Input.GetKeyDown ("4")) {

			player_mesh_anim.SetTrigger ("jump");
		}

		if (Input.GetKeyDown ("5")) {

			player_mesh_anim.SetTrigger ("step_swing_followthrough");
		}

		if (Input.GetKeyDown ("6")) {

			player_mesh_anim.SetBool ("pose", true);

		} else if (Input.GetKeyUp ("6")) {

			player_mesh_anim.SetBool ("pose", false);
		}
	}

	void OnGUI() {

		//set up GUI Styles
		GUI.skin = GameController.control.gui_skins[0];
		//GUIStyle center_text = new GUIStyle ("box");
		//center_text.wordWrap = true;
		//center_text.alignment = TextAnchor.MiddleCenter;
		GUI.skin.box.fontSize = Screen.width / 120;
		GUI.skin.button.fontSize = Screen.width / 120;
		//center_text.padding = new RectOffset (7, 7, 7, 7);
		GUI.skin.button.fontStyle = FontStyle.Bold;

		if (races != null) {

			GUI.Box (new Rect (Screen.width / 2 - (Screen.width / 3.9f), -Screen.height / 300, Screen.width / 1.95f, Screen.height / 6.25f), "");

			if (races.Count > 0) {

				GUI.color = new Color(0.5f, 0.5f, 1.0f);
				GUI.Box (new Rect ((Screen.width / 2) - (Screen.width / 50), Screen.height / 85, Screen.width / 25, Screen.height / 18), races [current_race].GetName());
				if (races.Count > 1) {

					content = new GUIContent ("<", testure, "A Player's Race governs how many dice (d6) are Rolled for each of that Player's Attributes. The Average Starting Attributes for each Race (selected by default) reflects a Roll of 3 on each die.");
					if (GUI.Button (new Rect ((Screen.width / 2) - (Screen.width / 24), Screen.height / 85, Screen.width / 50, Screen.height / 18), content)) {
						current_race--;
						if (current_race < 0) {
							current_race = races.Count - 1;
						}
						Roll ();
						SpawnCharModel ();
					}
					content = new GUIContent (">", testure, "A Player's Race governs how many dice (d6) are Rolled for each of that Player's Attributes. The Average Starting Attributes for each Race (selected by default) reflects a Roll of 3 on each die.");
					if (GUI.Button (new Rect ((Screen.width / 2) + (Screen.width / 45), Screen.height / 85, Screen.width / 50, Screen.height / 18), content)) {
						current_race++;
						if (current_race == races.Count) {
							current_race = 0;
						}
						Roll ();
						SpawnCharModel ();
					}
				}

				GUI.Box (new Rect (Screen.width - Screen.width / 3.1f, Screen.height / 6.05f, Screen.width / 5.75f, Screen.height / 2.95f), "");

				content = new GUIContent("Base Attributes", testure, "A player's Race determines how many dice (d6) are rolled for each Base Attribute. You may take the Average (3 for each die) or try to Reroll for better Attributes.");
				GUI.Box (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 5.75f, Screen.width / 15, Screen.height / 18), content);

				content = new GUIContent("Str", testure, "Strength is a measure of raw power and governs a player's Hitting and Throwing Power.");
				GUI.Box (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 4.3f, Screen.width / 30f, Screen.height / 25), content);

				content = new GUIContent ("Dex", testure, "Dexterity is possessed by the quick and agile and governs a player's Speed and Accuracy.");
				GUI.Box (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 3.63f, Screen.width / 30f, Screen.height / 25), content);

				content = new GUIContent ("Int", testure, "Intelligence is sharpness of the mind and governs a player's Points Per Level and Magical Ability.");
				GUI.Box (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 3.15f, Screen.width / 30f, Screen.height / 25), content);

				content = new GUIContent ("Con", testure, "Constitution is toughness and fortitude and governs a player's Health and Stamina.");
				GUI.Box (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 2.77f, Screen.width / 30f, Screen.height / 25), content);

				//GUI.Label (new Rect (Screen.width - Screen.width / 3.2f, Screen.height / 6, Screen.width / 15, Screen.height / 18), "Base Attributes", center_text);
				//GUI.Label (new Rect (Screen.width - Screen.width / 3.375f, Screen.height / 4.375f, Screen.width / 25, Screen.height / 25), "Str " + races[current_race].GetAtt("Str"), center_text);
				//GUI.Label (new Rect (Screen.width - Screen.width / 3.375f, Screen.height / 3.625f, Screen.width / 25, Screen.height / 25), "Dex " + races[current_race].GetAtt("Dex"), center_text);
				//GUI.Label (new Rect (Screen.width - Screen.width / 3.375f, Screen.height / 3.09f, Screen.width / 25, Screen.height / 25), "Int " + races[current_race].GetAtt("Int"), center_text);
				//GUI.Label (new Rect (Screen.width - Screen.width / 3.375f, Screen.height / 2.70f, Screen.width / 25, Screen.height / 25), "Con " + races[current_race].GetAtt("Con"), center_text);
				if (average_att) {
					GUI.color = Color.white;
				} else {
					GUI.color = new Color (1f, 0.35f, 0.11f);
				}

				int att = char_str;
				GUI.Box (new Rect (Screen.width - Screen.width / 3.60f, Screen.height / 4.3f, Screen.width / 30.5f, Screen.height / 25), "" + att + "");

				att = char_dex;
				GUI.Box (new Rect (Screen.width - Screen.width / 3.60f, Screen.height / 3.63f, Screen.width / 30.5f, Screen.height / 25), "" + att + "");

				att = char_int;
				GUI.Box (new Rect (Screen.width - Screen.width / 3.60f, Screen.height / 3.15f, Screen.width / 30.5f, Screen.height / 25), "" + att + "");

				att = char_con;
				GUI.Box (new Rect (Screen.width - Screen.width / 3.60f, Screen.height / 2.77f, Screen.width / 30.5f, Screen.height / 25), "" + att + "");

				GUI.color = new Color (1f, 0.35f, 0.11f);
				content = new GUIContent ("Reroll", testure, "Roll a number of dice (d6) for each Attribute to determine the Player's value. Number of dice to be rolled for each Attribute is governed by Race.");
				if (GUI.Button (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 2.45f, Screen.width / 6.5f, Screen.height / 25), content)) {
					Roll (false);
				}

				GUI.color = Color.white;
				content = new GUIContent ("Average", testure, "Instead of risking a roll of the dice, take an average score for each Attribute as determined by the Player's Race. Reflects a Roll of 3 on each die.");
				if (GUI.Button (new Rect (Screen.width - Screen.width / 3.21f, Screen.height / 2.2f, Screen.width / 6.5f, Screen.height / 25), content)) {
					Roll ();
				}

				GUI.skin.box.fontSize = Screen.width / 125;
				GUI.skin.button.fontSize = Screen.width / 125;
				GUI.skin.label.fontSize = Screen.width / 125;

				GUI.Box (new Rect (Screen.width / 2 - (Screen.width / 4), Screen.height / 14, Screen.width / 2, Screen.height / 13.5f), races [current_race].GetDescription ());

				//zoom, rotate, and animations tip
				GUI.color = Color.black;
				GUI.Label (new Rect (Screen.width / 2 - (Screen.width / 6), Screen.height / 7, Screen.width / 3, Screen.height / 20), "Scroll to zoom and use [a] - [d] to rotate. View Animations with [1], [2]. [3], [4], [5], and [6]");

			} else {

				GUI.Box (new Rect ((Screen.width / 2) - (Screen.width / 22), Screen.height / 85, Screen.width / 11, Screen.height / 18), "No Races Loaded");
			}
		}
			
		GUI.skin.box.fontSize = Screen.width / 120;
		GUI.skin.button.fontSize = Screen.width / 120;

		if (classes != null) {

			GUI.Box (new Rect (Screen.width / 2 - (Screen.width / 3.9f), Screen.height - (Screen.height / 4f), Screen.width / 1.95f, Screen.height / 3.75f), "");

			if (classes.Count > 0) {

				GUI.color = new Color (0.75f, 0.25f, 1f);
				GUI.Box (new Rect ((Screen.width / 2) - (Screen.width / 50), Screen.height - (Screen.height / 9), Screen.width / 25, Screen.height / 18), classes[current_class].GetName());

				if (classes.Count > 1) {
					content = new GUIContent ("<", testure, "A Player's Class governs Bonuses to that Player's Starting Attributes. Class also affects how quickly a Player Levels Up and which Skills and Equipment that Player will gain access to.");
					if (GUI.Button (new Rect ((Screen.width / 2) - (Screen.width / 24), Screen.height - (Screen.height / 9), Screen.width / 50, Screen.height / 18), content)) {
						current_class--;
						if (current_class < 0) {
							current_class = classes.Count - 1;
						}
						SpawnCharModel ();
					}
					content = new GUIContent (">", testure, "A Player's Class governs Bonuses to that Player's Starting Attributes. Class also affects how quickly a Player Levels Up and which Skills and Equipment that Player will gain access to.");
					if (GUI.Button (new Rect ((Screen.width / 2) + (Screen.width / 45), Screen.height - (Screen.height / 9), Screen.width / 50, Screen.height / 18), content)) {
						current_class++;
						if (current_class == classes.Count) {
							current_class = 0;
						}
						SpawnCharModel ();
					}
				}

				content = new GUIContent("Class Bonus", testure, "Class Bonuses derive from a Player's Class and can be positive or negative.");
				GUI.Box (new Rect (Screen.width - Screen.width / 4.15f, Screen.height / 5.75f, Screen.width / 25, Screen.height / 18), content);

				int att = classes [current_class].GetAtt ("Str");
				string str_att = "" + att + "";
				if (att < 0) {
					GUI.color = Color.red;
				} else if (att > 0) {
					GUI.color = Color.green;
					str_att = "+" + str_att;
				} else {
					GUI.color = Color.white;
				}
				GUI.Box (new Rect (Screen.width - Screen.width / 4.15f, Screen.height / 4.3f, Screen.width / 25f, Screen.height / 25), str_att);

				att = classes [current_class].GetAtt ("Dex");
				str_att = "" + att + "";
				if (att < 0) {
					GUI.color = Color.red;
				} else if (att > 0) {
					GUI.color = Color.green;
					str_att = "+" + str_att;
				} else {
					GUI.color = Color.white;
				}
				GUI.Box (new Rect (Screen.width - Screen.width / 4.15f, Screen.height / 3.63f, Screen.width / 25f, Screen.height / 25), str_att);

				att = classes [current_class].GetAtt ("Int");
				str_att = "" + att + "";
				if (att < 0) {
					GUI.color = Color.red;
				} else if (att > 0) {
					GUI.color = Color.green;
					str_att = "+" + str_att;
				} else {
					GUI.color = Color.white;
				}
				GUI.Box (new Rect (Screen.width - Screen.width / 4.15f, Screen.height / 3.15f, Screen.width / 25f, Screen.height / 25), str_att);

				att = classes [current_class].GetAtt ("Con");
				str_att = "" + att + "";
				if (att < 0) {
					GUI.color = Color.red;
				} else if (att > 0) {
					GUI.color = Color.green;
					str_att = "+" + str_att;
				} else {
					GUI.color = Color.white;
				}
				GUI.Box (new Rect (Screen.width - Screen.width / 4.15f, Screen.height / 2.77f, Screen.width / 25f, Screen.height / 25), str_att);

				GUI.color = Color.white;

				GUI.skin.box.fontSize = Screen.width / 125;
				GUI.skin.button.fontSize = Screen.width / 125;

				GUI.Box (new Rect (Screen.width / 2 - (Screen.width / 4), Screen.height - (Screen.height / 4.25f), Screen.width / 2, Screen.height / 13.5f), classes [current_class].GetDescription ());

				string pref_pos = "";
				foreach (string position in classes[current_class].GetPrefPos()) {

					pref_pos += position + ", ";
				}
				pref_pos = pref_pos.Remove (pref_pos.Length - 2);

				GUI.Box (new Rect ((Screen.width / 2) - (Screen.width / 16), Screen.height - (Screen.height / 6.42f), Screen.width / 8, Screen.height / 25), "Preferred Positions: " + pref_pos);
			} else {
				
				GUI.Box (new Rect ((Screen.width / 2) - (Screen.width / 22), Screen.height - (Screen.height / 9), Screen.width / 11, Screen.height / 18), "No Classes Loaded");
			}
		}

		if (races != null && races.Count > 0 && classes != null && classes.Count > 0) {

			GUI.Box (new Rect (Screen.width - Screen.width / 5.05f, Screen.height / 5.75f, Screen.width / 25, Screen.height / 18), "Total");

			int att_total = char_str + classes [current_class].GetAtt ("Str");
			GUI.Box (new Rect (Screen.width - Screen.width / 5.05f, Screen.height / 4.3f, Screen.width / 25f, Screen.height / 25), "" + att_total + "");

			att_total = char_dex + classes [current_class].GetAtt ("Dex");
			GUI.Box (new Rect (Screen.width - Screen.width / 5.05f, Screen.height / 3.63f, Screen.width / 25f, Screen.height / 25), "" + att_total + "");

			att_total = char_int + classes [current_class].GetAtt ("Int");
			GUI.Box (new Rect (Screen.width - Screen.width / 5.05f, Screen.height / 3.15f, Screen.width / 25f, Screen.height / 25), "" + att_total + "");

			att_total = char_con + classes [current_class].GetAtt ("Con");
			GUI.Box (new Rect (Screen.width - Screen.width / 5.05f, Screen.height / 2.77f, Screen.width / 25f, Screen.height / 25), "" + att_total + "");
		
			GUI.Box (new Rect (Screen.width / 5.34f, Screen.height / 6.05f, Screen.width / 8, Screen.height / 1.75f), "");

			GUI.color = Color.cyan;
			content = new GUIContent("Player Stats", testure, "A Player's Stats dictate how well that player performs on the field. Each Stat is governed by an Attribute and is further increased when the player Levels Up and allocates Points Per Level.");
			GUI.Box (new Rect (Screen.width / 5f, Screen.height / 5.75f, Screen.width / 10, Screen.height / 25), content);

			if (points_spent > 0) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.grey;
			}

			content = new GUIContent ("Clear Points", testure, "Clear all allocated Points and start over.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 1.8f, Screen.width / 10, Screen.height / 12.5f), content)) {
				ClearPointsSpent ();
			}

			int points_total = (char_int + classes [current_class].GetAtt ("Int"));
			GUI.color = Color.white;
			content = new GUIContent("Points Per Level " + (points_total - points_spent) + " / " + points_total, testure, "For each Level a player gains (including the first) you may distribute Points Per Level to any of that player's Stats. A player's Points Per Level is equal to that player's Intelligence.");
			GUI.Box (new Rect (Screen.width / 5f, Screen.height / 1.5f, Screen.width / 10, Screen.height / 25), content);

			int stat_total = ((char_str + classes[current_class].GetAtt("Str")) * 5) + stat_points["Hitting Power"];
			if (stat_points ["Hitting Power"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Hitting Power " + stat_total, testure, "Hitting Power measures a player's ability to hit the long ball. A player with high hitting power will hit more home runs. Governed by Strength.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 23f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Hitting Power"]++;
			}

			stat_total = ((((char_str + classes[current_class].GetAtt("Str")) * 5) + ((char_dex + classes[current_class].GetAtt("Dex")) * 5)) / 2) + stat_points["Throwing Power"];
			if (stat_points ["Throwing Power"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Throwing Power " + stat_total, testure, "Throwing Power describes how hard the player can throw. Throw the ball faster and farther. Governed by Strength and Dexterity.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 11.35f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Throwing Power"]++;
			}

			stat_total = ((char_dex + classes[current_class].GetAtt("Dex")) * 5) + stat_points["Speed"];
			if (stat_points ["Speed"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Speed " + stat_total, testure, "Speed reflects how quickly a player can move. Stealing bases and running down fly balls is easier with high Speed. Governed by Dexterity.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 7.55f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Speed"]++;
			}

			stat_total = ((((char_dex + classes[current_class].GetAtt("Dex")) * 5) + ((char_int + classes[current_class].GetAtt("Int")) * 5)) / 2) + stat_points["Accuracy"];
			if (stat_points ["Accuracy"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Accuracy " + stat_total, testure, "Accuracy applies to Throwing and Batting. High Accuracy allows hitters to cover more of the plate and pithers to paint the corners. Governed by Dexterity and Intelligence.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 5.7f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Accuracy"]++;
			}

			stat_total = ((char_int + classes[current_class].GetAtt("Int")) * 5) + stat_points["Magic"];
			if (stat_points ["Magic"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Magic " + stat_total, testure, "Magic ability dictates how effective a player's spells are. Especially important for Wizards. Goverened by Intelligence.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 4.55f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Magic"]++;
			}

			stat_total = ((char_con + classes[current_class].GetAtt("Con")) * 10) + stat_points["Health"];
			if (stat_points ["Health"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Health " + stat_total, testure, "Health determines how much damage a player can take before being removed from the game. Healthier players can stay on the field longer. Governed by Constitution.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 3.79f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Health"]++;
			}

			stat_total = ((char_con + classes[current_class].GetAtt("Con")) * 10) + stat_points["Stamina"];
			if (stat_points ["Stamina"] > 0) {
				GUI.color = Color.yellow;
			} else {
				GUI.color = Color.white;
			}
			content = new GUIContent("Stamina " + stat_total, testure, "Stamina allows a player to perform enhanced actions. A player with high Stamina can Sprint, Jump, Dive, and perform Power Swings more often. Governed by Constitution.");
			if (GUI.Button (new Rect (Screen.width / 5f, Screen.height / 5.75f + Screen.height / 3.25f, Screen.width / 10, Screen.height / 25), content) && (points_spent < points_total)) {
				points_spent++;
				stat_points ["Stamina"]++;
			}

			GUI.color = Color.white;
			GUI.Box (new Rect (Screen.width - Screen.width / 3.1f, Screen.height / 1.95f, Screen.width / 5.75f, Screen.height / 11.5f), "");

			string[] btn_strings = new string[player_skins.Length];
			for (int i = 0; i < player_skins.Length; i++) {
				btn_strings[i] = "Skin" + (i + 1);
			}

			new_skin = GUI.SelectionGrid (new Rect (Screen.width - Screen.width / 3.14f, Screen.height / 1.93f, Screen.width / 6f, Screen.height / 42f), new_skin, btn_strings, player_skins.Length);

			if (new_skin != current_skin) {

				ChangeSkinTone (new_skin);
			}

			btn_strings = new string[GameController.control.Eyes.Length];
			for (int i = 0; i < GameController.control.Eyes.Length; i++) {
				btn_strings[i] = "Eyes" + (i + 1);
			}
			new_eyes = GUI.SelectionGrid (new Rect (Screen.width - Screen.width / 3.14f, Screen.height / 1.84f, Screen.width / 6f, Screen.height / 42f), new_eyes, btn_strings, GameController.control.Eyes.Length);

			if (new_eyes != current_eyes) {

				ChangeEyeColor (new_eyes);
			}

			btn_strings = new string[player_uniforms.Length];
			for (int i = 0; i < player_uniforms.Length; i++) {
				btn_strings[i] = "Uni" + (i + 1);
			}
			new_uniform = GUI.SelectionGrid (new Rect (Screen.width - Screen.width / 3.14f, Screen.height / 1.757f, Screen.width / 6f, Screen.height / 42f), new_uniform, btn_strings, player_uniforms.Length);

			if (new_uniform != current_uniform) {

				ChangeUniformColor (new_uniform);
			}

			content = new GUIContent ("", testure, "Choose the Player's Skin Tone, Eye Color, and Default Uniform Color. Note that when on a Team, that Team's Uniform Color overrides a Player's Default Uniform Color.");
			GUI.Label (new Rect (Screen.width - Screen.width / 3.1f, Screen.height / 1.95f, Screen.width / 5.75f, Screen.height / 11.5f), content);

			GUI.Box (new Rect (Screen.width - Screen.width / 3.1f, Screen.height / 1.575f, Screen.width / 5.75f, Screen.height / 11.5f), "");

			if (points_spent < points_total) {
				
				GUI.color = Color.grey;
				content = new GUIContent ("Spend Points Per Level to Continue", testure, "You must allocate all of the Player's Statistic Points Per Level before Saving\n(left-hand side panel)");
				GUI.Button (new Rect (Screen.width - Screen.width / 3.13f, Screen.height / 1.565f, Screen.width / 12f, Screen.height / 13f), content);

			} else {
				
				GUI.color = Color.green;
				content = new GUIContent ("Save", testure, "Save and add this Player to the active roster. Players on the active roster can be added to Teams and can play in ballgames.");
				if (GUI.Button (new Rect (Screen.width - Screen.width / 3.13f, Screen.height / 1.565f, Screen.width / 12f, Screen.height / 13f), content)) {

					save_modal_open = true;
				}
					
			}

			GUI.color = Color.red;
			content = new GUIContent ("Exit", testure, "Exit the Character Creator and lose all unsaved progress.");
			if (GUI.Button (new Rect (Screen.width - Screen.width / 3.13f + (Screen.width / 11.75f), Screen.height / 1.565f, Screen.width / 12f, Screen.height / 13f), content)) {

				exit_modal_open = true;
			}

			if (exit_modal_open) {

				GUI.color = Color.white;
				GUI.skin.window.fontSize = Screen.width / 100;
				modal_rect = new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2);
				modal_rect = GUI.ModalWindow (0, modal_rect, Modal, "Are you sure you wish to exit?");

			} else if (save_modal_open) {

				GUI.color = Color.white;
				GUI.skin.window.fontSize = Screen.width / 100;
				modal_rect = new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2);
				modal_rect = GUI.ModalWindow (1, modal_rect, Modal, "Enter a Name to Save your Player:");
			}
		}

		GUI.skin.box.fontSize = Screen.width / 125;
		GUI.skin.button.fontSize = Screen.width / 125;

		//Set up the tooltip
		GUI.color = Color.yellow;
		GUI.Box (new Rect (Screen.width - Screen.width / 4.5f, Screen.height / 50, Screen.width / 5.25f, Screen.height / 10), GUI.tooltip);
	}
	
	private void ClearPointsSpent() {
		points_spent = 0;
		stat_points = new Dictionary<string, int> {

			{"Hitting Power", 0},
			{"Throwing Power", 0},
			{"Speed", 0},
			{"Accuracy", 0},
			{"Magic", 0},
			{"Health", 0},
			{"Stamina", 0},
		};
	}

	private void Roll (bool average = true) {

		if (average) {

			char_str = (races [current_race].GetAtt ("Str") * 3);
			char_dex = (races[current_race].GetAtt ("Dex") * 3);
			char_int = (races[current_race].GetAtt ("Int") * 3);
			char_con = (races [current_race].GetAtt ("Con") * 3);

			average_att = true;

		} else {

			char_str = 0;
			for (int i = races [current_race].GetAtt ("Str"); i > 0; i--) {
				char_str = char_str + Random.Range (1, 6);
			}
			char_dex = 0;
			for (int i = races [current_race].GetAtt ("Dex"); i > 0; i--) {
				char_dex = char_dex + Random.Range (1, 6);
			}
			char_int = 0;
			for (int i = races [current_race].GetAtt ("Int"); i > 0; i--) {
				char_int = char_int + Random.Range (1, 6);
			}
			char_con = 0;
			for (int i = races [current_race].GetAtt ("Con"); i > 0; i--) {
				char_con = char_con + Random.Range (1, 6);
			}

			average_att = false;
		}

		ClearPointsSpent ();
	}

	private void SpawnCharModel() {

		if (player_mesh != null) {
			Destroy (player_mesh);
		}
			
		switch (races [current_race].GetName ()) {

		case "Human":
			player_mesh = Instantiate (GameController.control.Human [current_class], Vector3.zero, new Quaternion (0, 180, 0, 0));
			player_skins = GameController.control.HumanSkin;
			break;

		case "Orc":
			player_mesh = Instantiate (GameController.control.Orc [current_class], Vector3.zero, new Quaternion (0, 180, 0, 0));
			player_skins = GameController.control.OrcSkin;
			break;

		default:
			break;
		}

		switch (classes [current_class].GetName ()) {

		case "Monk":
			player_uniforms = GameController.control.MonkUniforms;
			break;

		case "Rogue":
			player_uniforms = GameController.control.RogueUniforms;
			break;
		
		default:
			break;

		}

		char_name = "";
		current_skin = 0;
		new_skin = 0;
		current_uniform = 0;
		new_uniform = 0;
		current_eyes = 0;
		new_eyes = 0;
		player_mesh_anim = player_mesh.GetComponent<Animator> ();
		player_mesh_mats = player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
		ClearPointsSpent ();
	}

	private void ChangeSkinTone(int index) {

		player_mesh_mats [0] = player_skins[index];

		player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = player_mesh_mats;

		current_skin = index;

	}

	private void ChangeUniformColor(int index) {

		player_mesh_mats [player_mesh_mats.Length - 1] = player_uniforms[index];

		player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = player_mesh_mats;

		current_uniform = index;

	}

	private void ChangeEyeColor(int index) {

		player_mesh.transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [index];
		player_mesh.transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [index];

		current_eyes = index;
	}

	public IList<Player_Race> GetRaces() {

		return races;
	}

	public IList<Player_Class> GetClasses() {

		return classes;
	}

	//MODALS
	void Modal (int windowID) {

		GUI.skin.textField.fontSize = Screen.width / 75;
		GUI.skin.button.fontSize = Screen.width / 100;

		switch (windowID) {

		case 0:
			GUI.color = Color.green;
			if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 9, Screen.width / 6, Screen.height / 8), "Yes")) {

				Destroy (player_mesh);
				gameObject.AddComponent<CreationMenu> ();
				Destroy (this);
			}

			GUI.color = Color.red;
			if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 7f + (Screen.height / 8), Screen.width / 6, Screen.height / 8), "No")) {

				exit_modal_open = false;
			}

			GUI.color = Color.black;
			GUI.Label (new Rect (Screen.width / 12, Screen.height / 7f, Screen.width / 6, Screen.height / 8), "All unsaved data will be lost");
			GUI.Label (new Rect (Screen.width / 12, Screen.height / 5.75f + (Screen.height / 8), Screen.width / 6, Screen.height / 8), "Return to Character creation");
			break;

		case 1:

			GUI.SetNextControlName ("NameField");
			char_name = GUI.TextField (new Rect (Screen.width / 15, Screen.height / 12, Screen.width / 5, Screen.height / 15), char_name, 30);
			if (char_name == "") {
				
				GUI.color = Color.grey;
				GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "Enter a Name to Continue");

				GUI.FocusControl ("NameField");

			} else {

				bool name_taken = false;

				if (GameController.control.GetPlayers () != null) {

					foreach (Player_Character player in GameController.control.GetPlayers()) {

						if (char_name == player.GetName ()) {

							name_taken = true;

						} 
					}
				}

				if (!name_taken) {
					
					GUI.color = Color.green;
					if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "Save Character")) {

						IDictionary<string, int> final_atts = new Dictionary<string, int> {

							{ "Str", char_str + classes [current_class].GetAtt ("Str") },
							{ "Dex", char_dex + classes [current_class].GetAtt ("Dex") },
							{ "Int", char_int + classes [current_class].GetAtt ("Int") },
							{ "Con", char_con + classes [current_class].GetAtt ("Con") },
						};

						IDictionary <string, int> final_stats = new Dictionary<string, int> {

							{"Hitting Power", ((char_str + classes[current_class].GetAtt("Str")) * 5) + stat_points["Hitting Power"]},
							{"Throwing Power", ((char_str + classes[current_class].GetAtt("Str")) * 5) + stat_points["Throwing Power"]},
							{"Speed", ((char_dex + classes[current_class].GetAtt("Dex")) * 5) + stat_points["Speed"]},
							{"Accuracy", ((char_dex + classes[current_class].GetAtt("Dex")) * 5) + stat_points["Accuracy"]},
							{"Magic", ((char_int + classes[current_class].GetAtt("Int")) * 5) + stat_points["Magic"]},
							{"Health", ((char_con + classes[current_class].GetAtt("Con")) * 10) + stat_points["Health"]},
							{"Stamina", ((char_con + classes[current_class].GetAtt("Con")) * 10) + stat_points["Stamina"]},
						};

						GameController.control.SavePlayer (new Player_Character (char_name, final_atts, final_stats, races [current_race], classes [current_class], current_skin, current_eyes, current_uniform));
						current_race = 0;
						current_class = 0;
						SpawnCharModel ();
						Roll ();
						save_modal_open = false;
					}

				} else {

					GUI.color = Color.grey;
					GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "That Name is Already Taken");
				}
			}

			GUI.color = Color.red;
			if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 7f + (Screen.height / 5), Screen.width / 6, Screen.height / 8), "Cancel")) {

				save_modal_open = false;
			}
			break;

		default:
			break;
		}
	}
}
