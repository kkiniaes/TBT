using UnityEngine;
using System.Collections;

public class MainMenuPlayer : MonoBehaviour {

	public GameObject lines;
	public static bool LINES_ARE_LOADED;

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
		if(!LINES_ARE_LOADED) {
			GameObject.Instantiate(lines,Vector3.zero,Quaternion.identity);
		} else {
			GetComponent<Animator>().SetBool("SkipMainMenu", true);
			GetComponent<AudioSource>().time = 62.4f;
			state = MenuState.Main;
		}
	}
	
	// Update is called once per frame
	void Update () {
//		GetComponent<Animator>().SetBool("SplashToMenu", false);


		switch(state) {

		case MenuState.Splash:
			SplashUpdate();
			break;
		case MenuState.Main:
			MainUpdate();
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
		if(GetComponent<AudioSource>().time > 35) {
			GetComponent<AudioSource>().time = 21;
		}
	}

	void MainUpdate() {
//		if(GetComponent<AudioSource>().time >= (GetComponent<AudioSource>().clip.length-0.01f)) {
//			GetComponent<AudioSource>().time = 62.4f;
//		}
		if(!GetComponent<AudioSource>().isPlaying) {
			GetComponent<AudioSource>().time = 62.4f;
			GetComponent<AudioSource>().Play();
		}
	}

	void GoToMain() {
		GetComponent<Animator>().SetBool("SplashToMenu", true);
		state = MenuState.Main;
		GetComponent<AudioSource>().time = 35;
	}
}
