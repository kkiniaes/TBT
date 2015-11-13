using UnityEngine;
using System.Collections;

public class DynamicAudioTrigger : MonoBehaviour {

	public enum DynaAudioType {
		Distance,
		Velocity,
		Mass,
	}

	public enum Comparison {
		GreaterThan,
		LessThan,
		Equal
	}

	public DynaAudioType dynamicTrigger;
	public Comparison operation;
	public GameObject obj1, obj2;
	public float value;

	public float transitionSpeed = 1f;
	public float targetVolume = 1f;
	public float minimumVolume;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!Player.instance.BeatLevel) {
			switch(dynamicTrigger) {

			case DynaAudioType.Mass:
				switch(operation) {

				case Comparison.GreaterThan:
					GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, obj1.GetComponent<PhysicsModifyable>().mass > value ? targetVolume : minimumVolume, Time.deltaTime/transitionSpeed);
					break;
				
				}
				break;
			case DynaAudioType.Velocity:
				switch(operation) {
					
				case Comparison.GreaterThan:
					GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, obj1.GetComponent<Rigidbody>().velocity.magnitude > value ? targetVolume : minimumVolume, Time.deltaTime/transitionSpeed);
					break;
					
				}
				break;
			case DynaAudioType.Distance:
					GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, Mathf.Min(1,value/(Vector3.Distance(obj1.transform.position, obj2.transform.position)+0.01f)), Time.deltaTime/transitionSpeed);
				break;

			}
		}
	}
}
