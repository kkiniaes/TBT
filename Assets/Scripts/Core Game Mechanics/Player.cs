using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {
	private enum PhysicMode {
		Mass,
		Charge,
		Entangle,
	}
	private PhysicMode currentMode = PhysicMode.Mass;

	public float sensitivityX, sensitivityY;
	public float moveSpeed;
	public float timeScale = 1;

	public static Player instance;

	private float timeElapsed = 0;
	private Vector3 velocityVector;
	public bool timeFrozen;
	public bool timeReversed;
	private bool loadNextLevel;
	private float loadNextLevelTimer;
	private PhysicsModifyable entangleSelected;
	private bool wireframeMode;

	private const float REVERSE_TIME_SCALE = -3;

	private GameObject lookingAtObject;
	public GameObject LookingAtObject {
		get { return lookingAtObject; }
	}

	private bool noStateChangesThisFrame = true;
	public bool NoStateChangesThisFrame {
		get { return noStateChangesThisFrame; }
		set { noStateChangesThisFrame = value; }
	}

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

	}
	
	// Update is called once per frame
	void Update () {
		GetMovementInput();
		GetTimeManipInput();
		GetVisualModeInput();

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
		transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
		transform.Translate(velocityVector*Time.deltaTime, Space.World);
		velocityVector = Vector3.MoveTowards(velocityVector, Vector3.zero, Time.deltaTime*2f);
	
		//switching skills
		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			currentMode = PhysicMode.Mass;
		} else if(Input.GetKeyDown(KeyCode.Alpha2)) {
			currentMode = PhysicMode.Charge;
		} else if(Input.GetKeyDown(KeyCode.Alpha3)) {
			currentMode = PhysicMode.Entangle;
		}

		RaycastHit rh = new RaycastHit();
		Debug.DrawRay(transform.position, Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)).direction*10f); 
		if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)).origin,Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f)).direction, out rh, 100000, ~(1 << 10))) {
			GetComponent<DepthOfField>().focalLength = Vector3.Distance(transform.position, rh.point);
//			GetComponent<DepthOfField>().aperture = Mathf.MoveTowards(GetComponent<DepthOfField>().aperture, 10/(Vector3.Distance(transform.position, rh.point)), Time.deltaTime*10f);

			if(rh.transform.gameObject.GetComponent<PhysicsModifyable>() != null) {
				lookingAtObject = rh.transform.gameObject;

				PhysicsModifyable pM = rh.transform.gameObject.GetComponent<PhysicsModifyable>();
				rh.transform.gameObject.GetComponent<Renderer>().material.SetFloat("_Power", 0.3f);

				float delta = 10 * Input.mouseScrollDelta.y*Time.deltaTime;
				if(delta == 0) {
					delta = (Input.GetKeyDown(KeyCode.UpArrow) ? 0.5f : 0) + -1 * (Input.GetKeyDown(KeyCode.DownArrow) ? 0.5f : 0);
				}

				if(!timeReversed && !pM.immutable) {
					//Increases/Decreases mass of object
					if(!pM.specificallyImmutable.mass && currentMode == PhysicMode.Mass && pM.mass <= 6) {
						pM.Mass = Mathf.Max(0, pM.Mass + delta);
					}// Increase/Decrease charge of object 
					else if(!pM.specificallyImmutable.charge && currentMode == PhysicMode.Charge) {
						if((pM.Charge == -1 && delta > 0) || (pM.Charge == 1 && delta < 0)) {
							pM.Charge = 0;
						} else if(delta != 0) {
							pM.Charge = Mathf.Sign(delta);
						}
					}// Quantum Entangle objects
					else if(!pM.specificallyImmutable.entangled && currentMode == PhysicMode.Entangle) {
						if(Input.GetMouseButtonDown(0)) {
							if(pM.entangled != null) {
								//Debug.Log("Detangle");
								pM.entangled.Entangled = null;
								pM.Entangled = null;
								entangleSelected = null;
							} else if(entangleSelected != null) {
								//Debug.Log("Entangle " + pM + ":" + entangleSelected);
								pM.Entangled = entangleSelected;
								entangleSelected.Entangled = pM;
								entangleSelected = null;
							} else {
								//Debug.Log("Entangle " + pM);
								entangleSelected = pM;
							}
						}
					}
				}
			} else {
				lookingAtObject = null;
			}
		} else if(Input.GetMouseButtonDown(0)) {
			// handles canceling entanglement
			entangleSelected = null;
		} else {
			lookingAtObject = null;
		}


		//Loading next level with cool transition
		if(loadNextLevel) {
			loadNextLevelTimer += Time.deltaTime;
				if(loadNextLevelTimer > 2f) {

				Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 179f, Time.deltaTime*30f);
				transform.Translate(transform.forward*Time.deltaTime*Camera.main.fieldOfView/2f, Space.World);
				if(Camera.main.fieldOfView > 170) {
					Application.LoadLevel(Application.loadedLevel + 1);
				}
			}
		} else {
			Camera.main.fieldOfView = Mathf.MoveTowards(Camera.main.fieldOfView, 90f, Time.deltaTime*Camera.main.fieldOfView/3f);
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
				timeReversed = false;
				timeFrozen = !timeFrozen;
			}

			if (!timeFrozen) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					timeReversed = true;
				} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
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
			GetComponent<Camera>().nearClipPlane = Mathf.MoveTowards(GetComponent<Camera>().nearClipPlane, 50f, Time.deltaTime*(2+GetComponent<Camera>().nearClipPlane));
		} else {
			GetComponent<Camera>().nearClipPlane = Mathf.MoveTowards(GetComponent<Camera>().nearClipPlane, 0.1f, Time.deltaTime*(2+GetComponent<Camera>().nearClipPlane));
		}
		transform.FindChild("WireframeCam").GetComponent<Camera>().farClipPlane = GetComponent<Camera>().nearClipPlane;
	}

	public float TimeElapsed { get { return timeElapsed; } }

	public void LoadNextLevel() {
		loadNextLevel = true;
	}

	public bool BeatLevel {
		get {
			return loadNextLevelTimer > 2f;
		}
	}
}

