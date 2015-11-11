using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour {

	public enum Axis {
		X = 0,
		Y = 1,
		Z = 2,
	}

	public float speed = 5f;
	public Axis rotationAxis = Axis.X;
	public bool worldSpace = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(rotationAxis == Axis.X) {
			if(worldSpace) {
				transform.Rotate(new Vector3(speed*Player.instance.timeScale, 0f, 0f), Space.World);
			} else {
				transform.Rotate(new Vector3(speed*Player.instance.timeScale, 0f, 0f), Space.Self);
			}
		} else if(rotationAxis == Axis.Y) {
			if(worldSpace) {
				transform.Rotate(new Vector3(0f, speed*Player.instance.timeScale, 0f), Space.World);
			} else {
				transform.Rotate(new Vector3(0f, speed*Player.instance.timeScale, 0f), Space.Self);
			}
		} else if(rotationAxis == Axis.Z) {
			if(worldSpace) {
				transform.Rotate(new Vector3(0f, 0f, speed*Player.instance.timeScale), Space.World);
			} else {
				transform.Rotate(new Vector3(0f, 0f, speed*Player.instance.timeScale), Space.Self);
			}
		}
	}
}
