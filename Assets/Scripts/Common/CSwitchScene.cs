﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CSwitchScene : MonoBehaviour {

	public static bool onLoadScene = false;

	protected virtual void Awake() {
		SceneManager.activeSceneChanged += (arg0, arg1) => {
			onLoadScene = false;
		};
	}

	public void LoadScene(string name) {
		if (onLoadScene)
			return;
		SceneManager.LoadScene (name);
		onLoadScene = true;
	}

	public void LoadSceneAfterSeconds(string name, float time) {
		if (onLoadScene)
			return;
		StartCoroutine (this.HandleAfterTime(name, time));
		onLoadScene = true;
	}

	public void LoadSceneAfter3Seconds(string name) {
		if (onLoadScene)
			return;
		StartCoroutine (this.HandleAfterTime(name, 3f));
		onLoadScene = true;
	}

	protected IEnumerator HandleAfterTime(string name, float time) {
		yield return new WaitForSeconds (time);
		SceneManager.LoadScene (name);
	}

}
