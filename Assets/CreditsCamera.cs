using UnityEngine;
using System.Collections;

public class CreditsCamera : MonoBehaviour {

	private float timeElapsed = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(-Vector3.forward*Time.deltaTime);

		if(Input.GetKey(KeyCode.Space)) {
			transform.Rotate(Vector3.forward*Time.deltaTime*15f);
			timeElapsed += Time.deltaTime*6f;
		} else {
			transform.Rotate(Vector3.forward*Time.deltaTime*5f);
			timeElapsed += Time.deltaTime;
		}

		if(timeElapsed > 110) {
			Application.LoadLevel(0);
		}
	}
}
