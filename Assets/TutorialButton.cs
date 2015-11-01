using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour {

	public KeyCode buttonToPress;
	public bool holdForTime;

	private float timeHeld;
	private float timeFadeOffset = 1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(timeHeld >= 1f && holdForTime || (!holdForTime && Input.GetKey(buttonToPress))) {
			transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
			GetComponent<Image>().color = new Color(0,0,0,0);

			timeFadeOffset -= Time.deltaTime/2f;
			transform.GetChild(0).GetComponent<Image>().color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, timeFadeOffset);

			if(timeFadeOffset <= 0) {
				Destroy(this.gameObject);
			}
		} else {
			if(Input.GetKey(buttonToPress)) {
				timeHeld += Time.deltaTime;
			} else {
				timeHeld -= Time.deltaTime/2f;
			}
			timeHeld = Mathf.Max(0,timeHeld);
			transform.GetChild(0).GetComponent<Image>().fillAmount = timeHeld;
		}

	}
}
