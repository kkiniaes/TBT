using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenuBlackHole : MonoBehaviour {

	private bool mouseOver;
	private static MainMenuPlayer player;
	private static GameObject blackHolesController;
	private AsyncOperation loading;
	private string levelToLoad = "NA";

	private float timeOffset;

	// Use this for initialization
	void Start () {
		if(player == null) {
			player = GameObject.Find("Player").GetComponent<MainMenuPlayer>();
			blackHolesController = GameObject.Find("Black Holes Controller");
			if(MainMenuPlayer.LINES_ARE_LOADED) {
				blackHolesController.GetComponent<Animator>().SetTrigger("Show");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(mouseOver);

//		transform.GetChild(0).LookAt(Camera.main.transform);
//		transform.GetChild(0).localEulerAngles += new Vector3(0f,90f,0f);
//		if(Vector3.Distance(Camera.main.transform.position, transform.position) < 10) {
//			Camera.main.fieldOfView = 900/(Vector3.Distance(Camera.main.transform.position, transform.position));
//		}
		transform.GetChild(0).Rotate(transform.up*Time.deltaTime*200f);
//		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_BumpAmt", Vector3.Distance(transform.position, Camera.main.transform.position)*4f);
//		transform.GetChild(0).Rotate(transform.forward*Time.deltaTime*200f);
		if(mouseOver) {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one, Time.deltaTime*2f);
			if(Input.GetKeyDown(KeyCode.Alpha1)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "Switches";
					LoadLevel(2);
				} else if(gameObject.name.Contains("4")) {
					levelToLoad = "ForceField";
					LoadLevel(5);
				} else if(gameObject.name.Contains("2")) {
					levelToLoad = "WeightWatching";
					LoadLevel(4);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "IntroAntimatter";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "IntroToGravity";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "WindTunnel";
					LoadLevel(2);
				} else if(gameObject.name.Contains("2")) {
					levelToLoad = "Machine";
					LoadLevel(4);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "ThreadTheNeedle";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "BarriersToGravity";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha3)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "Chris";
					LoadLevel(2);
				} else if(gameObject.name.Contains("2")) {
					levelToLoad = "GiantMaze";
					LoadLevel(4);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "TugOfWar";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "BarriersToGravity2";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha4)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "LongMaze";
					LoadLevel(2);
				} else if(gameObject.name.Contains("2")) {
					levelToLoad = "Slit";
					LoadLevel(4);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "ControlPanel";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "TimePausingSpheres";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha5)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "IntroFission";
					LoadLevel(2);
				} else if(gameObject.name.Contains("2")) {
					levelToLoad = "AntiMineField";
					LoadLevel(4);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "LargeMaze";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "Gate";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha6)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "Funnel";
					LoadLevel(2);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "AntiMatter";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "SmallMaze";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha7)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "IntroToQE";
					LoadLevel(2);
				} else if(gameObject.name.Contains("1")) {
					levelToLoad = "Timing";
					LoadLevel(3);
				} else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "CombiningElements";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha8)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "IntroToQuantumEntanglement";
					LoadLevel(2);
				}
				else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "OverTheWall";
					LoadLevel(1);
				}
			} else if(Input.GetKeyDown(KeyCode.Alpha9)) {
				if(gameObject.name.Contains("5")) {
					levelToLoad = "CircuitSwitch";
					LoadLevel(2);
				}
				else if(gameObject.name.Equals("BlackHole")) {
					levelToLoad = "Slopes";
					LoadLevel(1);
				}
			}
		} else {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one*0.925f, Time.deltaTime*2f);
		}

		if(timeOffset > 0) {
			timeOffset += Time.deltaTime;
			if(timeOffset > 3f) {
				loading.allowSceneActivation = true;
			}
		}


	}

	public void OnPointerEnter() {
		mouseOver = true;
	}

	public void OnPointerExit() {
		mouseOver = false;
	}

	public void LoadLevel(int num) {
		player.GetComponent<Animator>().SetInteger("LoadLevel", num);
		blackHolesController.GetComponent<Animator>().SetInteger("LoadLevel", num);
		if(levelToLoad.Equals("NA")) {
			if(gameObject.name.Contains("5")) {
				loading = Application.LoadLevelAsync("Switches");
			} else if(gameObject.name.Contains("4")) {
				loading = Application.LoadLevelAsync("ForceField");
			} else if(gameObject.name.Contains("2")) {
				loading = Application.LoadLevelAsync("WeightWatching");
			} else if(gameObject.name.Contains("1")) {
				loading = Application.LoadLevelAsync("IntroAntimatter");
			} else {
				loading = Application.LoadLevelAsync("IntroToGravity");
			}
		} else {
			loading = Application.LoadLevelAsync(levelToLoad);
		}
		loading.allowSceneActivation = false;
		timeOffset = 1;
	}

}
