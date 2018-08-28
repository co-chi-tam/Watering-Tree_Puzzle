using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRain : MonoBehaviour {

	[Header("Configs")]
	[SerializeField]	protected bool m_Active;
	public bool active { 
		get { return this.m_Active; } 
		set { this.m_Active = value; }
	}
	[SerializeField]	protected Transform m_RainDrop;
	[SerializeField]	protected Renderer m_RainRenderer;
	[SerializeField]	protected Vector2 m_Direction = Vector2.up;
	[SerializeField]	protected float m_RainDropSpeed = 3f;
	[SerializeField]	protected Vector3 m_CloudPosition;
	public Vector3 cloudPosition { 
		get { return this.m_CloudPosition; }
		set { this.m_CloudPosition = value; }
	}

	protected Transform m_Transform;
	protected Vector2 m_TileOffset = Vector2.zero;

	protected virtual void Awake() {
		this.m_Transform = this.transform;
	}

	protected virtual void LateUpdate() {
		if (this.m_Active) {
			this.UpdateMaterial();
		} 
	}

	public void UpdateMaterial() {
		this.m_TileOffset.x += this.m_Direction.x * Time.deltaTime * this.m_RainDropSpeed;
		this.m_TileOffset.y += this.m_Direction.y * Time.deltaTime * this.m_RainDropSpeed;
		this.m_RainRenderer.material.SetTextureOffset("_MainTex", this.m_TileOffset);
	}

	public void SetCloudMesh(Vector3 value) {
		this.m_CloudPosition = value;
		this.m_RainDrop.gameObject.SetActive(true);
		var localScale = this.m_RainDrop.localScale;
		localScale.y = Mathf.Abs (this.m_CloudPosition.y - this.m_Transform.position.y);
		this.m_RainDrop.localScale = localScale;
		var localPosition = this.m_RainDrop.localPosition;
		localPosition.y = localScale.y / 2f;
		this.m_RainDrop.localPosition = localPosition;
	}

	public void SetPosition(Vector3 value) {
		this.m_Transform.position = value;
	}

	public void SetRotation(Vector3 value) {
		this.m_Transform.rotation = Quaternion.Euler (value);
	}

	public void SetActive(bool value) {
		this.gameObject.SetActive (value);
		this.m_Active = value;
	}

	public void SetRainActive(bool value) {
		this.m_RainDrop.gameObject.SetActive(value);
	}

	public bool GetActive() {
		return this.gameObject.activeInHierarchy && this.m_Active;
	}
	
}
