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

	// Must occur before time starts being logged.
	void Awake () {
		start = attachedObject.transform.position;
		if (activated) {
			attachedObject.transform.position = end;
		}
	}

	// Use this for initialization
	new void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	new void Update () {
		base.Update ();
		if (Player.instance.timeScale > 0) {
			Vector3 destination = activated ? end : start;
			attachedObject.transform.position = Vector3.MoveTowards (attachedObject.transform.position, destination, speed * Time.deltaTime);
		}
		LineRenderer line = gameObject.transform.FindChild ("LineRenderer" + SwitchIndex).GetComponent<LineRenderer> ();
		line.SetPosition (1, attachedObject.transform.position);
	}
}
