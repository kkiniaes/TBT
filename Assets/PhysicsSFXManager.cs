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
	
	// Use this for initialization
	void Start () {
		GetComponent<AudioSource>().clip = clips[2];
		if (clips[2] != null) {
			GetComponent<AudioSource>().time = clips [2].length - 0.01f;
			GetComponent<AudioSource>().pitch = -4f;
			GetComponent<AudioSource>().volume = 0.4f;
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().Play();
		}
	}
	
	// Update is called once per frame
	void Update () {
		audioRepeatTimer -=	Time.deltaTime;
		if(GetComponent<AudioSource>().clip != null
		   && GetComponent<AudioSource>().clip.Equals(clips[2])
		   && GetComponent<AudioSource>().pitch == -2f
		   && GetComponent<AudioSource>().time < 4f) {
			GetComponent<AudioSource>().Stop();
			GetComponent<AudioSource>().clip = null;
		}
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

	public void PlayWarpingSFX() {
		GetComponent<AudioSource>().clip = clips[2];
		GetComponent<AudioSource>().time = 7f;
		GetComponent<AudioSource>().pitch = 0.8f;
		GetComponent<AudioSource>().volume = 0.4f;
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
	}
}
