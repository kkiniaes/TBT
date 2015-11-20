using UnityEngine;
using System.Collections;

public class SuperCredits_SpinningGalaxy : MonoBehaviour {

	public float speed = 2f;
	public int xAxis, yAxis, zAxis = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(xAxis*speed,yAxis*speed,zAxis*speed)*Time.deltaTime);
	}
}
