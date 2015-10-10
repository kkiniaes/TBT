using UnityEngine;
using System.Collections;

public class TimeControlledParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<ParticleSystem>().playbackSpeed = Player.instance.timeScale;
	}
}
