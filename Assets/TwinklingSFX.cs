using UnityEngine;
using System.Collections;

public class TwinklingSFX : MonoBehaviour {

	public AudioClip[] clips;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayTwinkle() {
		GetComponent<AudioSource>().pitch = 1f;
		int index = (int)(Random.value*(clips.Length - 1));
		GetComponent<AudioSource>().PlayOneShot(clips[index], Random.Range(0.1f, 0.4f));
		AudioClip temp = clips[index];
		clips[index] = clips[clips.Length-1];
		clips[clips.Length-1] = temp;
	}

	public void PlayTwinkleLow() {
		GetComponent<AudioSource>().pitch = 0.8f;
		int index = (int)(Random.value*(clips.Length - 1));
		GetComponent<AudioSource>().PlayOneShot(clips[index], Random.Range(0.1f, 0.4f));
		AudioClip temp = clips[index];
		clips[index] = clips[clips.Length-1];
		clips[clips.Length-1] = temp;
	}
}
