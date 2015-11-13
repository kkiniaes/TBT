using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EntangleTutorial : MonoBehaviour {

	public PhysicsModifyable aim1,aim2;

	private float fadeInTimer = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.GetSiblingIndex() == 0) {
			fadeInTimer -= Time.deltaTime;
			if((aim1.entangled != null || aim2.entangled != null)) {
				GetComponent<Image>().color = new Color(1,1,1,GetComponent<Image>().color.a - Time.deltaTime*2f);
				if(GetComponent<Image>().color.a <= 0) {
					Destroy(this.gameObject);
				}
			} else if(fadeInTimer <= 0) {
				GetComponent<Image>().color = new Color(1,1,1,GetComponent<Image>().color.a + Time.deltaTime);
			}
		}
	}
}
