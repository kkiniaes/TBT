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
		Equal,
		Scale
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
				case Comparison.Scale:
					GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, Mathf.Max(minimumVolume, Mathf.Min(targetVolume,obj1.GetComponent<Rigidbody>().velocity.magnitude/value)), Time.deltaTime/transitionSpeed);
					break;
					
				}
				break;
			case DynaAudioType.Distance:
					Debug.Log(Vector3.Distance(obj1.transform.position, obj2.transform.position));
					GetComponent<AudioSource>().volume = Mathf.MoveTowards(GetComponent<AudioSource>().volume, Mathf.Max(minimumVolume, Mathf.Min(targetVolume,(value/(Vector3.Distance(obj1.transform.position, obj2.transform.position)+0.0001f)) - 1)), Time.deltaTime/transitionSpeed);
				break;

			}
		}
	}
}
