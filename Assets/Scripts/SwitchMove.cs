using UnityEngine;
using System.Collections;

// Moves the attached object to another location when switched on.
// Should not be used with PhysicsAffected objects.
public class SwitchMove : Switch {

	// The initial position of the attached object and where it returns to when the switch is turned off.
	Vector3 start;
	// The destination position of the attached object when the switch is turned on.
	public Vector3 end;
	// The speed at which the object will move.
	public float speed;

	// Use this for initialization
	void Start () {
		base.Start ();
		start = attachedObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 destination = activated ? end : start;
		attachedObject.transform.position = Vector3.MoveTowards (attachedObject.transform.position, destination, speed * Time.deltaTime);
		GetComponent<LineRenderer> ().SetPosition (1, attachedObject.transform.position);
	}
}
