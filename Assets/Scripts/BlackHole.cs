using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(Camera.main.transform.position, transform.position) < 10) {
//			Camera.main.fieldOfView = 900/(Vector3.Distance(Camera.main.transform.position, transform.position));
		}
	}
}
