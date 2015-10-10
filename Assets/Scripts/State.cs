using UnityEngine;

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

	//Switch State
	public bool[] activated;

	public bool Equals(State s) {
		if (!active && !s.active) {
			return true;
		}

		bool pmEqual = mass == s.mass && charge == s.charge && entangled == s.entangled;
		bool paEqual = velocity == s.velocity && angularVelocity == s.angularVelocity && position == s.position && rotation == s.rotation;
		return pmEqual && paEqual;
	}
}