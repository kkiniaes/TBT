using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour {

	private static TwinklingSFX twinkle;

	public KeyCode buttonToPress;
	public bool holdForTime;

	private float timeHeld;
	private float timeFadeOffset = 1f;

	private static float timeFadeIn = -2f;
	private bool parentController;

	public PhysicsModifyable target;

	// Use this for initialization
	void Start () {
		if(timeFadeIn == -2f) {
			transform.parent.GetComponent<CanvasGroup>().alpha = 0;
			timeFadeIn = 3f;
			parentController = true;
		}
		if(twinkle == null) {
			twinkle = GameObject.FindObjectOfType<TwinklingSFX>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(timeFadeIn < 0) {
			if(parentController) {
				transform.parent.GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(transform.parent.GetComponent<CanvasGroup>().alpha, 0.6f, Time.deltaTime/2f);
			}
			if((timeHeld >= 1f && holdForTime) || (!holdForTime && timeHeld > 0)) {
				if(timeFadeOffset == 1) {
					twinkle.PlayTwinkle();
					transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
				}
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
				if(holdForTime) {
					transform.GetChild(0).GetComponent<Image>().fillAmount = timeHeld;
				}
			}
		} else if(parentController) {
			if((target != null && target.mass > 0) || target == null) {
				timeFadeIn -= Time.deltaTime;
			}
		}


	}

	void OnLevelWasLoaded() {
		timeFadeIn = -2f;
	}
}
