using UnityEngine;
using System.Collections.Generic;

public class State {
	//General State
	public float timeElapsed;
	public bool active;
	public Vector3 position;
	public Quaternion rotation;
	
	//Physics Modifyable State
	public float mass;
	public float charge;
	public PhysicsModifyable entangled;
	
	//Physics Affected State
	public Vector3 velocity;
	public Vector3 angularVelocity;

	//Goal State
	public bool combined;
	public int numElementsCombined;
	public Stack<Goal> children;

	//Switch State
	public bool[] activated;

	public bool Equals(State s) {
		if (!active && !s.active) {
			return true;
		}

		bool pmEqual = mass == s.mass && charge == s.charge && entangled == s.entangled;
		bool paEqual = velocity == s.velocity && angularVelocity == s.angularVelocity && position == s.position && rotation == s.rotation;
		bool goalEqual = combined == s.combined && numElementsCombined == s.numElementsCombined;
		bool switchEqual = false;
		if (activated != null) {
			for (int i = 0; i < activated.Length; i++) {
				switchEqual = switchEqual || activated [i] == s.activated [i];
			}
		}
		return pmEqual && paEqual && goalEqual && switchEqual;
	}

	public static State GetState(PhysicsModifyable pM) {
		State myState = new State ();
		myState.timeElapsed = Player.instance.TimeElapsed;
		myState.active = pM.gameObject.activeSelf;
		if (pM.gameObject.activeSelf) {
			myState.mass = pM.Mass;
			myState.charge = pM.Charge;
			myState.entangled = pM.Entangled;

			PhysicsAffected pA = pM.GetComponent<PhysicsAffected>();
			if (pA != null) {
				myState.velocity = pA.Velocity;
				myState.angularVelocity = pA.AngularVelocity;
				myState.position = pA.Position;
				myState.rotation = pA.Rotation;
			} else {
				myState.position = pM.Position;
				myState.rotation = pM.Rotation;
			}

			Goal g = pM.GetComponent<Goal>();
			if(g != null) {
				myState.combined = g.Combined;
				myState.numElementsCombined = g.numElementsCombined;
				myState.children = LevelManager.Clone(g.Children);
			}
			
			Switch[] switches = pM.GetComponents<Switch>();
			myState.activated = new bool[switches.Length];
			foreach (Switch s in switches) {
				myState.activated[s.SwitchIndex] = s.activated;
			}
		}
		
		return myState;
	}
}