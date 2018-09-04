using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUILevelSelectController : MonoBehaviour {

	[Header("Level")]
	[SerializeField]	protected string m_LevelFolder = "Levels/";
	[SerializeField]	protected Transform m_LevelItemRoot;
	[SerializeField]	protected CUILevelItem m_LevelItemPrefab;

	[Header("Utility")]
	[SerializeField]	protected CSwitchScene m_SwitchScene;

	protected CSaveLevel m_SaveLevel;

	protected virtual void Awake() {
		this.m_SaveLevel = new CSaveLevel();
	}

	protected virtual void Start() {
		this.SetupLevelDisplays();
	}

	public void SetupLevelDisplays() {
		var levelAssets = Resources.LoadAll<TextAsset>(this.m_LevelFolder);
		System.Array.Sort(levelAssets, new CMapParser.LevelComparer());
		for (int i = 0; i < levelAssets.Length; i++)
		{
			var textAsset = levelAssets[i];
			var levelItem = Instantiate(this.m_LevelItemPrefab);
			var mapDisplay = CMapParser.parseMapDisplay(textAsset.text);
			levelItem.transform.SetParent (this.m_LevelItemRoot);
			levelItem.Setup(
				mapDisplay.backgroundColor,
				string.Format("{0}", mapDisplay.name), 
				mapDisplay.hard / 5f,
				textAsset.text, 
				this.m_SaveLevel.isCompleted(mapDisplay.name),
				this.SubmitLevel
			);
			levelItem.transform.localPosition = Vector3.zero;
			levelItem.transform.localScale = Vector3.one;
		}
		this.m_LevelItemPrefab.gameObject.SetActive(false);
	}

	public void SubmitLevel(string map) {
		CPuzzleMap.MAP_ASSET = map;
		this.m_SwitchScene.LoadSceneAfterSeconds ("IslandPuzzleScene", 1f);
	}

}
