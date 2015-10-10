using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour {

	private static List<Goal> goals;
	private static GameObject combineEffect;

	[HideInInspector]
	public bool combined = false;
	public int numElementsCombined = 1;

	private Vector3 originalScale;

	public bool Combined {
		get { return combined; }
	}

	public int NumElementsCombined {
		get { return numElementsCombined; }
		set { 
			numElementsCombined = value;
			transform.localScale = Vector3.one*numElementsCombined;
			transform.GetChild(0).GetComponent<TextMesh>().text = System.Enum.GetNames(typeof(Element))[numElementsCombined-1];
		}
	}

	// Use this for initialization
	void Start () {
		if(goals == null) {
			goals = new List<Goal>();
			goals.AddRange(GameObject.FindObjectsOfType<Goal>());
		}
		if(combineEffect == null) {
			combineEffect = Resources.Load<GameObject>("CombineEffect");
		}

		originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(!combined) {
			foreach(Goal g in goals) {
				if(g != null && g.gameObject != null && g != this && !g.combined && Vector3.Distance(g.transform.position, this.transform.position) < 2*transform.localScale.magnitude) {
					if(g.numElementsCombined == numElementsCombined) {
						g.Combine();
						GameObject.Instantiate(combineEffect, transform.position, Quaternion.identity);
						numElementsCombined++;
						if(GetComponent<Rigidbody>() != null && g.GetComponent<Rigidbody>() != null) {
							GetComponent<Rigidbody>().velocity = (GetComponent<Rigidbody>().velocity + g.GetComponent<Rigidbody>().velocity)/2f;
						} else if(GetComponent<Rigidbody>() != null) {
							GetComponent<Rigidbody>().velocity /= 2f;
						}
					}
				}
			}

			transform.localScale = originalScale * numElementsCombined;
			transform.GetChild(0).GetComponent<TextMesh>().text = System.Enum.GetNames(typeof(Element))[numElementsCombined-1];

			if((int)LevelManager.instance.goalElement == (numElementsCombined-1)) {
				Player.instance.LoadNextLevel();
			}
		}
	}

	public void Combine() {
		if (!combined) {
			combined = true;
			gameObject.SetActive (false);
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<PhysicsModifyable> ().mass = 0;
			GetComponent<PhysicsModifyable> ().enabled = false;
			GetComponent<Collider> ().enabled = false;
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).gameObject.SetActive (false);
			}
		}
	}

	public void UnCombine() {
		if (combined) {
			combined = false;
			gameObject.SetActive (true);
			GetComponent<MeshRenderer> ().enabled = true;
			GetComponent<PhysicsModifyable> ().enabled = true;
			GetComponent<Collider> ().enabled = true;
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).gameObject.SetActive (true);
			}
		}
	}

	public static void AddGoal(Goal g) {
		goals.Add(g);
	}
}
