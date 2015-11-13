using UnityEngine;
using System.Collections;

public class CreditsScroll : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Space)) {
			transform.Translate(Vector3.up*Time.deltaTime*30f, Space.Self);
		} else {
			transform.Translate(Vector3.up*Time.deltaTime*5f, Space.Self);
		}

//		if(transform.localPosition.y > 100) {
//			if(Input.GetKey(KeyCode.Space)) {
//				transform.Rotate(Vector3.up*30f*Time.deltaTime, Space.Self);
//			} else {
//				transform.Rotate(Vector3.up*5f*Time.deltaTime, Space.Self);
//			}
//		}
	}
}
