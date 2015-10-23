using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BlackHole : MonoBehaviour {

	private bool mouseOver;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(mouseOver);

//		transform.GetChild(0).LookAt(Camera.main.transform);
//		transform.GetChild(0).localEulerAngles += new Vector3(0f,90f,0f);
//		if(Vector3.Distance(Camera.main.transform.position, transform.position) < 10) {
//			Camera.main.fieldOfView = 900/(Vector3.Distance(Camera.main.transform.position, transform.position));
//		}
		transform.GetChild(0).Rotate(transform.up*Time.deltaTime*200f);
//		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_BumpAmt", Vector3.Distance(transform.position, Camera.main.transform.position)*4f);
//		transform.GetChild(0).Rotate(transform.forward*Time.deltaTime*200f);
		if(mouseOver) {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one, Time.deltaTime*2f);
		} else {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one*0.925f, Time.deltaTime*2f);
		}
	}

	public void OnPointerEnter() {
		mouseOver = true;
	}

	public void OnPointerExit() {
		mouseOver = false;
	}

}
