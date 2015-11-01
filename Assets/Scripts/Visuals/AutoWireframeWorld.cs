using UnityEngine;
using System.Collections;

public class AutoWireframeWorld : MonoBehaviour {

	public Material wireframeMat;
	private GameObject childRef;
	public GameObject[] originalChildren;

	// Use this for initialization
	void Start () {

		Collider collider = GetComponent<Collider>();
		if (collider != null) {
			collider.enabled = false;
		}
		GameObject temp = (GameObject)GameObject.Instantiate(this.gameObject, transform.position, transform.rotation);
		Destroy(temp.GetComponent<AutoWireframeWorld>());
		Component[] comps = temp.GetComponents<Component>();
		for(int i = 0; i < comps.Length; i++) {
			if(!(comps[i] is MeshRenderer) && !(comps[i] is MeshFilter) && !(comps[i] is Transform)) {
				Destroy(comps[i]);
			}
		}
		foreach (Transform child in temp.transform) {
			if (child.GetComponent<PhysicsModifyable> ()) {
				child.gameObject.SetActive(false);
				Destroy (child.gameObject);
			}
		}
		temp.transform.parent = this.transform;
		if (collider != null) {
			collider.enabled = true;
		}
		temp.GetComponent<MeshRenderer>().material = wireframeMat;
		temp.gameObject.layer = 11;
		if (temp.GetComponentInChildren<LineRenderer>()) {
			temp.GetComponentInChildren<LineRenderer>().enabled = false;
		}
		if (temp.GetComponentInChildren<TextMesh>()) {
			Destroy(temp.GetComponentInChildren<TextMesh>());
		}
		temp.transform.localScale = Vector3.one;
		childRef = temp;
	}
	
	// Update is called once per frame
	void Update () {
		childRef.layer = 11;
		childRef.transform.localScale = Vector3.one;
		foreach(GameObject g in originalChildren) {
			g.GetComponent<AutoWireframeWorld>().childRef.layer = 11;
			g.GetComponent<AutoWireframeWorld>().childRef.transform.localScale = Vector3.one;
		}
	}

	public void HighlightObject () {
		childRef.layer = 0;
		childRef.transform.localScale = Vector3.one*1.05f;
		foreach(GameObject g in originalChildren) {
			g.GetComponent<AutoWireframeWorld>().childRef.layer = 0;
			g.GetComponent<AutoWireframeWorld>().childRef.transform.localScale = Vector3.one*1.05f;
		}
	}
}
