using UnityEngine;
using System.Collections;

public class SuperCredits_Sun : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up*Time.deltaTime);
		transform.GetChild(0).Rotate(Vector3.up*Time.deltaTime);
	}
}
