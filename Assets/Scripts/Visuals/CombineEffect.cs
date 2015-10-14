using UnityEngine;
using System.Collections;

public class CombineEffect : MonoBehaviour {

	private float timer = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += Vector3.one*Time.deltaTime*transform.localScale.magnitude*2f;
		transform.localScale = new Vector3(Mathf.Min(10000, transform.localScale.x),Mathf.Min(10000, transform.localScale.y),Mathf.Min(10000, transform.localScale.z));
		transform.Translate(-Vector3.forward*Time.deltaTime*50f);
		if(transform.localScale.x < 10f) {
			transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_BumpAmt", transform.localScale.x*100f);
		} else {
			transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_BumpAmt", Mathf.Max(0,128 - (transform.localScale.magnitude)/2f));
		}
		timer -= Time.deltaTime;
		if(timer <= 0f) {
			Destroy(this.gameObject);
		}
	}
}
