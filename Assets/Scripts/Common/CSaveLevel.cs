using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSaveLevel {

	public const string LEVEL_SAVED = "LEVEL_SAVED";
	public string levelSavedPrefab;
	public string[] levelSaved;

	public CSaveLevel() {
		this.levelSavedPrefab = PlayerPrefs.GetString(LEVEL_SAVED, string.Empty);
		this.levelSaved = this.levelSavedPrefab.Split(',');
	}

	public void Save(string value) {
		if (string.IsNullOrEmpty(this.levelSavedPrefab)) {
			this.levelSavedPrefab = string.Format("{0}", value);
		} else {
			this.levelSavedPrefab = string.Format("{0},{1}", this.levelSavedPrefab, value);
		}
		this.levelSaved = this.levelSavedPrefab.Split(',');
		PlayerPrefs.SetString(LEVEL_SAVED, this.levelSavedPrefab);
		PlayerPrefs.Save();
	}

	public string[] Get() {
		var prefab = new string[this.levelSaved.Length];
		System.Array.Copy(this.levelSaved, prefab, this.levelSaved.Length);
		return prefab;
	}

	public bool isCompleted(string name) {
		return System.Array.IndexOf (this.levelSaved, name) != -1;
	}

}
