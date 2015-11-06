using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour { 

	int resolutionIndex;

	public AudioMixer audioMixer;

	public Text qualityText, resolutionText, musicText, sfxText;

	// Use this for initialization
	void Start () {
		resolutionIndex = Array.FindIndex(Screen.resolutions, var => (var.height == Screen.currentResolution.height && var.width == Screen.currentResolution.width));
		qualityText.text = Enum.GetNames(typeof(QualityLevel))[QualitySettings.GetQualityLevel()].ToString();
		resolutionText.text = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
		float volume = 0f;
		audioMixer.GetFloat("MusicVolume", out volume);
		if(volume == 0f) {
			musicText.text = "•";
		} else {
			musicText.text = "";
		}
		audioMixer.GetFloat("SFXVolume", out volume);
		if(volume == 0f) {
			sfxText.text = "•";
		} else {
			sfxText.text = "";
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseQuality() {
		QualitySettings.IncreaseLevel(true);
		qualityText.text = Enum.GetNames(typeof(QualityLevel))[QualitySettings.GetQualityLevel()].ToString();
	}

	public void DecreaseQuality() {
		QualitySettings.DecreaseLevel(true);
		qualityText.text = Enum.GetNames(typeof(QualityLevel))[QualitySettings.GetQualityLevel()].ToString();
	}

	public void IncreaseResolution() {
		if(resolutionIndex < Screen.resolutions.Length-1) {
			resolutionIndex++;
		}
		int width = Screen.resolutions[resolutionIndex].width;
		int height = Screen.resolutions[resolutionIndex].height;
		Screen.SetResolution(width,height,Screen.fullScreen);
		resolutionText.text = width + "x" + height;
	}

	public void DecreaseResolution() {
		if(resolutionIndex > 0) {
			resolutionIndex--;
		}
		int width = Screen.resolutions[resolutionIndex].width;
		int height = Screen.resolutions[resolutionIndex].height;
		Screen.SetResolution(width,height,Screen.fullScreen);
		resolutionText.text = width + "x" + height;
	}

	public void ToggleMusic() {
		float volume = 0f;
		audioMixer.GetFloat("MusicVolume", out volume);
		if(volume == 0f) {
			audioMixer.SetFloat("MusicVolume", -80f);
			musicText.text = "";
		} else {
			audioMixer.SetFloat("MusicVolume", 0f);
			musicText.text = "•";
		}
	}

	public void ToggleSFX() {
		float volume = 0f;
		audioMixer.GetFloat("SFXVolume", out volume);
		if(volume == 0f) {
			audioMixer.SetFloat("SFXVolume", -80f);
			sfxText.text = "";
		} else {
			audioMixer.SetFloat("SFXVolume", 0f);
			sfxText.text = "•";
		}
	}
}
