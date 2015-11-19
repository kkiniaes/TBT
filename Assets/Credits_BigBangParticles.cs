using UnityEngine;
using System.Collections;

public class Credits_BigBangParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<ParticleSystem>().playbackSpeed -= Time.deltaTime/20f;
		transform.GetChild(0).GetComponent<ParticleSystem>().playbackSpeed -= Time.deltaTime/20f;
	}
}
