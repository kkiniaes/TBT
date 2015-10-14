using UnityEngine;
using System.Collections;

// Changes a physics attribute of the attached object when turned on.
public class SwitchPhysics : Switch {

	// Possible attributes to modify.
	public enum SwitchPhysicsMode {
		Mass,
		Charge
	}

	// The physics attribute modified by the switch.
	public SwitchPhysicsMode mode;
	// The value that the attribute will become when the switch is turned on.
	public float target;
	// The attached object's physics.
	PhysicsModifyable objectPhysics;

	// Use this for initialization
	new void Start () {
		base.Start ();
		objectPhysics = attachedObject.GetComponent<PhysicsModifyable> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Player.instance.timeScale > 0) {
			switch (mode) {
			case SwitchPhysicsMode.Mass:
				objectPhysics.mass = activated ? target : 0;
				break;
			case SwitchPhysicsMode.Charge:
				objectPhysics.charge = activated ? target : 0;
				break;
			}
		}
	}
}
