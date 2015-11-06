using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour { 

	// Use this for initialization
	void Start () {
		Debug.Log("HERE");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseQuality(Text textObj) {
		QualitySettings.IncreaseLevel(true);
		textObj.text = Enum.GetNames(typeof(QualityLevel))[QualitySettings.GetQualityLevel()].ToString();
	}

	public void DecreaseQuality(Text textObj) {
		QualitySettings.DecreaseLevel(true);
		textObj.text = Enum.GetNames(typeof(QualityLevel))[QualitySettings.GetQualityLevel()].ToString();
	}

	public void IncreaseResolution(Text textObj) {

	}

	public void DecreaseResolution(Text textObj) {

	}

	public void ToggleMusic(Text textObj) {

	}

	public void ToggleSFX(Text textObj) {

	}
}
