using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsAffected : MonoBehaviour {
	public static List<PhysicsModifyable> objs = new List<PhysicsModifyable>();

	//note: attraction is linear (stuff / radius instead of stuff / radius^2) to simplify things
	private const float G = 20f; //gravitational constant

	private Vector3 velocity;
	public Vector3 Velocity {
		get { return velocity; }
		set { 
			velocity = value; 
			GetComponent<Rigidbody>().velocity = value;
		}
	}

	private Vector3 angularVelocity;
	public Vector3 AngularVelocity {
		get { return angularVelocity; }
		set { 
			angularVelocity = value; 
			GetComponent<Rigidbody>().angularVelocity = value;
		}
	}

	public Vector3 Position {
		get { return GetComponent<Rigidbody>().position; }
		set { GetComponent<Rigidbody>().position = value; }
	}
	
	public Quaternion Rotation {
		get { return GetComponent<Rigidbody>().rotation; }
		set { GetComponent<Rigidbody>().rotation = value; }
	}
	
	public float Inertia {
		get { return GetComponent<Rigidbody>().mass; }
		set { GetComponent<Rigidbody>().mass = value; }
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		Rigidbody myRB = GetComponent<Rigidbody>();
		//PhysicsModifyable myPM = GetComponent<PhysicsModifyable>();

		if (Mathf.Abs(Player.instance.timeScale) == 1) {
			velocity = myRB.velocity;
			angularVelocity = myRB.angularVelocity;
		}

		myRB.velocity = velocity * Player.instance.timeScale;
		myRB.angularVelocity = angularVelocity * Player.instance.timeScale;

		foreach(PhysicsModifyable pM in objs) {
			if(pM != null && pM.gameObject != this.gameObject) {

				if(Player.instance.timeScale > 0) {
					//Handles Gravity
					if(pM.gameObject.activeSelf && pM.mass > 0 && pM.antiMatter) {
						float forceMagnitude = -Player.instance.timeScale * G * pM.mass * myRB.mass / Vector3.Distance(transform.position, pM.transform.position);
						myRB.AddForce(Vector3.Normalize(pM.transform.position - transform.position) * forceMagnitude);
						
						if(pM.GetComponent<PhysicsAffected>() != null) {
							pM.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - pM.transform.position) * forceMagnitude);
						}
					} else if(pM.gameObject.activeSelf && pM.mass > 0) {
						float forceMagnitude = Player.instance.timeScale * G * pM.mass * myRB.mass / Vector3.Distance(transform.position, pM.transform.position);
						myRB.AddForce(Vector3.Normalize(pM.transform.position - transform.position) * forceMagnitude);

						if(pM.GetComponent<PhysicsAffected>() != null) {
							pM.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.position - pM.transform.position) * forceMagnitude);
						}
						//Handles Spaghettification around black holes (probably will end up removing)
//						if(pM.mass >= 6) {
//							transform.LookAt(pM.transform);
//							transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + 1/Vector3.Distance(transform.position, pM.transform.position));
//						}
					}
				}
			}
		}
	}

	void OnLevelWasLoaded() {
		objs.Clear();
	}

	public static void TryAddPM(PhysicsModifyable pM) {
		if(!objs.Contains(pM)) {
			objs.Add(pM);
		}
	}
}
