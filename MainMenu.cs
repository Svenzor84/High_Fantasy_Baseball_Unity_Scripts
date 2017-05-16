using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene (int scene_index) {

		if (GameController.control.GetPlayers () == null && scene_index == 1) {

			//turn on a boolean that opens a modal explaining that you must create players in order to proceed to the Play menu

		} else {
			
			SceneManager.LoadSceneAsync (scene_index);
			Destroy (this);
		}
	}

	public void Quit () {
		Application.Quit ();
	}
}
