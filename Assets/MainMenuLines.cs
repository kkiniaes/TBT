using UnityEngine;
using System.Collections;

public class MainMenuLines : MonoBehaviour {

	void Awake() {
		MainMenuPlayer.LINES_ARE_LOADED = true;
	}

	void OnLevelWasLoaded() {
		if(Application.loadedLevel != 0) {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
