using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {
	private enum PhysicsMode {
		Mass,
		Charge,
		Entangle,
	}
	private PhysicsMode currentMode = PhysicsMode.Mass;

	public float sensitivityX, sensitivityY;
	public float moveSpeed;
	public float timeScale = 1;

	public static Player instance;

	private Vector3 velocityVector;
	public bool timeFrozen;
	public bool timeReversed;
	private bool loadNextLevel;
	private float loadNextLevelTimer;
	private PhysicsModifyable entangleSelected;
	private bool wireframeMode;
	private GameObject starField;
	private GameObject timeSFXManager, physicsSFXManager;
	private AsyncOperation loadingNextLevel;
	private GameObject entangleLine;
	private bool gamePaused;
	private GameObject pauseMenu;
	private bool loadMainMenu;

	private static GameObject mainMenuLines;

	private float antimatterResetTime = 0;
	public float AntimatterResetTime {
		get { return antimatterResetTime; }
		set { antimatterResetTime = value; }
	}

	private float timeElapsed = 0;
	public float TimeElapsed { 
		get { return timeElapsed; } 
	}

	private GameObject lookingAtObject;
	public GameObject LookingAtObject {
		get { return lookingAtObject; }
	}

	private bool noStateChangesThisFrame = true;
	public bool NoStateChangesThisFrame {
		get { return noStateChangesThisFrame; }
		set { noStateChangesThisFrame = value; }
	}

	private const float REVERSE_TIME_SCALE = -3;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		starField = transform.FindChild("Starfield").gameObject;
		timeSFXManager = transform.FindChild("TimeSFXManager").gameObject;
		physicsSFXManager = transform.FindChild("PhysicsSFXManager").gameObject;
		pauseMenu = transform.parent.FindChild("Canvas").FindChild("PauseMenu").gameObject;
		if(mainMenuLines == null) {
			mainMenuLines = Resources.Load<GameObject>("Lines");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(timeElapsed <= antimatterResetTime) {
			antimatterResetTime = -1;
		}

		GetMovementInput();
		GetTimeManipInput();
		GetVisualModeInput();

		if(!loadNextLevel && GetComponent<Camera>().fieldOfView <= 90 && (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))) {
			gamePaused = !gamePaused;
			pauseMenu.GetComponent<Animator>().SetBool("GamePaused",gamePaused);
			if(gamePaused) {
				pauseMenu.GetComponent<CanvasGroup>().interactable = true;
				pauseMenu.transform.FindChild("PauseTitle").GetComponent<LevelIntroText>().Start();
				timeReversed = false;
				timeFrozen = true;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			} else {
				pauseMenu.GetComponent<CanvasGroup>().interactable = false;
				timeReversed = false;
				timeFrozen = false;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		//time manipulation code
		if(timeFrozen) {
			if(timeScale < 0) {
				timeScale = Mathf.MoveTowards(timeScale, 0, Time.deltaTime*3f*Mathf.Abs(REVERSE_TIME_SCALE));
			} else {
				timeScale = Mathf.MoveTowards (timeScale, 0, Time.deltaTime * 3f);
			}
			GetComponent<MotionBlur>().blurAmount = Mathf.MoveTowards(GetComponent<MotionBlur>().blurAmount, 0.5f, Time.deltaTime*3f);
		} else {
			if(timeReversed) {
				entangleSelected = null;
				if(timeElapsed <= 0) {
					timeReversed = false;
					timeElapsed = 0;
					noStateChangesThisFrame = true;
				} else {
					timeScale = Mathf.MoveTowards(timeScale, REVERSE_TIME_SCALE, Time.deltaTime*3f*Math.Abs(REVERSE_TIME_SCALE));
					GetComponent<VignetteAndChromaticAberration>().chromaticAberration = Mathf.MoveTowards(GetComponent<VignetteAndChromaticAberration>().chromaticAberration, -timeScale*30f, Time.deltaTime*10f*Math.Abs(REVERSE_TIME_SCALE));
				}
			} else {
				GetComponent<VignetteAndChromaticAberration>().chromaticAberration = Mathf.MoveTowards(GetComponent<VignetteAndChromaticAberration>().chromaticAberration, 0f, Time.deltaTime * 200f);
				if(timeScale < 0) {
					timeScale = Mathf.MoveTowards(timeScale, 0, Time.deltaTime*3f*Math.Abs(REVERSE_TIME_SCALE));
				} else {
					timeScale = Mathf.MoveTowards (timeScale, 1, Time.deltaTime * 3f);
				}
			}

			if(!noStateChangesThisFrame || timeReversed) {
				timeElapsed = Mathf.Max(0, timeElapsed + timeScale);
				noStateChangesThisFrame = true;
			}

			GetComponent<MotionBlur>().blurAmount = Mathf.MoveTowards(GetComponent<MotionBlur>().blurAmount, 0f, Time.deltaTime*3f);
		}

		//First person controls
		float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
		float rotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * sensitivityY;
		if(Camera.main.fieldOfView <= 90f && !gamePaused) {
			transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
			transform.Translate(velocityVector*Time.deltaTime, Space.World);
			velocityVector = Vector3.MoveTowards(velocityVector, Vector3.zero, Time.deltaTime*2f);
		}
	
		//switching skills
		if(!gamePaused) {
			if(Input.GetKeyDown(KeyCode.Alpha1)) {
				currentMode = PhysicsMode.Mass;
			} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
				currentMode = PhysicsMode.Charge;
			} else if(Input.GetKeyDown(KeyCode.Alpha3)) {
				currentMode = PhysicsMode.Entangle;
			}
		}

		RaycastHit rh = new RaycastHit();
		float clipPlane = Camera.main.nearClipPlane;
		Camera.main.nearClipPlane = 0.1f;
		Ray cameraRay = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f));
		Camera.main.nearClipPlane = clipPlane;
		Debug.DrawRay(transform.position, cameraRay.direction*10f);
		if(!gamePaused && Physics.Raycast(cameraRay.origin,cameraRay.direction, out rh, 100000, ~(1 << 10))) {
			GetComponent<DepthOfField>().focalLength = Vector3.Distance(transform.position, rh.point);
//			GetComponent<DepthOfField>().aperture = Mathf.MoveTowards(GetComponent<DepthOfField>().aperture, 10/(Vector3.Distance(transform.position, rh.point)), Time.deltaTime*10f);

			if(rh.transform.gameObject.GetComponent<PhysicsModifyable>() != null) {
				lookingAtObject = rh.transform.gameObject;

				if(!wireframeMode) {
					lookingAtObject.GetComponent<AutoWireframeWorld>().HighlightObject();
				}

				PhysicsModifyable pM = rh.transform.gameObject.GetComponent<PhysicsModifyable>();
				rh.transform.gameObject.GetComponent<Renderer>().material.SetFloat("_Power", 0.3f);

				float delta = 10 * Input.mouseScrollDelta.y*Time.deltaTime;
				if(delta == 0) {
					delta = (Input.GetKeyDown(KeyCode.UpArrow) ? 0.5f : 0) + -1 * (Input.GetKeyDown(KeyCode.DownArrow) ? 0.5f : 0);
				}

				if(!timeReversed && !pM.immutable) {
					//Increases/Decreases mass of object
					if(!pM.specificallyImmutable.mass && currentMode == PhysicsMode.Mass) {
						float tempVal = pM.Mass;
						pM.Mass = Mathf.Min(6, Mathf.Max(0, pM.Mass + delta));
						if(pM.Mass != tempVal) {
							physicsSFXManager.GetComponent<PhysicsSFXManager>().PlayGravityChangeSFX(delta);
						}
					}// Increase/Decrease charge of object 
					else if(!pM.specificallyImmutable.charge && currentMode == PhysicsMode.Charge) {
						if((pM.Charge == -1 && delta > 0) || (pM.Charge == 1 && delta < 0)) {
							pM.Charge = 0;
						} else if(delta != 0) {
							pM.Charge = Mathf.Sign(delta);
						}
					}// Quantum Entangle objects
					else{// if(!pM.specificallyImmutable.entangled && currentMode == PhysicsMode.Entangle) {
						if(Input.GetMouseButtonDown(0)) {
							if(pM.entangled != null) {
								//Debug.Log("Detangle");
								pM.entangled.Entangled = null;
								pM.Entangled = null;
								entangleSelected = null;
								if(entangleLine != null) {
									Destroy(entangleLine.gameObject);
								}
							} else if(entangleSelected != null && entangleSelected != pM) {
								//Debug.Log("Entangle " + pM + ":" + entangleSelected);
								pM.Entangled = entangleSelected;
								entangleSelected.Entangled = pM;
								entangleSelected = null;
								if(entangleLine != null) {
									Destroy(entangleLine.gameObject);
								}
							} else if(!pM.specificallyImmutable.entangled){
								//Debug.Log("Entangle " + pM);
								entangleSelected = pM;
								if(entangleLine == null) {
									entangleLine = new GameObject("EntangleLine", typeof(LineRenderer));
									entangleLine.GetComponent<LineRenderer>().useWorldSpace = true;
									entangleLine.GetComponent<LineRenderer>().sharedMaterial = Resources.Load<GameObject>("SwitchLine").GetComponent<LineRenderer>().sharedMaterial;
									entangleLine.GetComponent<LineRenderer>().material.color = Color.black;
									entangleLine.GetComponent<LineRenderer>().SetPosition(0,pM.transform.position);
								}
							}
						}
					}

					if(currentMode == PhysicsMode.Mass || currentMode == PhysicsMode.Charge) {
						entangleSelected = null;
					}
				}
			} else {
				lookingAtObject = null;
			}
		} else if(Input.GetMouseButtonDown(0)) {
			// handles canceling entanglement
			entangleSelected = null;
			if(entangleLine != null) {
				Destroy(entangleLine.gameObject);
			}
		} else {
			lookingAtObject = null;
		}

		//Handles entangle indicator
		if(entangleLine != null) {
			LineRenderer entangleRenderer = entangleLine.GetComponent<LineRenderer>();
			if(entangleSelected != null) {
				entangleRenderer.SetWidth(Vector3.Distance(transform.position,entangleSelected.transform.position)/30f,Vector3.Distance(transform.position,entangleSelected.transform.position)/30f);
				if(lookingAtObject == null) {
					entangleRenderer.SetPosition(1,transform.position + (transform.forward*Vector3.Distance(transform.position, entangleSelected.transform.position)));
				} else if(lookingAtObject != null && lookingAtObject.GetComponent<PhysicsModifyable>() != null) {
					entangleRenderer.SetPosition(1,lookingAtObject.transform.position);
				}
			}
		}


		//Loading next level with cool transition
		if(loadNextLevel && !loadMainMenu) {
			loadNextLevelTimer += Time.deltaTime;
			if(loadNextLevelTimer > 2f) {
				if(loadingNextLevel == null) {
					loadingNextLevel = Application.LoadLevelAsync(Application.loadedLevel + 1);
					loadingNextLevel.allowSceneActivation = false;
					physicsSFXManager.GetComponent<PhysicsSFXManager>().PlayWarpingSFX();
				}
				starField.GetComponent<ParticleSystemRenderer>().lengthScale = Mathf.MoveTowards(starField.GetComponent<ParticleSystemRenderer>().lengthScale, 100, Time.deltaTime*50f);
				Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 179f, Time.deltaTime*30f);
				transform.Translate(transform.forward*Time.deltaTime*Camera.main.fieldOfView/2f, Space.World);
				if(physicsSFXManager.GetComponent<AudioSource>().time/physicsSFXManager.GetComponent<AudioSource>().clip.length > 0.98f) {
					loadingNextLevel.allowSceneActivation = true;
				}
			}
		} else if (!loadNextLevel && !loadMainMenu) {
			starField.GetComponent<ParticleSystemRenderer>().lengthScale = Mathf.MoveTowards(starField.GetComponent<ParticleSystemRenderer>().lengthScale, 1, Time.deltaTime*50f);
			Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 90f, Time.deltaTime*Camera.main.fieldOfView/3f);
			if(Camera.main.fieldOfView > 90f) {
				transform.Translate(transform.forward*Time.deltaTime*10f, Space.World);
			}
		} else if(loadNextLevel && loadMainMenu) {
			lookingAtObject = null;
			loadNextLevelTimer += Time.deltaTime;
			if(loadNextLevelTimer > 0.2f) {
				if(loadingNextLevel == null) {
					StartCoroutine(LoadMainMenuSeamlessly());
					loadingNextLevel.allowSceneActivation = false;
					physicsSFXManager.GetComponent<PhysicsSFXManager>().PlayWarpingSFX();
				}
				starField.GetComponent<ParticleSystemRenderer>().lengthScale = Mathf.MoveTowards(starField.GetComponent<ParticleSystemRenderer>().lengthScale, 100, Time.deltaTime*50f);
				starField.transform.localPosition = new Vector3(0f, 0f, -300f);
				Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 179f, Time.deltaTime*30f);
				transform.Translate(-transform.forward*Time.deltaTime*Camera.main.fieldOfView/2f, Space.World);
				if(physicsSFXManager.GetComponent<AudioSource>().time/physicsSFXManager.GetComponent<AudioSource>().clip.length > 0.98f) {
					loadingNextLevel.allowSceneActivation = true;
				}
			}
		}


	}

	//movement input
	private void GetMovementInput() {
		if(Input.GetKey(KeyCode.W)) {
			velocityVector = Vector3.MoveTowards(velocityVector, transform.forward*moveSpeed, Time.deltaTime*4f);
		}
		if(Input.GetKey(KeyCode.S)) {
			velocityVector = Vector3.MoveTowards(velocityVector, -transform.forward*moveSpeed, Time.deltaTime*4f);
		}
		if(Input.GetKey(KeyCode.D)) {
			velocityVector = Vector3.MoveTowards(velocityVector, transform.right*moveSpeed, Time.deltaTime*4f);
		}
		if(Input.GetKey(KeyCode.A)) {
			velocityVector = Vector3.MoveTowards(velocityVector, -transform.right*moveSpeed, Time.deltaTime*4f);
		}
		
		if(Input.GetKey(KeyCode.R)) {
			velocityVector = Vector3.MoveTowards(velocityVector, transform.up*moveSpeed, Time.deltaTime*4f);
		}
		if(Input.GetKey(KeyCode.F)) {
			velocityVector = Vector3.MoveTowards(velocityVector, -transform.up*moveSpeed, Time.deltaTime*4f);
		}
	}

	private void GetTimeManipInput() {
		if (!loadNextLevel) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				if(antimatterResetTime == -1 || timeReversed == false) {
					timeReversed = false;
					timeFrozen = !timeFrozen;
				}
			}

			if(timeReversed) {
				timeSFXManager.GetComponent<AudioSource>().pitch = -0.5f;
				timeSFXManager.GetComponent<AudioSource>().volume = 1;
			} else {
				timeSFXManager.GetComponent<AudioSource>().pitch = timeScale + 0.5f;
				if(timeScale != 1 && timeScale != 0) {
					timeSFXManager.GetComponent<AudioSource>().volume = Mathf.MoveTowards(timeSFXManager.GetComponent<AudioSource>().volume, 1f, Time.deltaTime*10f);
				} else {
					timeSFXManager.GetComponent<AudioSource>().volume = Mathf.MoveTowards(timeSFXManager.GetComponent<AudioSource>().volume, 0.1f-Mathf.Abs(timeScale), Time.deltaTime);
				}
			}

			if (!timeFrozen) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					timeReversed = true;
				} else if (Input.GetKeyDown (KeyCode.RightArrow) && antimatterResetTime == -1) {
					timeReversed = false;
				}
			}
		}
	}

	private void GetVisualModeInput() {
		if(Input.GetKeyDown(KeyCode.Tab)) {
			wireframeMode = !wireframeMode;
		}
		if(wireframeMode) {
			float maxDistance = 0;
			foreach (PhysicsModifyable obj in PhysicsAffected.objs) {
				maxDistance = Mathf.Max (maxDistance, Vector3.Distance (obj.transform.position, transform.position));
			}
			// Some tolerance.
			maxDistance += 2;
			GetComponent<Camera>().nearClipPlane = Mathf.MoveTowards(GetComponent<Camera>().nearClipPlane, maxDistance, Time.deltaTime*(2+GetComponent<Camera>().nearClipPlane));
		} else {
			GetComponent<Camera>().nearClipPlane = Mathf.MoveTowards(GetComponent<Camera>().nearClipPlane, 0.1f, Time.deltaTime*(2+GetComponent<Camera>().nearClipPlane));
		}
		transform.FindChild("WireframeCam").GetComponent<Camera>().farClipPlane = GetComponent<Camera>().nearClipPlane;
	}

	public void LoadNextLevel() {
		loadNextLevel = true;
	}

	public bool BeatLevel {
		get {
			return loadNextLevelTimer > 2f;
		}
	}

	IEnumerator LoadMainMenuSeamlessly() {
		loadingNextLevel = Application.LoadLevelAsync(0);
		GameObject temp = new GameObject("Lines",typeof(MainMenuLines));
		DontDestroyOnLoad(temp);
		for(int i = 0; i < mainMenuLines.transform.childCount; i++) {
			GameObject lineTemp = GameObject.Instantiate(mainMenuLines.transform.GetChild(i).gameObject);
			lineTemp.transform.parent = temp.transform;
			Debug.Log("HERE");
			yield return null;
		}
	}

	public void ResumeGame() {
		gamePaused = !gamePaused;
		pauseMenu.GetComponent<Animator>().SetBool("GamePaused",gamePaused);
		timeReversed = false;
		timeFrozen = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void LoadMainMenu() {
		loadNextLevel = true;
		loadMainMenu = true;
		timeReversed = false;
		timeFrozen = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		gamePaused = !gamePaused;
		pauseMenu.GetComponent<Animator>().SetBool("GamePaused",gamePaused);
	}
}

