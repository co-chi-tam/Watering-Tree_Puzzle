using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SimpleSingleton;

public class CUIGameController : CMonoSingleton<CUIGameController> {

	[Header("Start Game Panel")] 
	[SerializeField]	protected GameObject m_StartGamePanel;

	[Header("Game Panel")] 
	[SerializeField]	protected GameObject m_GamePanel;
	[SerializeField]	protected CUICloudSelection[] m_SelectClouds;
	[SerializeField]	protected Text m_CurrentRotateText;

	[Header("End Game Panel")] 
	[SerializeField]	protected GameObject m_EndGamePanel;

	[Header("Ads Panel")] 
	[SerializeField]	protected GameObject m_AdsPanel;

	public Action OnRotateX;
	public Action OnRotateY;
	public Action OnRotateZ;

	protected override void Awake()
	{
		base.Awake();
	}

	public void OnSubmitRotateX() {
		if (this.OnRotateX != null) {
			this.OnRotateX.Invoke();
		}
	}

	public void OnSubmitRotateY() {
		if (this.OnRotateY != null) {
			this.OnRotateY.Invoke();
		}
	}

	public void OnSubmitRotateZ() {
		if (this.OnRotateZ != null) {
			this.OnRotateZ.Invoke();
		}
	}

	public void ShowStartGamePanel(bool value) {
		this.m_StartGamePanel.SetActive (value);
		this.m_GamePanel.SetActive (!value);
		this.m_EndGamePanel.SetActive (false);
	}

	public void ShowCloudPanel(List<CCloud> cloudData, Action<int> selectCloud) {
		this.m_GamePanel.SetActive (true);
		// DISABLE ALL CLOUD UI
		for (int i = 0; i < this.m_SelectClouds.Length; i++)
		{
			var ui = this.m_SelectClouds[i];
			ui.gameObject.SetActive (false);
			ui.Reset();
		}
		var minIndex = 9999;
		// ENABLE CLOUD UI BASE ID
		for (int x = 0; x < cloudData.Count; x++)
		{
			var data = cloudData[x];
			for (int i = 0; i < this.m_SelectClouds.Length; i++)
			{
				var ui = this.m_SelectClouds[i];
				if (data.index == ui.cloudIndex) {
					ui.gameObject.SetActive (true);
					if (minIndex >= data.index) {
						minIndex = data.index;
						ui.SubmitSelection(data.index, true, selectCloud);
					} else {
						ui.SubmitSelection(data.index, false, selectCloud);
					}
				} 
			}
		}
	}

	public void ShowEndGamePanel(bool value) {
		this.m_StartGamePanel.SetActive (!value);
		this.m_EndGamePanel.SetActive (value);
		this.m_GamePanel.SetActive (!value);
	}

	public void ShowAdsPanel(bool value) {
		this.m_StartGamePanel.SetActive (false);
		this.m_EndGamePanel.SetActive (false);
		this.m_AdsPanel.SetActive(value);
	}

	public void UpdateRotate(int current, int max) {
		this.m_CurrentRotateText.text = current.ToString();
	}
	
}
