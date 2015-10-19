using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	public static LevelManager instance;
	public static Dictionary<PhysicsModifyable, Stack> stateStacks = new Dictionary<PhysicsModifyable, Stack>();

	public Element goalElement;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		Player player = Player.instance;
		foreach (PhysicsModifyable pM in stateStacks.Keys) {
			if(stateStacks[pM] == null) {
				stateStacks[pM] = new Stack();
			}
			Stack states = stateStacks[pM];
			PhysicsAffected pA = pM.GetComponent<PhysicsAffected>();
			
			if (player.timeScale >= 0) {
				State myState = GetState(pM, pA);
				if(states.Count > 1) {
					bool timeFrozenForBoth = player.timeScale == 0 && player.TimeElapsed == ((State) states.Peek()).timeElapsed;
					if (timeFrozenForBoth || myState.Equals((State) states.Peek())) {
						states.Pop ();
					} else {
						player.NoStateChangesThisFrame = false;
					}
				}

				states.Push (myState);
			} else {
				while(states.Count > 0 && player.TimeElapsed <= ((State) states.Peek()).timeElapsed) {
					State myState = (State) states.Pop();

					if(states.Count <= 0 || player.TimeElapsed > ((State) states.Peek()).timeElapsed) {
						setPMToState(pM, pA, myState);
					}
				}
			}
		}
	}

	private void setPMToState(PhysicsModifyable pM, PhysicsAffected pA, State state) {
		pM.gameObject.SetActive(state.active);

		if(state.active) {
			pM.Entangled = state.entangled;
			pM.Mass = state.mass;
			pM.Charge = state.charge;
			
			if(pA != null) {
				pA.Velocity = state.velocity;
				pA.AngularVelocity = state.angularVelocity;
				pA.Position = state.position;
				pA.Rotation = state.rotation;
			} else {
				pM.Position = state.position;
				pM.Rotation = state.rotation;
			}

			Goal g = pM.GetComponent<Goal>();
			if(g != null) {
				if(state.combined) {
					g.Combine();
				} else {
					g.UnCombine();
				}

				g.numElementsCombined = state.numElementsCombined;
				if(!g.Children.Equals(state.children)) {
					Stack<Goal> newChildren = new Stack<Goal>();
					while(state.children.Count > 0) {
						state.children.Peek().Combine();
						newChildren.Push(state.children.Pop());
					}
					while(g.Children.Count > 0) {
						if(!newChildren.Contains(g.Children.Peek())) {
							g.Children.Peek().UnCombine();
						}
						g.Children.Pop();
					}

					g.Children = ReverseClone(newChildren);
				}
			}

			if(state.activated.Length > 0) {
				foreach (Switch s in pM.GetComponents<Switch>()) {
					s.activated = state.activated[s.SwitchIndex];
					s.transform.FindChild("SwitchParticles" + s.SwitchIndex).gameObject.SetActive(s.activated);
				}
			}
		}
	}

	public static State GetState(PhysicsModifyable pM, PhysicsAffected pA) {
		State myState = new State();
		myState.timeElapsed = Player.instance.TimeElapsed;
		myState.active = pM.gameObject.activeSelf;
		if (pM.gameObject.activeSelf) {
			myState.mass = pM.Mass;
			myState.charge = pM.Charge;
			myState.entangled = pM.Entangled;
		
			if (pA != null) {
				myState.velocity = pA.Velocity;
				myState.angularVelocity = pA.AngularVelocity;
				myState.position = pA.Position;
				myState.rotation = pA.Rotation;
			} else {
				myState.position = pM.Position;
				myState.rotation = pM.Rotation;
			}

			if (pM.GetComponent<Goal>() != null) {
				myState.combined = pM.GetComponent<Goal>().Combined;
				myState.numElementsCombined = pM.GetComponent<Goal>().numElementsCombined;
				myState.children = Clone(pM.GetComponent<Goal>().Children);
			}
			
			Switch[] switches = pM.GetComponents<Switch>();
			myState.activated = new bool[switches.Length];
			for(int i = 0; i < switches.Length; i++) {
				Switch s = switches[i];
				myState.activated[s.SwitchIndex] = s.activated;
			}
		}
		
		return myState;
	}

	public static Stack<T> Clone<T>(Stack<T> stack) {
		return new Stack<T>(new Stack<T>(stack));
	}

	public static Stack<T> ReverseClone<T>(Stack<T> stack) {
		return new Stack<T>(stack);
	}
}
