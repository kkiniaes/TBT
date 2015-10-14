using UnityEngine;
using System.Collections;

public class PhysicsModifyable : MonoBehaviour {
	[System.Serializable]
	public class SpecificallyImmutable {
		public bool mass;
		public bool charge;
		public bool entangled;
	}

	public bool antiMatter;
	public bool immutable;
	public SpecificallyImmutable specificallyImmutable;
	public float mass;
	public float charge;
	public PhysicsModifyable entangled;

	[HideInInspector]
	public int switchCounter = 0;
	
	private const float NEUTRALIZE_DIST = 7.5f;
	
	public Vector3 Position {
		get { return transform.position; }
		set { transform.position = value; }
	}
	
	public Quaternion Rotation {
		get { return transform.rotation; }
		set { transform.rotation = value; }
	}
	
	public float Mass { 
		get { return mass; }
		set {
			if(entangled != null) {
				entangled.mass = value;
			}
			mass = value;
		}
	}
	
	public float Charge { 
		get { return charge; }
		set {
			if(entangled != null) {
				entangled.charge = value;
			}
			charge = value;
		}
	}
	
	public PhysicsModifyable Entangled {
		get { return entangled; }
		set { entangled = value; }
	}
	
	private static GameObject gravityWell;
	private static GameObject negativeCharge;
	private static GameObject positiveCharge;
	private static GameObject lightning;
	private static GameObject blackHole;
	private static GameObject entangledFX;
	private static GameObject antiMatterExplosion;

	private float chargeLockTimer = 0;
	private bool antiMatterAnhilated;
	private Vector3 entangledOffset;

	// Use this for initialization
	void Start () {
		if(gravityWell == null) {
			gravityWell = Resources.Load<GameObject>("GravityWell");
			blackHole = Resources.Load<GameObject>("BlackHole");
			negativeCharge = Resources.Load<GameObject>("NegativeCharge");
			positiveCharge = Resources.Load<GameObject>("PositiveCharge");
			lightning = Resources.Load<GameObject>("Lightning");
			entangledFX = Resources.Load<GameObject>("EntangledFX");
			antiMatterExplosion = Resources.Load<GameObject>("AntiMatterExplosion");
		}

		Stack initStackState = new Stack();
		State initState = GetInitState (GetComponent<PhysicsAffected> ());
		initStackState.Push (initState);
		
		LevelManager.stateStacks.Add (this, initStackState);
	}

	private State GetInitState(PhysicsAffected pA) {
		State myState = new State ();
		myState.timeElapsed = 0;
		myState.active = gameObject.activeSelf;
		if (gameObject.activeSelf) {
			myState.mass = mass;
			myState.charge = charge;
			myState.entangled = entangled;
			
			if (pA != null) {
				myState.velocity = pA.Velocity;
				myState.angularVelocity = pA.AngularVelocity;
				myState.position = pA.Position;
				myState.rotation = pA.Rotation;
			} else {
				myState.position = Position;
				myState.rotation = Rotation;
			}
			
			if(GetComponent<Goal>() != null) {
				myState.combined = GetComponent<Goal>().Combined;
				myState.numElementsCombined = GetComponent<Goal>().NumElementsCombined;
			}

			Switch[] switches = GetComponents<Switch>();
			myState.activated = new bool[switches.Length];
			foreach (Switch s in switches) {
				myState.activated[s.switchIndex] = s.activated;
			}
		}
		
		return myState;
	}
	
	// Update is called once per frame
	void Update () {
		if(immutable) {
			specificallyImmutable.mass = true;
			specificallyImmutable.charge = true;
			specificallyImmutable.entangled = true;
		}

		Player player = Player.instance;

		GetComponent<Renderer>().material.SetFloat("_Power", 1f);

		if(chargeLockTimer > 0) {
			chargeLockTimer -= Time.deltaTime * player.timeScale;
			charge = 0;
		}

		if(mass == 0) {
			if(transform.FindChild("GravityWell(Clone)") != null) {
				transform.FindChild("GravityWell(Clone)").gameObject.SetActive(false);
			}
		} else if (mass < 6) {
			if(transform.FindChild("GravityWell(Clone)") != null) {
				transform.FindChild("GravityWell(Clone)").gameObject.SetActive(true);
			} else {
				GameObject temp = (GameObject)GameObject.Instantiate(gravityWell, transform.position, Quaternion.identity);
				temp.transform.parent = transform;
			}
			//following lines just handle the gravitywell particles
			transform.FindChild("GravityWell(Clone)").localRotation = Quaternion.identity;
			transform.FindChild("GravityWell(Clone)").GetComponent<ParticleSystem>().startSpeed = -3 - mass;
			transform.FindChild("GravityWell(Clone)").GetComponent<ParticleSystem>().startLifetime = 1.5f - mass/10f;
			transform.FindChild("GravityWell(Clone)").GetComponent<ParticleSystem>().emissionRate = 300 + (mass/2f);
			transform.FindChild("GravityWell(Clone)").localScale = (new Vector3(1/transform.localScale.x,1/transform.localScale.y,1/transform.localScale.z))*(1 + mass/10f);
		} else {
			mass = 6;
		}
//		else if(GetComponent<MeshRenderer>().enabled) {
//			if(transform.localScale.x < 0.1f || transform.localScale.y < 0.1f || transform.localScale.z < 0.1f) {
//				GetComponent<MeshRenderer>().enabled = false;
//				GameObject temp = (GameObject)GameObject.Instantiate(blackHole, transform.position, Quaternion.identity);
//				temp.transform.SetParent(transform, false);
//				temp.transform.localPosition = Vector3.zero;
//				transform.localScale = Vector3.one;
//				temp.transform.localScale = Vector3.one;
//				transform.FindChild("GravityWell(Clone)").gameObject.SetActive(false);
//			} else {
//				transform.localScale -= Vector3.one*Time.deltaTime;
//			}
//		}

		if(charge == 0) {
			if(transform.FindChild("NegativeCharge(Clone)") != null) {
				transform.FindChild("NegativeCharge(Clone)").gameObject.SetActive(false);
			}
			if(transform.FindChild("PositiveCharge(Clone)") != null) {
				transform.FindChild("PositiveCharge(Clone)").gameObject.SetActive(false);
			}
		} else if(charge != 0 && chargeLockTimer <= 0) {
			if(charge < 0) {
				if(transform.FindChild("PositiveCharge(Clone)") != null) {
					transform.FindChild("PositiveCharge(Clone)").gameObject.SetActive(false);
				}
				if(transform.FindChild("NegativeCharge(Clone)") != null) {
					transform.FindChild("NegativeCharge(Clone)").gameObject.SetActive(true);
				} else {
					GameObject temp = (GameObject)GameObject.Instantiate(negativeCharge, transform.position, Quaternion.identity);
					temp.transform.parent = transform;
					Vector3 tempScale = transform.FindChild("NegativeCharge(Clone)").localScale;
					float minDimension = Mathf.Min(Mathf.Min(tempScale.x, tempScale.y), tempScale.z);
					transform.FindChild("NegativeCharge(Clone)").localScale = new Vector3(minDimension, minDimension, minDimension);
				}
				transform.Find("NegativeCharge(Clone)").GetComponent<ParticleSystemRenderer>().velocityScale = Mathf.Abs(charge);
			} else {
				if(transform.FindChild("NegativeCharge(Clone)") != null) {
					transform.FindChild("NegativeCharge(Clone)").gameObject.SetActive(false);
				}
				if(transform.FindChild("PositiveCharge(Clone)") != null) {
					transform.FindChild("PositiveCharge(Clone)").gameObject.SetActive(true);
				} else {
					GameObject temp = (GameObject)GameObject.Instantiate(positiveCharge, transform.position, Quaternion.identity);
					temp.transform.parent = transform;
					Vector3 tempScale = transform.FindChild("PositiveCharge(Clone)").localScale;
					float minDimension = Mathf.Min(Mathf.Min(tempScale.x, tempScale.y), tempScale.z);
					transform.FindChild("PositiveCharge(Clone)").localScale = new Vector3(minDimension, minDimension, minDimension);
				}
				transform.Find("PositiveCharge(Clone)").GetComponent<ParticleSystemRenderer>().velocityScale = Mathf.Abs(charge);
			}

			if(player.timeScale > 0) {
				Collider[] cols = Physics.OverlapSphere(transform.position, charge*10f);
				foreach(Collider col in cols) {
					if(col.GetComponent<PhysicsModifyable>() != null && Mathf.Sign(col.GetComponent<PhysicsModifyable>().charge) != Mathf.Sign(charge)
					   && col.GetComponent<PhysicsModifyable>().charge != 0) {
						col.GetComponent<PhysicsModifyable>().NegateCharge();
						NegateCharge();
						GameObject temp = (GameObject)GameObject.Instantiate(lightning, (transform.position + col.transform.position)/2, Quaternion.LookRotation(col.transform.position - transform.position));
						temp.transform.localScale = Vector3.one*Vector3.Distance(transform.position, col.transform.position)/30f;
					}
				}
			}
		}

		if(entangled != null) {
			Transform fx = transform.FindChild("EntangledFX(Clone)");
			if(fx == null) {
				GameObject fxObject = Instantiate(entangledFX, transform.position, transform.rotation) as GameObject;
				fxObject.transform.parent = transform;
				fx = fxObject.transform;
			}
			for(int i = 0; i < 10; i++) {
//				Vector3 randVect = new Vector3(
//					(transform.position.x+entangled.transform.position.x)/2f,
//					(transform.position.y+entangled.transform.position.y)/2f,
//					(transform.position.z+entangled.transform.position.z)/2f);

				Vector3 randVect = Vector3.Lerp(transform.position, entangled.transform.position, Random.value);
				//Debug.Log(transform.position + " " + randVect + " " + entangled.transform.position);
				Vector3 inverseTransform = transform.InverseTransformPoint(randVect);
				inverseTransform.x *= transform.localScale.x;
				inverseTransform.y *= transform.localScale.y;
				inverseTransform.z *= transform.localScale.z;
				fx.GetComponent<ParticleSystem>().Emit(inverseTransform, new Vector3(Random.value - 0.5f,Random.value - 0.5f,Random.value - 0.5f), 0.5f, 0.5f, Color.white);
			}
		}

		if(player.timeReversed) {
			antiMatterAnhilated = false;
		}

	}

	public void NegateCharge() {
		charge = 0;
		chargeLockTimer = 1f;
	}

	void OnCollisionEnter(Collision other) {
		if(antiMatter && other.gameObject.GetComponent<PhysicsModifyable>() != null && !other.gameObject.GetComponent<PhysicsModifyable>().antiMatter
		   && Player.instance.timeScale > 0 && !antiMatterAnhilated) {
			GameObject.Instantiate(antiMatterExplosion,transform.position, transform.rotation);
			antiMatterAnhilated = true;
			//other.transform.position -= (transform.position - other.transform.position);
		}
	}

	
}
