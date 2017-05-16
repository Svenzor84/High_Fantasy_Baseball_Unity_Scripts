/// <summary>
/// Game controller.cs
/// By: Steve Ross-Byers
/// Created: 01/28/2017
/// Description: Game Controller for High Fantasy Baseball. Handles GUI setup, Persistent Data, Save/Load functionality, and Game Mode Selection.
/// </summary>

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	//static reference for pseudo-singleton setup
	public static GameController control;

	//the ball
	public GameObject the_ball;

	//arrays
	private Player_Character[] Players;
	private Team[] Teams;
	//Races
	public GameObject[] Human;
	public Material[] HumanSkin;
	public GameObject[] Orc;
	public Material[] OrcSkin;

	//Weapons
	private IList<Weapon> Weapons;
	public GameObject[] WeaponMeshes;

	//Eyes
	public Material[] Eyes;

	//Classes
	public Material[] MonkUniforms;
	public Material[] RogueUniforms;

	public GUISkin[] gui_skins;

	private GameObject camera;
	private string current_game_mode;

	//game mode variables
	private int batter_id;
	private int pitcher_id;
	private int home_team_id;
	private int away_team_id;

	public Texture white;

	private IDictionary<string, float> mode_metrics;

	void Awake () {
	
		//pseudo-singleton setup
		if (control == null) {
			DontDestroyOnLoad (gameObject);
			control = this;
		} else if (control != this) {
			Destroy (gameObject);
		}

		camera = this.gameObject;

		//mode_metrics = new Dictionary<string, float> ();

		//set up weapons
		Weapons = new List<Weapon>();
		Weapons.Add (new Weapon ("Unarmed", "Str", 0));
		Weapons.Add (new Weapon ("Short Sword", "Dex", 2));
	}
		
	void OnEnable() {

		SceneManager.sceneLoaded += SceneLoaded;

		LoadPlayers ();
		LoadTeams ();
	}

	void OnGUI() {

		//set up GUI Styles
		GUIStyle center_text = new GUIStyle("box");
		center_text.alignment = TextAnchor.MiddleCenter;
		center_text.fontSize = Screen.width / 125;

		//GUI.color = Color.white;
		GUI.contentColor = Color.black;

		//team and player count
		if (Players == null) {

			GUI.Label (new Rect (5, 0, 150, 25), "0 Players");

		} else {

			GUI.Label (new Rect (5, 0, 150, 25), Players.Length + " Players");
		}
			
		if (Teams == null) {

			GUI.Label (new Rect (5, 15, 150, 25), "0 Teams");

		} else {

			GUI.Label (new Rect (5, 15, 150, 25), Teams.Length + " Teams");
		}

		if (current_game_mode != null) {

			//display the current game mode at the bottom middle of the screen
			GUI.Label (new Rect ((Screen.width / 2) - (Screen.width / 19), Screen.height - Screen.height / 22, Screen.width / 9.5f, Screen.height / 30), current_game_mode + " mode", center_text);
		} else {
			GUI.Label (new Rect ((Screen.width / 2) - (Screen.width / 19), Screen.height - Screen.height / 22, Screen.width / 9.5f, Screen.height / 30), "no mode", center_text);
		}
	}
	//CUSTOM FUNCTIONS
	void SceneLoaded(Scene scene, LoadSceneMode mode) {

		current_game_mode = scene.name;

		if (current_game_mode == "creation") {

			gameObject.AddComponent<CreationMenu> ();
		}

		if (current_game_mode == "play") {

			gameObject.AddComponent<PlayMenu> ();
		}
	}

	//camera movement, pass in the amount to move, followed by a limiting value
	public void MoveCameraX(float x_move, float limit) {

		float cam_x = camera.transform.position.x;

		if (x_move < 0) {
			if ((cam_x + x_move) < limit) {

				cam_x = limit;

			} else {

				cam_x += x_move;
			}

		} else if (x_move > 0) {

			if ((cam_x + x_move) > limit) {

				cam_x = limit;

			} else {

				cam_x += x_move;
			}
		}
			
		Vector3 final_pos = new Vector3 (cam_x, camera.transform.position.y, camera.transform.position.z);
		camera.transform.position = final_pos;
	}
	public void MoveCameraY(float y_move, float limit) {

		float cam_y = camera.transform.position.y;

		if (y_move < 0) {
			if ((cam_y + y_move) < limit) {

				cam_y = limit;

			} else {

				cam_y += y_move;
			}

		} else if (y_move > 0) {

			if ((cam_y + y_move) > limit) {

				cam_y = limit;

			} else {

				cam_y += y_move;
			}
		}

		Vector3 final_pos = new Vector3 (camera.transform.position.x, cam_y, camera.transform.position.z);
		camera.transform.position = final_pos;
	}
	public void MoveCameraZ(float z_move, float limit) {

		float cam_z = camera.transform.position.z;

		if (z_move < 0) {
			if ((cam_z + z_move) < limit) {

				cam_z = limit;

			} else {

				cam_z += z_move;
			}

		} else if (z_move > 0) {
			
			if ((cam_z + z_move) > limit) {

				cam_z = limit;

			} else {

				cam_z += z_move;
			}
		}

		Vector3 final_pos = new Vector3 (camera.transform.position.x, camera.transform.position.y, cam_z);
		camera.transform.position = final_pos;
	}

	public void SavePlayer(Player_Character new_player) {

		if (Players != null) {

			new_player.SetId (Players.Length);

			Player_Character[] new_player_list = new Player_Character[Players.Length + 1];

			for (int i = 0; i < Players.Length; i++) {

				new_player_list [i] = Players [i];
			}

			new_player_list [Players.Length] = new_player;

			Players = new_player_list;

		} else {

			new_player.SetId (0);
			Players = new Player_Character[1] {new_player};
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream player_list = File.Create(Application.persistentDataPath + "/player_data.dat");
		bf.Serialize (player_list, Players);
		player_list.Close ();
	}

	public void LoadPlayers() {

		if (File.Exists(Application.persistentDataPath + "/player_data.dat")) {

			BinaryFormatter bf = new BinaryFormatter();
			FileStream player_file = File.Open(Application.persistentDataPath + "/player_data.dat", FileMode.Open);
			Players = (Player_Character[])bf.Deserialize (player_file);
			player_file.Close ();

		} else {

			Players = null;
		}
	}

	public void SaveTeam(Team new_team) {

		if (Teams != null) {

			new_team.SetId (Teams.Length);

			Team[] new_team_list = new Team[Teams.Length + 1];

			for (int i = 0; i < Teams.Length; i++) {

				new_team_list [i] = Teams [i];
			}

			new_team_list [Teams.Length] = new_team;

			Teams = new_team_list;

		} else {

			new_team.SetId (0);
			Teams = new Team[1] {new_team};
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream team_list = File.Create(Application.persistentDataPath + "/team_data.dat");
		bf.Serialize (team_list, Teams);
		team_list.Close ();
	}

	public void LoadTeams() {

		if (File.Exists(Application.persistentDataPath + "/team_data.dat")) {

			BinaryFormatter bf = new BinaryFormatter();
			FileStream team_file = File.Open(Application.persistentDataPath + "/team_data.dat", FileMode.Open);
			Teams = (Team[])bf.Deserialize (team_file);
			team_file.Close ();

		} else {

			Teams = null;
		}
	}

	public Player_Character[] GetPlayers() {
			
		return Players;

	}

	public Player_Character GetPlayer(int index) {

		return Players [index];
	}

	public Player_Character GetPlayerById(int p_id) {
		Player_Character requested_player = null;
		foreach (Player_Character player in Players) {
			if (player.GetId () == p_id) {
				requested_player = player;
			}
		}
		return requested_player;
	}

	public Team[] GetTeams() {

		return Teams;

	}

	public Team GetTeam(int index) {

		return Teams [index];
	}

	//function used to set up batting practice and home run derby game modes, where only a pitcher and batter are required (this may get replaced)
	public void SetBatterPitcher(int new_batter_id, int new_pitcher_id) {

		batter_id = new_batter_id;
		pitcher_id = new_pitcher_id;

	}

	public int GetBatterId() {

		return batter_id;
	}

	public int GetPitcherId() {

		return pitcher_id;
	}
		
	public IDictionary<string, float> Mode_metrics {

		get {
			return mode_metrics;
		}
		set {
			mode_metrics = value;
		}
	}

	public void ReportStat(string metric, float value, bool replace = false) {

		if (mode_metrics != null) {

			if (mode_metrics.ContainsKey (metric)) {

				if (replace) {

					mode_metrics [metric] = value;

				} else {

					mode_metrics [metric] += value;
				}

			} else {

				mode_metrics.Add (metric, value);
			}

		} else {

			//mode_metrics = new Dictionary<string, float> { metric, value };
		}
	}

	public Weapon GetWeapon(int index) {

		return Weapons [index];
	}
}

[Serializable]
public class Player_Race {

	string name;
	string description;
	IDictionary<string, int> att_dice;

	//default constructor creates the Human race
	public Player_Race() {

		name = "Human";
		description = "Humans are the shortest lived and least specialized of the humanoid races. " +
					  "They are neither as agile as the Elves, stout as the Dwarves, nor strong as the Orcs. " +
					  "It is through a combination of flexibility, passion, and cunning that they find their advantage.\n" +
					  "Humans can be successful members of a Team when participating as any class.";

		att_dice = new Dictionary<string, int> {
			{"Str", 2},
			{"Dex", 2},
			{"Int", 2},
			{"Con", 2},
		};
	}

	//specific constructor for all other races
	public Player_Race(string race_name, string race_desc, int r_str, int r_dex, int r_int, int r_con) {

		name = race_name;
		description = race_desc;
		att_dice = new Dictionary<string, int>{
			{"Str", r_str},
			{"Dex", r_dex},
			{"Int", r_int},
			{"Con", r_con},
		};
	}

	public string GetName() {

		return name;
	}

	public string GetDescription() {

		return description;
	}

	public int GetAtt(string att) {

		return att_dice[att];
	}
}

[Serializable]
public class Player_Class {

	int class_id;
	string name;
	string description;
	string[] pref_positions;
	IDictionary<string, int> att_bonuses;

	public Player_Class() {

		name = "Monk";
		description = "The Monk is the most flexible and balanced class to take the field. " + 
					  "They are equal parts power, agility, brains, and toughness. " +
					  "Unlike the other classes, the Monk uses no equipment and suffers no Attribute penalties.";

		pref_positions = new string[1] {"All"};
		att_bonuses = new Dictionary<string, int>{
			{"Str", 1},
			{"Dex", 1},
			{"Int", 1},
			{"Con", 1},
		};

		class_id = 0;
	}

	public Player_Class(string class_name, string class_desc, string[] class_pos, int c_str, int c_dex, int c_int, int c_con, int new_id) {

		name = class_name;
		description = class_desc;
		pref_positions = class_pos;
		att_bonuses = new Dictionary<string, int> {
			{ "Str", c_str },
			{ "Dex", c_dex },
			{ "Int", c_int },
			{ "Con", c_con },
		};

		class_id = new_id;
	}

	public int GetId() {

		return class_id;
	}

	public string GetName() {

		return name;
	}

	public string GetDescription() {

		return description;
	}

	public int GetAtt(string att) {

		return att_bonuses[att];
	}

	public string[] GetPrefPos() {

		return pref_positions;
	}
}

[Serializable]
public class Player_Character {

	int player_id;
	string name;
	int level;
	int experience;
	int current_hp;
	int current_stam;
	IDictionary<string, int> attributes;
	IDictionary<string, int> statistics;
	Player_Race p_race;
	Player_Class p_class;
	int skin_index;
	int eyes_index;
	int default_uniform_index;
	int equipped_weapon;

	public Player_Character(string p_name, IDictionary<string, int> atts, IDictionary<string, int> stats, Player_Race new_race, Player_Class new_class, int new_skin, int new_eyes, int uniform) {

		name = p_name;
		attributes = atts;
		statistics = stats;
		current_hp = statistics ["Health"];
		current_stam = statistics ["Stamina"];
		p_race = new_race;
		p_class = new_class;
		skin_index = new_skin;
		eyes_index = new_eyes;
		default_uniform_index = uniform;
		level = 1;
		experience = 0;

		int default_weapon;

		switch (p_class.GetName()) {

		case "Rogue":
			default_weapon = 1;
			break;

		default:
			default_weapon = 0;
			break;
		}

		EquipWeapon (default_weapon);
	}

	public void EquipWeapon(int weap_index) {
		equipped_weapon = weap_index;
	}

	public int GetLevel() {

		return level;
	}

	public string GetName () {

		return name;
	}

	public Player_Race GetRace() {

		return p_race;
	}

	public Player_Class GetClass() {

		return p_class;
	}

	public IDictionary<string, int> GetAtts() {

		return attributes;
	}

	public IDictionary<string, int> GetStats() {

		return statistics;
	}

	public int GetSkin() {

		return skin_index;
	}

	public int GetUni() {

		return default_uniform_index;
	}

	public int GetEyes() {

		return eyes_index;
	}

	public int GetId() {

		return player_id;
	}

	public void SetId(int id) {

		player_id = id;
	}

	public int Equipped_weapon {

		get {

			return equipped_weapon;
		}
	}
}

[Serializable]
public class Team {

	int team_id;
	string name;
	int[] roster;
	int team_level;

	public Team(string t_name, int[] t_roster) {

		name = t_name;
		roster = t_roster;
		team_level = 0;
		CalculateTeamLevel ();
	}

	public int GetLevel() {
		return team_level;
	}

	public string GetName() {
		return name;
	}

	public void SetId(int id) {

		team_id = id;
	}

	//calculates the average team level for the team and returns the difference (if any change) from the previous team level
	public int CalculateTeamLevel() {

		int new_team_level = 0;

		foreach (int p_id in roster) {

			new_team_level += GameController.control.GetPlayerById(p_id).GetLevel ();
		}

		new_team_level = Mathf.RoundToInt ((float)new_team_level / roster.Length);

		int level_diff = new_team_level - team_level;

		if (level_diff != 0) {
			team_level = new_team_level;
		}

		return level_diff;
	}
		
}

[Serializable]
public class Weapon {

	string name;
	string att;
	int multiplier;

	public Weapon (string new_name, string attribute, int new_multiplier) {
		name = new_name;
		att = attribute;
		multiplier = new_multiplier;
	}

	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}

	public string Att {
		get {
			return att;
		}
		set {
			att = value;
		}
	}

	public int Multiplier {
		get {
			return multiplier;
		}
		set {
			multiplier = value;
		}
	}
}
