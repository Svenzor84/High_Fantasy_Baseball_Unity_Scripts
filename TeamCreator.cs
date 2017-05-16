using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCreator : MonoBehaviour {

	private GUIContent content;
	private Texture testure;
	private string[] player_roster;
	private Player_Character current_player;
	private IDictionary<string, int> current_player_atts;
	private IDictionary<string, int> current_player_stats;
	private string[] stat_text;

	private string team_name;
	private int[] team_roster;
	private string roster_text;

	private Player_Character[] temp_roster;
	private Material[] new_mesh_mats;

	private GameObject player_mesh;

	private Rect modal_rect;
	private bool exit_modal_open;
	private bool save_modal_open;

	private int selected_player;
	private int last_player;
	private bool selected_in_roster;

	private Vector2 players_scroll_pos;

	// Use this for initialization
	void Awake () {

		GetComponent<Camera> ().transform.position = new Vector3 (0, 1.8f, -2);

		temp_roster = GameController.control.GetPlayers ();

		team_name = "";

		roster_text = "No Players have been added to this Team";

		exit_modal_open = false;
		save_modal_open = false;

		if (temp_roster != null) {

			player_roster = new string[temp_roster.Length];
			for (int i = 0; i < temp_roster.Length; i++) {

				current_player = temp_roster[i];
				current_player_atts = current_player.GetAtts ();
				player_roster [i] = current_player.GetName () + " (" + current_player.GetRace ().GetName () + " " + current_player.GetClass ().GetName () + " " + current_player.GetLevel() + ")\n" 
					+ "Str " + current_player_atts["Str"] + " / Dex " + current_player_atts["Dex"] + " / Int " + current_player_atts["Int"] + " / Con " + current_player_atts["Con"];
			}

		} else {

			player_roster = new string[1] {"No Active Players"};
		}

		last_player = 0;
		selected_in_roster = false;

		if (temp_roster != null) {

			UpdateCharMesh ();
			UpdateCharStats ();
		} else {
			stat_text = new string[] {""};
		}
	}

	void Update() {

		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if ((selected_player + 1) == player_roster.Length) {
				selected_player = 0;
			} else {
				selected_player++;
			}

		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if ((selected_player - 1) < 0) {
				selected_player = player_roster.Length - 1;
			} else {
				selected_player--;
			}
		} else if (Input.GetKeyDown (KeyCode.Return) && (team_roster == null || team_roster.Length < 9 || selected_in_roster)) {
			UpdateRoster(GameController.control.GetPlayer(selected_player).GetId());
		}

		if (selected_player != last_player) {

			last_player = selected_player;
			UpdateCharMesh ();
			UpdateCharStats ();


			if (team_roster != null && Array.Exists (team_roster, player_id => player_id == GameController.control.GetPlayer (selected_player).GetId())) {
				selected_in_roster = true;
			} else {
				selected_in_roster = false;
			}
		}
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
		content = new GUIContent ("Players", testure, "This panel shows all Players on the current Active Roster.");
		GUI.Box (new Rect(Screen.width / 25, Screen.height / 30, Screen.width / 3, Screen.height - (Screen.height / 15)), content, GUI.skin.window);

		players_scroll_pos = GUI.BeginScrollView(new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.25f, Screen.height - (Screen.height / 6)), players_scroll_pos, 
			new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.5f, (Screen.height / (20 / player_roster.Length))));
		selected_player = GUI.SelectionGrid (new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.25f, (Screen.height / (20 / player_roster.Length))), selected_player, player_roster, 1, GUI.skin.button);
		GUI.EndScrollView ();

		content = new GUIContent ("", testure, "This panel displays a summary of the selected player's ability.");
		GUI.Box (new Rect(Screen.width / 25 + (Screen.width / 3), Screen.height / 30, Screen.width / 4, Screen.height / 3.5f), content, GUI.skin.window);

		//attempt at stat bars
		if (temp_roster != null) {

			//hitting power
			if ((current_player_stats ["Hitting Power"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Hitting Power"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 26.25f, Screen.width / (4.08f / (current_player_stats["Hitting Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//throwing power
			if ((current_player_stats ["Throwing Power"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Throwing Power"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 12.85f, Screen.width / (4.08f / (current_player_stats["Throwing Power"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//speed
			if ((current_player_stats ["Speed"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Speed"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 8.5f, Screen.width / (4.08f / (current_player_stats["Speed"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//accuracy
			if ((current_player_stats ["Accuracy"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Accuracy"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 6.35f, Screen.width / (4.08f / (current_player_stats["Accuracy"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//magic
			if ((current_player_stats ["Magic"] / 100f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Magic"] / 100f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 5.05f, Screen.width / (4.08f / (current_player_stats["Magic"] / 100f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//health
			if ((current_player_stats ["Health"] / 200f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Health"] / 200f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 4.2f, Screen.width / (4.08f / (current_player_stats["Health"] / 200f)), Screen.height / 29), "", GUI.skin.customStyles[0]);

			//stamina
			if ((current_player_stats ["Stamina"] / 200f) > 0.79f) {
				GUI.color = Color.green;
			} else if ((current_player_stats ["Stamina"] / 200f) < 0.40f) {
				GUI.color = Color.red;
			} else {
				GUI.color = Color.yellow;
			}
			GUI.Label (new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 3.61f, Screen.width / (4.08f / (current_player_stats["Stamina"] / 200f)), Screen.height / 29), "", GUI.skin.customStyles[0]);
		}
			
		GUI.color = Color.white;
		//selected character stats go here
		GUI.SelectionGrid(new Rect(Screen.width / 23.75f + (Screen.width / 3), Screen.height / 26.25f, Screen.width / 4.08f, Screen.height / 3.65f), 0, stat_text, 1, GUI.skin.box);

		//add/remove button goes here
		GUI.skin.customStyles[2].fontSize = Screen.width / 120;

		if (selected_in_roster) {

			GUI.color = Color.red;
			content = new GUIContent ("<\n<\n<", testure, "Click here to remove this player from the Team Roster.");
			if (GUI.Button (new Rect (Screen.width / 25f + (Screen.width / 1.715f), Screen.height / 30f, Screen.width / 23f, Screen.height / 3.5f), content, GUI.skin.customStyles [2])) {
				UpdateRoster (GameController.control.GetPlayer(selected_player).GetId());
			}

		} else if (team_roster == null || team_roster.Length < 9) {
			GUI.color = Color.green;
			content = new GUIContent (">\n>\n>", testure, "Click here to add this player to the Team Roster.");
			if (GUI.Button (new Rect (Screen.width / 25f + (Screen.width / 1.715f), Screen.height / 30f, Screen.width / 23f, Screen.height / 3.5f), content, GUI.skin.customStyles [2]) && temp_roster != null) {
				UpdateRoster (GameController.control.GetPlayer(selected_player).GetId());
			}
		} else {
			GUI.color = Color.gray;
			content = new GUIContent ("Roster\nFull", testure, "The Team Roster is full and you may Save your Team.");
			GUI.Button (new Rect (Screen.width / 25f + (Screen.width / 1.715f), Screen.height / 30f, Screen.width / 23f, Screen.height / 3.5f), content, GUI.skin.customStyles [2]);
		}

		GUI.color = Color.white;
		GUI.skin.window.fontSize = Screen.width / 50;
		content = new GUIContent ("Team Roster", testure, "This panel shows the roster for the Team being edited.");
		GUI.Box (new Rect(Screen.width - (Screen.width / 2000) - (Screen.width / 3), Screen.height / 30, Screen.width / 4, Screen.height - (Screen.height / 5)), content, GUI.skin.window);

		content = new GUIContent (roster_text, testure, "This panel shows the roster for the Team being edited.");
		GUI.Label (new Rect(Screen.width - (Screen.width / 2000) - (Screen.width / 3.1f), Screen.height / 9, Screen.width / 4.35f, Screen.height - (Screen.height / 3.3f)), content);

		if (team_roster != null && team_roster.Length == 9) {
			GUI.color = Color.green;
			if (GUI.Button (new Rect (Screen.width - (Screen.width / 25) - (Screen.width / 3), Screen.height - (Screen.height / 7), Screen.width / 3, Screen.height / 20), "Save")) {

				save_modal_open = true;
			}

		} else {

			GUI.color = Color.gray;
			GUI.Button (new Rect (Screen.width - (Screen.width / 25) - (Screen.width / 3), Screen.height - (Screen.height / 7), Screen.width / 3, Screen.height / 20), "Team Roster must total 9 Players to Save");
		}

		if (save_modal_open) {

			GUI.color = Color.white;
			GUI.skin.window.fontSize = Screen.width / 100;
			modal_rect = new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2);
			modal_rect = GUI.ModalWindow (1, modal_rect, Modal, "Enter a Name to Save your Team:");

		}

		GUI.color = Color.red;
		if (GUI.Button(new Rect(Screen.width - (Screen.width / 25) - (Screen.width / 3), Screen.height - (Screen.height / 13), Screen.width / 3, Screen.height / 20), "Exit")) {

			exit_modal_open = true;

		}

		if (exit_modal_open) {

			GUI.color = Color.white;
			GUI.skin.window.fontSize = Screen.width / 100;
			modal_rect = new Rect (Screen.width / 3, Screen.height / 5, Screen.width / 3f, Screen.height / 2);
			modal_rect = GUI.ModalWindow (0, modal_rect, Modal, "Are you sure you wish to exit?");

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
			GUI.Label (new Rect (Screen.width / 12, Screen.height / 5.75f + (Screen.height / 8), Screen.width / 6, Screen.height / 8), "Return to Team creation");
			break;

		case 1:
			GUI.SetNextControlName ("NameField");
			team_name = GUI.TextField (new Rect (Screen.width / 15, Screen.height / 12, Screen.width / 5, Screen.height / 15), team_name, 30);
			if (team_name == "") {

				GUI.color = Color.grey;
				GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "Enter a Name to Continue");

				GUI.FocusControl ("NameField");

			} else {

				bool name_taken = false;
				if (GameController.control.GetTeams () != null) {
					foreach (Team team in GameController.control.GetTeams()) {
						if (team_name == team.GetName ()) {
							name_taken = true;
						} 
					}
				}

				if (!name_taken) {
					GUI.color = Color.green;
					if (GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "Save Team")) {
						GameController.control.SaveTeam (new Team (team_name, team_roster));

						team_name = "";
						team_roster = null;
						selected_player = 0;
						roster_text = "No Players have been added to this Team";
						save_modal_open = false;
					}
				} else {
					GUI.color = Color.grey;
					GUI.Button (new Rect (Screen.width / 12, Screen.height / 5f, Screen.width / 6, Screen.height / 8), "Team Name is already taken");
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

	private void UpdateCharMesh() {

		if (player_mesh != null) {

			Destroy (player_mesh);
		}
			
		switch (GameController.control.GetPlayer (selected_player).GetRace ().GetName()) {
		case "Human":
			player_mesh = Instantiate (GameController.control.Human [GameController.control.GetPlayer (selected_player).GetClass ().GetId()], Vector3.zero, Quaternion.Euler (0, 160, 0));
			new_mesh_mats = player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
			new_mesh_mats[0] = GameController.control.HumanSkin[GameController.control.GetPlayer(selected_player).GetSkin()];
			break;

		case "Orc":
			player_mesh = Instantiate (GameController.control.Orc [GameController.control.GetPlayer (selected_player).GetClass ().GetId()], Vector3.zero, Quaternion.Euler (0, 160, 0));
			new_mesh_mats = player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials;
			new_mesh_mats[0] = GameController.control.OrcSkin[GameController.control.GetPlayer(selected_player).GetSkin()];
			break;

		default:
			break;
		}



		switch (GameController.control.GetPlayer (selected_player).GetClass ().GetName ()) {
		case "Monk":
			new_mesh_mats [2] = GameController.control.MonkUniforms [GameController.control.GetPlayer (selected_player).GetUni ()];
			break;

		case "Rogue":
			new_mesh_mats [1] = GameController.control.RogueUniforms [GameController.control.GetPlayer (selected_player).GetUni ()];
			break;
		
		default:
			break;

		}
		player_mesh.transform.GetChild (2).GetComponent<SkinnedMeshRenderer> ().materials = new_mesh_mats;
		player_mesh.transform.GetChild (0).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer(selected_player).GetEyes()];
		player_mesh.transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ().material = GameController.control.Eyes [GameController.control.GetPlayer(selected_player).GetEyes()];
	}


	private void UpdateCharStats() {

		current_player_stats = GameController.control.GetPlayer (selected_player).GetStats ();
		stat_text = new string[current_player_stats.Count];
		int counter = 0;
		foreach (KeyValuePair<string, int> stat in current_player_stats) {

			stat_text [counter] = stat.Key + " " + stat.Value;
			counter++;
		}
	}

	private void UpdateRoster (int player_id) {

		if (team_roster == null) {

			team_roster = new int[] { player_id };
			roster_text = "1. " + GameController.control.GetPlayerById(team_roster [0]).GetName () + "\n";

		} else if (!selected_in_roster) {

			int[] temp_team_roster = team_roster;
			team_roster = new int[team_roster.Length + 1];

			for (int i = 0; i < temp_team_roster.Length; i++) {

				team_roster [i] = temp_team_roster [i];
			}
				
			team_roster [team_roster.Length - 1] = player_id;

			roster_text = "";
			for (int i = 0; i < team_roster.Length; i++) {
				roster_text += (i + 1) + ". " + GameController.control.GetPlayerById (team_roster [i]).GetName () + "\n";
			}
		} else {

			int[] temp_team_roster = team_roster;
			team_roster = new int[team_roster.Length - 1];

			int j = 0;
			for (int i = 0; i < temp_team_roster.Length; i++) {

				if (temp_team_roster [i] != player_id) {

					team_roster [j] = temp_team_roster [i];
					j++;
				}
			}

			roster_text = "";
			for (int i = 0; i < team_roster.Length; i++) {
				roster_text += (i + 1) + ". " + GameController.control.GetPlayerById (team_roster [i]).GetName () + "\n";
			}
		}

		if (roster_text == "") {
			roster_text = "No Players have been added to this Team";
		}

		if (Array.Exists (team_roster, p_id => p_id == GameController.control.GetPlayer (selected_player).GetId())) {
			selected_in_roster = true;
		} else {
			selected_in_roster = false;
		}
	}
}
