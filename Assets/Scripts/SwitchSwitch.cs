using UnityEngine;
using System.Collections.Generic;

// Toggles another switch on or off.
public class SwitchSwitch : Switch {

	// Used to prevent infinite recursion in strongly connected SwitchSwitch components.
	static List<Switch> switchChain = new List<Switch>();

	// Update is called once per frame.
	void Update () {
		if (switchChain.Count > 0) {
			switchChain.Clear ();
		}
	}
	
	// Turns the switch on or off.
	public override void Toggle () {
		if (!switchChain.Contains (this)) {
			base.Toggle ();
			foreach (Switch s in attachedObject.GetComponents<Switch> ()) {
				if (s is SwitchSwitch) {
					switchChain.Add (this);
				}
				s.Toggle ();
			}
		}
	}
}
