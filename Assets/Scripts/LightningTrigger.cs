using UnityEngine;
using System.Collections;

// Causes effects when lightning hits certain objects.
public class LightningTrigger : MonoBehaviour {

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Switch") {
			foreach (Switch s in other.gameObject.GetComponents<Switch> ()) {
				s.Toggle ();
			}
		}
	}
}
