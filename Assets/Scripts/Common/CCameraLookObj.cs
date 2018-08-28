using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CCameraLookObj : MonoBehaviour {

	[Header("Config")]
	[SerializeField]	protected bool m_IsActive = true;
	[SerializeField]	protected bool m_IsRotating = false;
	[SerializeField]	protected float m_HoldingTime = 0.5f;
	[SerializeField]	protected float m_RotationSpeed = 6f;
	[SerializeField]	protected float m_DeltaPosition = 0.5f;
	[SerializeField]	protected float m_DeltaRotation = 0.5f;
	[SerializeField]	protected Camera m_Camera;
	[SerializeField]	protected Transform m_Target;

	protected Vector3 m_LastMousePosition;
	protected Vector3 m_RotationMouse;

	protected virtual void Start() {
		this.m_Camera.transform.LookAt(this.m_Target);
	}

	protected virtual void Update() {
		if (this.m_IsActive) {
			// CAMERA
			if (Input.GetMouseButtonDown(0)) {
				this.m_LastMousePosition = Input.mousePosition;
#if UNITY_ANDROID
				if (Input.touchCount == 1) {
					var touch = Input.GetTouch(0);
					this.m_IsRotating = !EventSystem.current.IsPointerOverGameObject(touch.fingerId);
				}
#else
				this.m_IsRotating = !EventSystem.current.IsPointerOverGameObject();
#endif
			}
			if (Input.GetMouseButton(0)) {
				if (this.m_IsRotating) {
					this.RotationObject();
				}
			}
			if (Input.GetMouseButtonUp(0)) {
				this.m_IsRotating = false;
			}
		}
	}

	public virtual void RotationObject() {
		var direction = Input.mousePosition - this.m_LastMousePosition;
		var delta = direction.normalized;
		var speed = direction.magnitude;
		this.RotationObjectWith (delta, speed * this.m_RotationSpeed * Time.deltaTime);
	}

	public virtual void RotationObjectWith(Vector3 value, float speed) {
		this.m_Camera.transform.RotateAround(this.m_Target.position, Vector3.up, value.x * speed);
		this.m_Camera.transform.LookAt(this.m_Target);
		this.m_LastMousePosition = Input.mousePosition;
	}

}
