using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour {

	private GameObject ballfield;
	private GameObject camera;

	private string button_active;

	private GUIContent content;

	private Player_Character[] active_roster;
	private string[] player_list;
	private Vector2 batter_scroll_pos;
	private Vector2 pitcher_scroll_pos;

	private int selected_batter;
	private int last_batter;
	private int selected_pitcher;
	private int last_pitcher;

	private GameObject batter_mesh;
	private IDictionary<string, int> current_batter_stats;
	private string[] stat_text;
	private GameObject pitcher_mesh;
	private int current_pitcher_throwing_power;

	private bool spin_field;

	// Use this for initialization
	void Awake () {

		GameObject.Find ("Pitcher_Camera").GetComponent<Camera> ().enabled = false;
		GameObject.Find ("Ball_Camera").GetComponent<Camera> ().enabled = false;

		spin_field = true;

		button_active = "";

		ballfield = GameObject.Find ("ballfield_light");

		active_roster = GameController.control.GetPlayers ();

		if (active_roster != null) {

			Player_Character current_player;

			player_list = new string[active_roster.Length];
			for (int i = 0; i < active_roster.Length; i++) {

				current_player = active_roster[i];
				player_list [i] = current_player.GetName () + " (" + current_player.GetRace ().GetName () + " " + current_player.GetClass ().GetName () + " " + current_player.GetLevel() + ")";
			}

		} else {

			player_list = new string[1] {"No Active Players"};
		}

		camera = this.gameObject;
		camera.GetComponent<Camera> ().farClipPlane = 2000;
		camera.transform.position = new Vector3 (634, 330, 613);
		camera.transform.rotation = Quaternion.Euler (42.7f, -135.41f, 0);

		last_batter = 0;
		//selected_player = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if (spin_field) {
			ballfield.transform.RotateAround (Vector3.zero, Vector3.up, 10 * Time.deltaTime);
		} else {
			ballfield.transform.rotation = Quaternion.Euler (0, 0, 0);
		}

		if (button_active == "practice") {
			if (last_batter != selected_batter) {
				last_batter = selected_batter;
				UpdateCharMesh ("batter");
			}
			if (last_pitcher != selected_pitcher) {
				last_pitcher = selected_pitcher;
				UpdateCharMesh ("pitcher");
			}
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
		GUI.skin.label.fontSize = Screen.width / 120;
		//center_text.padding = new RectOffset (7, 7, 7, 7);
		GUI.skin.button.fontStyle = FontStyle.Bold;

		GUI.Label (new Rect (Screen.width / 16, 0 - (Screen.height / 30), Screen.width / 1.15f, Screen.height / 10), "", GUI.skin.button);

		if (button_active == "exhib") {
			GUI.contentColor = new Color (167f / 255f, 255 / 255f, 0);
		} else {
			GUI.contentColor = Color.white;
		}

		if (GUI.Button (new Rect (Screen.width / 11, Screen.height / 50, Screen.width / 10, Screen.height / 30), "Exhibition")) {

			if (batter_mesh != null) {
				Destroy (batter_mesh);
			}

			if (pitcher_mesh != null) {
				Destroy (pitcher_mesh);
			}

			if (button_active == "exhib") {
				button_active = "";
			} else {
				button_active = "exhib";
			}
		}

		if (button_active == "derby") {
			GUI.contentColor = new Color (167f / 255f, 255 / 255f, 0);
		} else {
			GUI.contentColor = Color.white;
		}
		if (GUI.Button (new Rect (Screen.width / 5, Screen.height / 50, Screen.width / 10, Screen.height / 30), "Home Run Derby")) {

			if (batter_mesh != null) {
				Destroy (batter_mesh);
			}

			if (pitcher_mesh != null) {
				Destroy (pitcher_mesh);
			}

			if (button_active == "derby") {
				button_active = "";
			} else {
				button_active = "derby";
			}
		}

		if (button_active == "practice") {
			GUI.contentColor = new Color (167f / 255f, 255 / 255f, 0);
		} else {
			GUI.contentColor = Color.white;
		}
		if (GUI.Button (new Rect (Screen.width / 3.225f, Screen.height / 50, Screen.width / 10, Screen.height / 30), "Batting Practice") && active_roster != null) {

			if (batter_mesh != null) {
				Destroy (batter_mesh);
			}

			if (pitcher_mesh != null) {
				Destroy (pitcher_mesh);
			}

			if (button_active == "practice") {
				button_active = "";
			} else {
				button_active = "practice";
				UpdateCharMesh ("batter");
				UpdateCharMesh ("pitcher");
			}
		}

		GUI.color = Color.red;
		GUI.contentColor = Color.white;
		if (GUI.Button (new Rect (Screen.width - (Screen.width / 5), Screen.height / 50, Screen.width / 10, Screen.height / 30), "Exit")) {

			SceneManager.LoadSceneAsync (0);
			camera.transform.position = new Vector3 (0, 1.43f, -3);
			camera.transform.rotation = Quaternion.Euler (10, 0, 0);
			Destroy (this);
		}

		GUI.color = Color.white;
		if (button_active == "practice") {

			GUI.skin.window.fontSize = Screen.width / 50;
			content = new GUIContent ("Choose a Batter", "This panel shows all Players on the current Active Roster.");
			GUI.Box (new Rect(Screen.width / 25, Screen.height / 10, Screen.width / 4, Screen.height - (Screen.height / 8f)), content, GUI.skin.window);

			batter_scroll_pos = GUI.BeginScrollView(new Rect(Screen.width / 19, Screen.height / 5.5f, Screen.width / 4.25f, Screen.height - (Screen.height / 4)), batter_scroll_pos, 
				new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 4.5f, (Screen.height / (20 / player_list.Length))));
			selected_batter = GUI.SelectionGrid (new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 4.25f, (Screen.height / (20 / player_list.Length))), selected_batter, player_list, 1);
			GUI.EndScrollView ();


			content = new GUIContent ("", "This panel displays a summary of the selected player's ability.");
			GUI.Box (new Rect(Screen.width / 25 + (Screen.width / 4), Screen.height / 10, Screen.width / 4, Screen.height / 3.5f), content, GUI.skin.window);

			//attempt at stat bars
			if (active_roster != null) {

				//hitting power
				if (( current_batter_stats["Hitting Power"] / 100f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Hitting Power"] / 100f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 9.5f, Screen.width / (4.08f / (current_batter_stats["Hitting Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//throwing power
				if ((current_batter_stats ["Throwing Power"] / 100f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Throwing Power"] / 100f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 6.9f, Screen.width / (4.08f / (current_batter_stats["Throwing Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//speed
				if ((current_batter_stats ["Speed"] / 100f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Speed"] / 100f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 5.4f, Screen.width / (4.08f / (current_batter_stats["Speed"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//accuracy
				if ((current_batter_stats ["Accuracy"] / 100f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Accuracy"] / 100f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 4.43f, Screen.width / (4.08f / (current_batter_stats["Accuracy"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//magic
				if ((current_batter_stats ["Magic"] / 100f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Magic"] / 100f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 3.765f, Screen.width / (4.08f / (current_batter_stats["Magic"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//health
				if ((current_batter_stats ["Health"] / 200f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Health"] / 200f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 3.271f, Screen.width / (4.08f / (current_batter_stats["Health"] / 200f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

				//stamina
				if ((current_batter_stats ["Stamina"] / 200f) > 0.79f) {
					GUI.color = Color.green;
				} else if ((current_batter_stats ["Stamina"] / 200f) < 0.40f) {
					GUI.color = Color.red;
				} else {
					GUI.color = Color.yellow;
				}
				GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 2.891f, Screen.width / (4.08f / (current_batter_stats["Stamina"] / 200f)), Screen.height / 29), "", GUI.skin.customStyles[0]);
			}

			GUI.color = Color.white;
			//selected character stats go here
			GUI.SelectionGrid(new Rect(Screen.width / 23.75f + (Screen.width / 4), Screen.height / 9.5f, Screen.width / 4.08f, Screen.height / 3.65f), 0, stat_text, 1, GUI.skin.box);

			//play ball button
			GUI.skin.customStyles [2].fontSize = Screen.width / 50;
			GUI.color = Color.green;
			if (GUI.Button (new Rect (Screen.width / 23.75f + (Screen.width / 2), Screen.height / 9.5f, Screen.width / 5.35f, Screen.height / 3.65f), "Play Ball!", GUI.skin.customStyles [2])) {

				spin_field = false;
				ballfield.transform.rotation = Quaternion.Euler (0, 0, 0);
				//set the batter and pitcher for the batting practice (Player IDs are saved in the Game Controller)
				GameController.control.SetBatterPitcher (GameController.control.GetPlayer(selected_batter).GetId(), GameController.control.GetPlayer(selected_pitcher).GetId());

				//add the new component, batting practice, and destroy this instance
				gameObject.AddComponent<BattingPractice>();
				Destroy (batter_mesh);
				Destroy (pitcher_mesh);
				Destroy (this);
			}

			GUI.color = Color.white;
			GUI.skin.window.fontSize = Screen.width / 50;
			content = new GUIContent ("Choose a Pitcher", "This panel shows all Players on the current Active Roster.");
			GUI.Box (new Rect(Screen.width - (Screen.width / 3.75f), Screen.height / 10, Screen.width / 4, Screen.height - (Screen.height / 8f)), content, GUI.skin.window);

			pitcher_scroll_pos = GUI.BeginScrollView(new Rect(Screen.width - (Screen.width / 3.9f), Screen.height / 5.5f, Screen.width / 4.25f, Screen.height - (Screen.height / 4)), pitcher_scroll_pos, 
				new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 4.5f, (Screen.height / (20 / player_list.Length))));
			selected_pitcher = GUI.SelectionGrid (new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 4.25f, (Screen.height / (20 / player_list.Length))), selected_pitcher, player_list, 1);
			GUI.EndScrollView ();

			if ((current_pitcher_throwing_power / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_pitcher_throwing_power / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width - (Screen.width / 2.275f), Screen.height - (Screen.height / 22f), Screen.width / (6f / (current_pitcher_throwing_power / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);
		}
	}

	private void UpdateCharMesh(string which_mesh) {

		if (which_mesh == "batter") {
			if (batter_mesh != null) {

				Destroy (batter_mesh);
			}

			Material[] new_mesh_mats = new Material[3];

			switch (GameController.control.GetPlayer (selected_batter).GetRace ().GetName ()) {
			case "Human":
				batter_mesh = Instantiate (GameController.control.Human [GameController.control.GetPlayer (selected_batter).GetClass ().GetId ()], new Vector3 (633.7f, 327.15f, 612f), Quaternion.Euler (-45.5f, 44.9f, -1.2f));
				new_mesh_mats = batter_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
				new_mesh_mats [0] = GameController.control.HumanSkin [GameController.control.GetPlayer (selected_batter).GetSkin ()];
				break;

			case "Orc":
				batter_mesh = Instantiate (GameController.control.Orc [GameController.control.GetPlayer (selected_batter).GetClass ().GetId ()], new Vector3 (633.7f, 327.15f, 612f), Quaternion.Euler (-45.5f, 44.9f, -1.2f));
				new_mesh_mats = batter_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
				//new_mesh_mats [1] = GameController.control.OrcSkin [GameController.control.GetPlayer (selected_batter).GetSkin ()];
				new_mesh_mats [0] = GameController.control.OrcSkin [GameController.control.GetPlayer (selected_batter).GetSkin ()];
				break;

			default:
				break;
			}
				
			switch (GameController.control.GetPlayer (selected_batter).GetClass ().GetName ()) {
			case "Monk":
				new_mesh_mats [2] = GameController.control.MonkUniforms [GameController.control.GetPlayer (selected_batter).GetUni ()];
				break;

			case "Rogue":
				new_mesh_mats [1] = GameController.control.RogueUniforms [GameController.control.GetPlayer (selected_batter).GetUni ()];
				break;

			default:
				break;

			}
			batter_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = new_mesh_mats;
			batter_mesh.transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer (selected_batter).GetEyes ()];
			batter_mesh.transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer (selected_batter).GetEyes ()];

			current_batter_stats = GameController.control.GetPlayer (selected_batter).GetStats ();
			stat_text = new string[current_batter_stats.Count];
			int counter = 0;
			foreach (KeyValuePair<string, int> stat in current_batter_stats) {

				stat_text [counter] = stat.Key + " " + stat.Value;
				counter++;
			}
		}

		if (which_mesh == "pitcher") {

			if (pitcher_mesh != null) {

				Destroy (pitcher_mesh);
			}

			Material[] new_mesh_mats = new Material[3];

			switch (GameController.control.GetPlayer (selected_pitcher).GetRace ().GetName ()) {
			case "Human":
				pitcher_mesh = Instantiate (GameController.control.Human [GameController.control.GetPlayer (selected_pitcher).GetClass ().GetId ()], new Vector3 (632.6f, 327.15f, 613f), Quaternion.Euler (-47.4f, 67.9f, -18.1f));
				new_mesh_mats = pitcher_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
				new_mesh_mats [0] = GameController.control.HumanSkin [GameController.control.GetPlayer (selected_pitcher).GetSkin ()];
				break;

			case "Orc":
				pitcher_mesh = Instantiate (GameController.control.Orc [GameController.control.GetPlayer (selected_pitcher).GetClass ().GetId ()], new Vector3 (632.6f, 327.15f, 613f), Quaternion.Euler (-47.4f, 67.9f, -18.1f));
				new_mesh_mats = pitcher_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
				//new_mesh_mats [1] = GameController.control.OrcSkin [GameController.control.GetPlayer (selected_pitcher).GetSkin ()];
				new_mesh_mats [0] = GameController.control.OrcSkin [GameController.control.GetPlayer (selected_pitcher).GetSkin ()];
				break;

			default:
				break;
			}

			switch (GameController.control.GetPlayer (selected_pitcher).GetClass ().GetName ()) {
			case "Monk":
				new_mesh_mats [2] = GameController.control.MonkUniforms [GameController.control.GetPlayer (selected_pitcher).GetUni ()];
				break;

			case "Rogue":
				new_mesh_mats [1] = GameController.control.RogueUniforms [GameController.control.GetPlayer (selected_pitcher).GetUni ()];
				break;

			default:
				break;

			}
			pitcher_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = new_mesh_mats;
			pitcher_mesh.transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer (selected_pitcher).GetEyes ()];
			pitcher_mesh.transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer (selected_pitcher).GetEyes ()];

			current_pitcher_throwing_power = GameController.control.GetPlayer (selected_pitcher).GetStats () ["Throwing Power"];
		}
	}
}
