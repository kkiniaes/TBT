using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageTutorial : MonoBehaviour {

	public float disappearAfterTime = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.GetSiblingIndex() == 0) {
			disappearAfterTime -= Time.deltaTime;
			if(disappearAfterTime <= 0) {
				GetComponent<Image>().color = new Color(1,1,1,GetComponent<Image>().color.a-Time.deltaTime);
				if(GetComponent<Image>().color.a <= 0) {
					Destroy(this.gameObject);
				}
			} else {
				GetComponent<Image>().color = new Color(1,1,1,GetComponent<Image>().color.a+Time.deltaTime);
			}
		} else {
			GetComponent<Image>().color = new Color(1,1,1,0);
		}
	}
}
