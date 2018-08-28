using System;
using UnityEngine;
using UnityEngine.UI;

public class CUICloudSelection : MonoBehaviour {
	
	[SerializeField]	protected int m_CloudIndex = 0; 
	public int cloudIndex { 
		get { return this.m_CloudIndex; } 
		set { this.m_CloudIndex = value; }
	}
	[SerializeField]	protected Toggle m_SelectButton;
	[SerializeField]	protected int m_SelectIndex; 

	public void SubmitSelection(int index, bool isSelected, Action<int> selectCloud) {
		this.m_SelectIndex = index;
		this.m_SelectButton.isOn = isSelected;
		this.m_SelectButton.onValueChanged.RemoveAllListeners();
		this.m_SelectButton.onValueChanged.AddListener ((value) => {
			if (selectCloud != null) {
				selectCloud.Invoke(this.m_SelectIndex);
			}
		});
	}

	public void Reset() {
		this.m_SelectIndex = -1;
		this.m_SelectButton.onValueChanged.RemoveAllListeners();
	}
	
}
