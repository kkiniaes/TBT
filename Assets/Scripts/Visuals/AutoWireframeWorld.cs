﻿using UnityEngine;
using System.Collections;

public class AutoWireframeWorld : MonoBehaviour {

	public Material wireframeMat;

	// Use this for initialization
	void Start () {
		GameObject temp = (GameObject)GameObject.Instantiate(this.gameObject, transform.position, transform.rotation);
		temp.transform.parent = this.transform;
		Destroy(temp.GetComponent<AutoWireframeWorld>());
		Component[] comps = temp.GetComponents<Component>();
		for(int i = 0; i < comps.Length; i++) {
			if(!comps[i].GetType().Equals(typeof(MeshRenderer)) && !comps[i].GetType().Equals(typeof(MeshFilter)) && !comps[i].GetType().Equals(typeof(Transform))) {
				Destroy(comps[i]);
			}
		}
		temp.GetComponent<MeshRenderer>().material = wireframeMat;
		temp.gameObject.layer = 11;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}