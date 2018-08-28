using System;
using UnityEngine;
using UnityEngine.UI;

public class CUILevelItem : MonoBehaviour {

	[SerializeField]	protected Image m_LevelBG;
	[SerializeField]	protected Image m_LevelHard;
	[SerializeField]	protected Text m_LevelText;
	[SerializeField]	protected Button m_SubmitButton;
	[SerializeField]	protected GameObject m_IsCompletedObj;

	protected string m_MapText;

	public void Setup (string bg, string name, float hard, string map, bool isCompleted, Action<string> callback) {
		Color bgColor;
		if (ColorUtility.TryParseHtmlString(bg, out bgColor)) {
			this.m_LevelBG.color = bgColor;
		}
		this.m_LevelText.text = name;
		this.m_LevelHard.fillAmount = hard;
		this.m_MapText = map;
		this.m_IsCompletedObj.SetActive(isCompleted);
		this.m_SubmitButton.onClick.RemoveAllListeners();
		this.m_SubmitButton.onClick.AddListener (() => {
			if (callback != null) {
				callback.Invoke(this.m_MapText);
			}
		});
	}
	
}
