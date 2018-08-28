using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CInputRotation : MonoBehaviour {

	[SerializeField]	protected Rigidbody m_Target;
	[SerializeField]	protected float m_Speed = 2f;
	[SerializeField]	protected float m_Threahold = 0.5f;

	protected Vector3 m_LastPosition;
	protected Vector3 m_LastRotation;

	protected virtual void Awake() {
		this.m_LastRotation = this.m_Target.rotation.eulerAngles;
	}

	protected virtual void Update()
	{
		this.UpdateRotation();
	}

	public virtual void UpdateRotation() {
		if (Input.GetMouseButtonDown(0)) {
			this.m_LastPosition = Input.mousePosition;
			this.StopRotation();
		}
		if (Input.GetMouseButton(0)) {
			var direction = this.m_LastPosition - Input.mousePosition;
			if (direction.magnitude > this.m_Threahold) {
				var mousePosition = direction.normalized;
				this.UpdateRotation(-mousePosition.y, mousePosition.x, 0f, direction.magnitude * this.m_Speed);
				this.m_LastPosition = Input.mousePosition;
			} else {
				this.StopRotation();
			}
		}
	}

	public virtual void UpdateRotation(Vector3 value, float speed) {
		this.UpdateRotation(value.x, value.y, value.z, speed);
	}

	public virtual void UpdateRotation(float x, float y, float z, float speed) {
		// var newRotation = this.m_LastRotation;
		// var deltaSpeed = Time.fixedDeltaTime * speed;
		// newRotation.x += x * deltaSpeed;
		// newRotation.y += y * deltaSpeed;
		// this.m_Target.rotation = Quaternion.Lerp (
		// 	this.m_Target.rotation, 
		// 	Quaternion.Euler(newRotation), 
		// 	this.m_Threahold
		// );
		// this.m_LastRotation = newRotation;
		// this.m_Target.Rotate(Vector3.up,	y * speed * Mathf.Deg2Rad);
		// this.m_Target.Rotate(Vector3.right,	x * speed * Mathf.Deg2Rad);
		this.m_Target.AddTorque (
			x * speed, 
			y * speed, 
			z * speed, 
			ForceMode.Force
		);
	}

	public virtual void StopRotation() {
		this.m_Target.angularVelocity = Vector3.zero;
	}

}
