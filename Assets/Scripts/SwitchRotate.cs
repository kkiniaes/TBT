using UnityEngine;
using System.Collections;

// Causes the attached object to begin/stop rotating.
// Attached object must have a rotating component.
public class SwitchRotate : Switch {
	
	// The rotating component of the attached object.
	Rotating objectRotation;
	// The rotation speed of the attached object.
	float rotateSpeed = 0;

	// Use this for initialization
	void Start () {
		base.Start ();
		objectRotation = attachedObject.GetComponent<Rotating> ();
		rotateSpeed = objectRotation.speed;
		if (!activated) {
			objectRotation.speed = 0;
		}
	}

	// Update is called once per frame
	void Update () {
		float targetSpeed = activated ? rotateSpeed : 0;
		objectRotation.speed = Mathf.MoveTowards (objectRotation.speed, targetSpeed, rotateSpeed * Time.deltaTime);
	}
}
