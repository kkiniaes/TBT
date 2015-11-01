using UnityEngine;
using System.Collections;

public class PhysicsSFXManager : MonoBehaviour {

	public AudioClip[] clips;

	public static PhysicsSFXManager instance;

	private float audioRepeatTimer = 0.1f;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		audioRepeatTimer -=	Time.deltaTime;
	}

	public void PlayGravityChangeSFX(float delta) {
		if(audioRepeatTimer <= 0) {
			if(delta > 0) {
				GetComponent<AudioSource>().pitch = 1f;
			} else {
				GetComponent<AudioSource>().pitch = 0.5f;
			}
			GetComponent<AudioSource>().volume = Mathf.Abs(delta*2f);
			GetComponent<AudioSource>().PlayOneShot(clips[0]);
			audioRepeatTimer = 0.05f;
		}
	}

	public void PlayElementCombineSFX() {
		GetComponent<AudioSource>().pitch = 1f;
		GetComponent<AudioSource>().volume = 1f;
		GetComponent<AudioSource>().PlayOneShot(clips[1]);
		GetComponent<AudioSource>().pitch = 2f;
//		GetComponent<AudioSource>().volume = 1f;
		GetComponent<AudioSource>().PlayOneShot(clips[1]);
	}
}
