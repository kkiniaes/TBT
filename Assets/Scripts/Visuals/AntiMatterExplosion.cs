using UnityEngine;
using System.Collections;

public class AntiMatterExplosion : MonoBehaviour {

	private bool destroyMe;

	public static bool exists = false;

	// Use this for initialization
	void Start () {
		exists = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(!Player.instance.timeFrozen) {
			if(Player.instance.timeScale < 0) {
				transform.localScale -= Vector3.one*Time.deltaTime;
				if(transform.localScale.x <= 0) {
					DestroyExplosion();
				}
			} else if(!destroyMe) {
				transform.FindChild("Flare").GetComponent<LensFlare>().brightness -= Time.deltaTime/2f;
				Player.instance.transform.eulerAngles += (new Vector3((Random.value*2f - 1f), (Random.value*2f - 1f), (Random.value*2f - 1f)))*transform.FindChild("Flare").GetComponent<LensFlare>().brightness/2f;
				Player.instance.transform.position += (new Vector3((Random.value*2f - 1f), (Random.value*2f - 1f), (Random.value*2f - 1f)))*transform.FindChild("Flare").GetComponent<LensFlare>().brightness/4f;
				if(transform.FindChild("Flare").GetComponent<LensFlare>().brightness <= 0) {
					Player.instance.timeReversed = true;
					destroyMe = true;
	//				Destroy(this.gameObject);
				}
			} else {
				transform.localScale -= Vector3.one*Time.deltaTime;
				if(transform.localScale.x <= 0) {
					DestroyExplosion();
				}
			}
		}
	}

	// Removes the explosion from the scene.
	void DestroyExplosion() {
		Destroy(this.gameObject);
		exists = false;
	}
}
