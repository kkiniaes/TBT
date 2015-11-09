using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelIntroText : MonoBehaviour {

	private Vector3[] initialPositions;
	private Vector3[] initialRotations;
	private float timer = 3f;

	// Use this for initialization
	void Start () {
		initialPositions = new Vector3[transform.childCount];
		initialRotations = new Vector3[transform.childCount];
		for(int i = 0; i < transform.childCount; i++) {
			initialPositions[i] = new Vector3(transform.GetChild(i).localPosition.x,transform.GetChild(i).localPosition.y, transform.GetChild(i).localPosition.z);
			initialRotations[i] = new Vector3(transform.GetChild(i).localEulerAngles.x,transform.GetChild(i).localEulerAngles.y, transform.GetChild(i).localEulerAngles.z);
			transform.GetChild(i).position += new Vector3(Random.value*20f - 10f, Random.value*20f - 10f, Random.value*20f - 10f);
			transform.GetChild(i).eulerAngles += new Vector3(Random.value*360f - 180f, Random.value*360f - 180f, Random.value*360f - 180f);
		}

	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).localPosition = Vector3.MoveTowards(transform.GetChild(i).localPosition, initialPositions[i],Time.deltaTime*(100f+Vector3.Distance(transform.GetChild(i).localPosition,initialPositions[i])));
			transform.GetChild(i).localRotation = Quaternion.RotateTowards(transform.GetChild(i).localRotation, Quaternion.Euler(initialRotations[i]),Time.deltaTime*100f);
		}
		if(timer > 0) {
			timer -= Time.deltaTime;
		} else {
			Destroy(this.gameObject);
		}
		transform.Translate(-Vector3.forward*(3f/(Mathf.Abs(timer)+0.1f))*Time.deltaTime);
	}
}
