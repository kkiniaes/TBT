using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	public static LevelManager instance;

	public Dictionary<PhysicsModifyable, Stack> stateStacks = new Dictionary<PhysicsModifyable, Stack>();
	public Element goalElement;

	private Vector3 avgPos = Vector3.zero;
	private float maxDist = 0;
    private float MAX_DISTANCE_MULTIPLIER = 10f;

	void Awake() {
		instance = this;
	}

    // Use this for initialization
    void Start() {
        float numObjects = 0;
        foreach(PhysicsModifyable pM in stateStacks.Keys) {
            numObjects++;
            avgPos = avgPos + pM.Position;

            int innerCount = 0;
            foreach(PhysicsModifyable pmInner in stateStacks.Keys) {
                innerCount++;
                if(innerCount > numObjects) {
                    float dist = Vector3.Distance(pM.Position, pmInner.Position) * MAX_DISTANCE_MULTIPLIER;
                    if(dist > maxDist) {
                        maxDist = dist;
                    }
                }
            }

            float playerDist = Vector3.Distance(pM.Position, Player.instance.transform.position) * MAX_DISTANCE_MULTIPLIER;
            if(playerDist > maxDist) {
                maxDist = playerDist;
            }
        }

        if(stateStacks.Keys.Count > 0) {
            avgPos /= numObjects;
        } else {
            avgPos = Player.instance.transform.position;
        }
	}

    public bool inBounds(Vector3 pos) {
        return Vector3.Distance(pos, avgPos) <= maxDist;
    }

    public Vector3 reflect(Vector3 pos) {
        if(!inBounds(pos)) {
            return pos + (Vector3.Distance(pos, avgPos) + maxDist) * Vector3.Normalize(avgPos - pos);
        } else {
            return pos;
        }
    }

	// Update is called once per frame
	void Update () {
		Player player = Player.instance;
		foreach (PhysicsModifyable pM in stateStacks.Keys) {
			if(stateStacks[pM] == null) {
				stateStacks[pM] = new Stack();
			}

			if(pM == null) {
				return;
			}

			Stack states = stateStacks[pM];
			PhysicsAffected pA = pM.GetComponent<PhysicsAffected>();
			
			if (player.timeScale >= 0) {
				State myState = State.GetState(pM);
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
			}

			pM.Position = state.position;
			pM.Rotation = state.rotation;

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
					// s.transform.FindChild("SwitchParticles" + s.SwitchIndex).gameObject.SetActive(s.activated);
				}
			}
		}
	}

	public static Stack<T> Clone<T>(Stack<T> stack) {
		return new Stack<T>(new Stack<T>(stack));
	}

	public static Stack<T> ReverseClone<T>(Stack<T> stack) {
		return new Stack<T>(stack);
	}
}
