using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCell : MonoBehaviour {

	[SerializeField]	protected float m_X;
	public float x { 
		get { return this.m_X; }
		set { this.m_X = value; }
	}
	[SerializeField]	protected float m_Y;
	public float y { 
		get { return this.m_Y; }
		set { this.m_Y = value; }
	}
	[SerializeField]	protected float m_Z;
	public float z { 
		get { return this.m_Z; }
		set { this.m_Z = value; }
	}
	[SerializeField]	protected int m_Value;
	public int value { 
		get { return this.m_Value; }
		set { this.m_Value = value; }
	}

	protected Transform m_Transform;

	protected virtual void Awake() {
		this.m_Transform = this.transform;
	}

	public void SetPosition(Vector3 value) {
		this.m_Transform.position = value;
	}

	public Vector3 GetPosition() {
		return this.m_Transform.position;
	}

	public void SetRotation(Vector3 value) {
		this.m_Transform.rotation = Quaternion.Euler (value);
	}
	
}
