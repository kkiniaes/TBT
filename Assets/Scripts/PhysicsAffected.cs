using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsAffected : MonoBehaviour {
	private static List<PhysicsModifyable> objs;

	//note: attraction is linear (stuff / radius instead of stuff / radius^2) to simplify things
	private const float G = 20f; //gravitational constant
	private const float K = 50f; //Coulomb's constant
	
	private Vector3 velocity, angularVelocity;

	public Vector3 Velocity {
		get { return velocity; }
		set { velocity = value; }
	}

	public Vector3 AngularVelocity {
		get { return angularVelocity; }
		set { angularVelocity = value; }
	}

	public Vector3 Position {
		get { return GetComponent<Rigidbody>().position; }
		set { GetComponent<Rigidbody>().position = value; }
	}
	
	public Quaternion Rotation {
		get { return GetComponent<Rigidbody>().rotation; }
		set { GetComponent<Rigidbody>().rotation = value; }
	}

	// Use this for initialization
	void Start () {
		if(objs == null) {
			objs = new List<PhysicsModifyable>();
			objs.AddRange(GameObject.FindObjectsOfType<PhysicsModifyable>());
		}
	}

	// Update is called once per frame
	void Update () {
		Player player = Player.instance;
		Rigidbody myRB = GetComponent<Rigidbody>();
		PhysicsModifyable myPM = GetComponent<PhysicsModifyable>();

		if (Mathf.Abs(player.timeScale) == 1) {
			velocity = myRB.velocity;
			angularVelocity = myRB.angularVelocity;
		}

		myRB.velocity = velocity * player.timeScale;
		myRB.angularVelocity = angularVelocity * player.timeScale;

		foreach(PhysicsModifyable pM in objs) {
			if(pM != null && pM.gameObject != this.gameObject) {

				if(player.timeScale > 0) {
					//Handles Gravity
					if(pM.gameObject.activeSelf && pM.mass > 0) {
						float forceMagnitude = player.timeScale * G * pM.mass * myRB.mass / Vector3.Distance(transform.position, pM.transform.position);
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
}
