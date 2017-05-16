using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreationMenu : MonoBehaviour {

	private string[] player_roster;
	private string[] team_list;
	private Player_Character current_player;
	private IDictionary<string, int> current_player_atts;
	private GUIContent content;
	private GameObject camera;
	public Texture testure;

	private Vector2 players_scroll_pos;
	private Vector2 teams_scroll_pos;

	// Use this for initialization
	void Awake () {

		camera = this.gameObject;
		camera.transform.position = new Vector3 (0, 1.43f, -3);
		camera.transform.rotation = Quaternion.Euler (10, 0, 0);

		if (GameController.control.GetPlayers () != null) {

			player_roster = new string[GameController.control.GetPlayers ().Length];
			for (int i = 0; i < GameController.control.GetPlayers().Length; i++) {

				current_player = GameController.control.GetPlayer (i);
				current_player_atts = current_player.GetAtts ();
				player_roster [i] = current_player.GetName () + " (" + current_player.GetRace ().GetName () + " " + current_player.GetClass ().GetName () + " " + current_player.GetLevel() + ")\n" 
					+ "Str " + current_player_atts["Str"] + " / Dex " + current_player_atts["Dex"] + " / Int " + current_player_atts["Int"] + " / Con " + current_player_atts["Con"];
			}

		} else {

			player_roster = new string[1] {"No Active Players"};
		}

		if (GameController.control.GetTeams() != null) {

			Team[] teams = GameController.control.GetTeams ();
			team_list = new string[teams.Length];
			for (int i = 0; i < teams.Length; i++) {
				team_list [i] = teams[i].GetName () + " (Team Level " + teams[i].GetLevel() + ")";
			}

		} else {

			team_list = new string[1] {"No Active Teams"};
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// get your UI on
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

		GUI.skin.window.fontSize = Screen.width / 50;
		content = new GUIContent ("Players", testure, "This panel shows all Players on the current Active Roster.");
		GUI.Box (new Rect(Screen.width / 25, Screen.height / 30, Screen.width / 3, Screen.height - (Screen.height / 15)), content, GUI.skin.window);

		players_scroll_pos = GUI.BeginScrollView(new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.25f, Screen.height - (Screen.height / 6)), players_scroll_pos, 
			new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.5f, (Screen.height / (20 / player_roster.Length))));
		GUI.SelectionGrid (new Rect(Screen.width / 19, Screen.height / 9, Screen.width / 3.25f, (Screen.height / (20 / player_roster.Length))), 0, player_roster, 1, GUI.skin.box);
		GUI.EndScrollView ();

		//Center menu
		GUI.color = Color.green;
		if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width / 10), Screen.height / 4, Screen.width / 5, Screen.height / 10), "Create New Player")) {

			gameObject.AddComponent<CharacterCreator> ();
			Destroy (this);
		}

		GUI.color = Color.blue;
		if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width / 10), Screen.height / 2.75f, Screen.width / 5, Screen.height / 10), "Create New Team")) {

			gameObject.AddComponent<TeamCreator> ();
			Destroy (this);
		}

		GUI.color = Color.red;
		if (GUI.Button(new Rect(Screen.width / 2 - (Screen.width / 10), Screen.height / 1.25f, Screen.width / 5, Screen.height / 10), "Exit")) {

			SceneManager.LoadSceneAsync (0);
			Destroy (this);
		}

		GUI.color = Color.white;
		GUI.skin.window.fontSize = Screen.width / 50;
		content = new GUIContent ("Teams", testure, "This panel shows all active Teams.");
		GUI.Box (new Rect(Screen.width - (Screen.width / 25) - (Screen.width / 3), Screen.height / 30, Screen.width / 3, Screen.height - (Screen.height / 15)), content, GUI.skin.window);

		teams_scroll_pos = GUI.BeginScrollView(new Rect(Screen.width - (Screen.width / 37) - (Screen.width / 3), Screen.height / 9, Screen.width / 3.25f, Screen.height - (Screen.height / 6)), teams_scroll_pos, 
			new Rect(Screen.width - (Screen.width / 37) - (Screen.width / 3), Screen.height / 9, Screen.width / 3.5f, (Screen.height / (10 / team_list.Length))));
		GUI.SelectionGrid (new Rect(Screen.width - (Screen.width / 37) - (Screen.width / 3), Screen.height / 9, Screen.width / 3.25f, (Screen.height / (10 / team_list.Length))), 0, team_list, 1, GUI.skin.box);
		GUI.EndScrollView();
	}
}
