using UnityEngine;
using System.Collections;

public class AudioLooper : MonoBehaviour {

	public float startingPoint,loopPoint;

	private float startingPitch;

	// Use this for initialization
	void Start () {
		startingPitch = GetComponent<AudioSource>().pitch;
	}
	
	// Update is called once per frame
	void Update () {


		if(Player.instance.BeatLevel) {
//			GetComponent<AudioSource>().pitch = Mathf.MoveTowards(GetComponent<AudioSource>().pitch, 3f, Time.deltaTime/3f);
			GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, 0f, Time.deltaTime/5f);
		} else {
			GetComponent<AudioSource>().pitch = Mathf.Clamp(Player.instance.timeScale, -1f, 1f)*startingPitch;
			if(GetComponent<AudioSource>().pitch > 0) {
				if(GetComponent<AudioSource>().time > loopPoint) {
					GetComponent<AudioSource>().time = startingPoint;
				}
			} else {
				if(GetComponent<AudioSource>().time < startingPoint && loopPoint < GetComponent<AudioSource>().clip.length) {
					GetComponent<AudioSource>().time = loopPoint;
				} else if (GetComponent<AudioSource>().time < startingPoint) {
					GetComponent<AudioSource>().time = GetComponent<AudioSource>().clip.length;
				}
			}
		}

	}
}
