using UnityEngine;
using System.Collections;

// Affects a certain object when turned on or off. Triggered by lightning.
public class Switch : MonoBehaviour {

	// The object affected when the switch is triggered.
	public PhysicsModifyable attachedObject;
	// Whether the switch is on or off.
	public bool activated = false;

	// Use this for initialization
	protected void Start () {
		// Draw a line between the switch and its attached object.
		GetComponent<LineRenderer> ().SetPosition (0, transform.position);
		GetComponent<LineRenderer> ().SetPosition (1, attachedObject.transform.position);
		transform.FindChild ("SwitchParticles").gameObject.SetActive (activated);
	}

	// Turns the switch on or off.
	public void Toggle () {
		activated = !activated;
		transform.FindChild ("SwitchParticles").gameObject.SetActive (activated);
	}
}
