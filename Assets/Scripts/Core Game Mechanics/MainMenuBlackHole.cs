﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenuBlackHole : MonoBehaviour {

	private bool mouseOver;
	private static MainMenuPlayer player;
	private static GameObject blackHolesController;
	private AsyncOperation loading;
	private string levelToLoad = "NA";

	private float timeOffset;

	// Use this for initialization
	void Start () {
		if(player == null) {
			player = GameObject.Find("Player").GetComponent<MainMenuPlayer>();
			blackHolesController = GameObject.Find("Black Holes Controller");
		}
	}
	
	// Update is called once per frame
	void Update () {
//		Debug.Log(mouseOver);

//		transform.GetChild(0).LookAt(Camera.main.transform);
//		transform.GetChild(0).localEulerAngles += new Vector3(0f,90f,0f);
//		if(Vector3.Distance(Camera.main.transform.position, transform.position) < 10) {
//			Camera.main.fieldOfView = 900/(Vector3.Distance(Camera.main.transform.position, transform.position));
//		}
		transform.GetChild(0).Rotate(transform.up*Time.deltaTime*200f);
//		transform.GetChild(0).GetComponent<Renderer>().material.SetFloat("_BumpAmt", Vector3.Distance(transform.position, Camera.main.transform.position)*4f);
//		transform.GetChild(0).Rotate(transform.forward*Time.deltaTime*200f);
		if(mouseOver) {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one, Time.deltaTime*2f);
		} else {
			transform.GetChild(1).localScale = Vector3.MoveTowards(transform.GetChild(1).localScale, Vector3.one*0.925f, Time.deltaTime*2f);
		}

		if(timeOffset > 0) {
			timeOffset += Time.deltaTime;
			if(timeOffset > 3f) {
				loading.allowSceneActivation = true;
			}
		}

		if(Input.GetKeyDown(KeyCode.Alpha1)) {
			if(gameObject.name.Contains("5")) {

			}
		}
	}

	public void OnPointerEnter() {
		mouseOver = true;
	}

	public void OnPointerExit() {
		mouseOver = false;
	}

	public void LoadLevel(int num) {
		player.GetComponent<Animator>().SetInteger("LoadLevel", num);
		blackHolesController.GetComponent<Animator>().SetInteger("LoadLevel", num);
		if(levelToLoad.Equals("NA")) {
			if(gameObject.name.Contains("5")) {
				loading = Application.LoadLevelAsync("Switches");
			} else if(gameObject.name.Contains("4")) {
				loading = Application.LoadLevelAsync("Finale");
			} else if(gameObject.name.Contains("2")) {
				loading = Application.LoadLevelAsync("WeightWatching");
			} else if(gameObject.name.Contains("1")) {
				loading = Application.LoadLevelAsync("TugOfWar");
			} else {
				loading = Application.LoadLevelAsync("IntroToGravity");
			}
		}
		loading.allowSceneActivation = false;
		timeOffset = 1;
	}

}
