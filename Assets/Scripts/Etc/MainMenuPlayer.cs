using UnityEngine;
using System.Collections;

public class MainMenuPlayer : MonoBehaviour {

	private enum MenuState {
		Splash,
		Main,
		Options,
		Credits
	}

	private MenuState state;

	// Use this for initialization
	void Start () {
		state = MenuState.Splash;
	}
	
	// Update is called once per frame
	void Update () {
//		GetComponent<Animator>().SetBool("SplashToMenu", false);


		switch(state) {

		case MenuState.Splash:
			SplashUpdate();
			break;
		case MenuState.Main:
			break;
		case MenuState.Options:
			break;
		case MenuState.Credits:
			break;

		}
	}

	void SplashUpdate() {
		if(Input.anyKeyDown) {
			GoToMain();
		}
	}

	void GoToMain() {
		GetComponent<Animator>().SetBool("SplashToMenu", true);
		state = MenuState.Main;
	}
}
