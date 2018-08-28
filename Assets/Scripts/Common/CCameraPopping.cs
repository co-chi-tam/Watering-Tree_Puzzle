using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Camera))]
public class CCameraPopping : MonoBehaviour {

	[Header("Config")]
	[SerializeField]	protected AnimationCurve m_PopCurve;
	[SerializeField]	protected float m_PopTime = 2f;

	[Header("Events")]
	public UnityEvent OnEndPop;


	protected Camera m_Camera;
	protected WaitForFixedUpdate m_WaitDelay = new WaitForFixedUpdate();
	protected float m_OrthographicSize;
	protected bool m_Pop;

	protected virtual void Awake() {
		this.m_Camera = this.GetComponent<Camera>();
		this.m_OrthographicSize = this.m_Camera.orthographicSize;
	}

	public void Pop() {
		if (this.m_Pop)
			return;
		this.m_Pop = true;
		StartCoroutine (this.HandlePopping());
	}

	public IEnumerator HandlePopping() {
		var popCounter = this.m_PopTime;
		var deltaTime = Time.deltaTime;
		var delta = deltaTime / this.m_PopTime;
		var evalate = 0f;
		while (popCounter > 0f) {
			evalate += delta;
			this.m_Camera.orthographicSize = this.m_OrthographicSize * this.m_PopCurve.Evaluate (evalate);
			yield return this.m_WaitDelay;
			popCounter -= deltaTime;
		}
		this.m_Camera.orthographicSize = this.m_OrthographicSize;
		this.m_Pop = false;
		if (this.OnEndPop != null) {
			this.OnEndPop.Invoke();
		}
	}
	
}
