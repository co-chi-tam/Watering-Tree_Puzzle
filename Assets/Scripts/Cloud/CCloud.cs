using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCloud : MonoBehaviour {

	#region Fields

	[Header("Cloud")]
	[SerializeField]	protected GameObject m_CloudPrefab;
	[SerializeField]	protected Vector3 m_Spot;
	public Vector3 spot { 
		get { return this.m_Spot; }
		set { this.m_Spot = value; }
	}
	[SerializeField]	protected Vector3 m_Move;
	public Vector3 move { 
		get { return this.m_Move; }
		set { this.m_Move = value; }
	}
	[SerializeField]	protected int m_Index;
	public int index { 
		get { return this.m_Index; }
		set { this.m_Index = value; }
	}

	protected Transform m_Transform;
	protected int[,,] m_CloudGrid;
	public int[,,] cloudGrid { 
		get { return this.m_CloudGrid; }
		protected set { this.m_CloudGrid = value; } 
	}
	
	protected Vector3 m_PositionSpot;
	protected Quaternion m_RotationSpot;
	private WaitForFixedUpdate m_Wait = new WaitForFixedUpdate();
	private bool m_IsMoveHandling = false;
	private bool m_IsRotationHandling = false;

	#endregion

	#region Implementation Monobehaviour

	protected virtual void Awake() {
		this.m_Transform = this.transform;
	}

	protected virtual void Start() {
		
	}

	#endregion

	#region Generate cloud

	public virtual string GenerateRandom3x3Cloud() {
		var newGrid = new int[,,] {
			{{0, 1, 0}, {0, 0, 0}, {0, 1, 0}},
			{{0, 0, 0}, {0, 1, 0}, {0, 0, 0}},
			{{0, 1, 0}, {0, 0, 0}, {0, 1, 0}}
		};
		int i, randomX, randomY, randomZ, x, y, z;
		for (z = 0; z < newGrid.GetLength(2); z++)
		{
			for (y = 0; y < newGrid.GetLength(1); y++)
			{
				for (x = 0; x < newGrid.GetLength(0); x++)
				{
					randomX = UnityEngine.Random.Range(0, newGrid.GetLength(0));
					randomY = UnityEngine.Random.Range(0, newGrid.GetLength(1));
					randomZ = UnityEngine.Random.Range(0, newGrid.GetLength(2));
					i = newGrid[x, y, z];
					newGrid[x, y, z] = newGrid[randomX, randomY, randomZ];
					newGrid[randomX, randomY, randomZ] = i;
				}
			}
		}
		this.GenerateCloud (newGrid);
		return TinyJSON.JSON.Dump(newGrid);
	}

	public virtual string GenerateRandom3x3WithRate(int rate) {
		var newGrid = new int[,,] {
			{{0, 0, 0}, {0, 0, 0}, {0, 0, 0}},
			{{0, 0, 0}, {0, 0, 0}, {0, 0, 0}},
			{{0, 0, 0}, {0, 0, 0}, {0, 0, 0}}
		};
		int i, randomX, randomY, randomZ, x, y, z;
		for (z = 0; z < newGrid.GetLength(2); z++)
		{
			for (y = 0; y < newGrid.GetLength(1); y++)
			{
				for (x = 0; x < newGrid.GetLength(0); x++)
				{
					// RANDOM VALUE
					var random = UnityEngine.Random.Range (0, 100);
					newGrid[x, y, z] = random < rate ? 1 : 0;
					// SHUFFLE
					randomX = UnityEngine.Random.Range(0, newGrid.GetLength(0));
					randomY = UnityEngine.Random.Range(0, newGrid.GetLength(1));
					randomZ = UnityEngine.Random.Range(0, newGrid.GetLength(2));
					i = newGrid[x, y, z];
					newGrid[x, y, z] = newGrid[randomX, randomY, randomZ];
					newGrid[randomX, randomY, randomZ] = i;
				}
			}
		}
		this.GenerateCloud (newGrid);
		return TinyJSON.JSON.Dump(newGrid);
	}

	public virtual void GenerateCloud(int[,,] maps) {
		this.ResetCloudObject();
		var center	= this.m_Transform == null ? this.transform.position : this.m_Transform.position;
		center.x 	= (int) (maps.GetLength(0) / 2f);
		center.y 	= (int) (maps.GetLength(1) / 2f);
		center.z 	= (int) (maps.GetLength(2) / 2f);
		this.m_CloudGrid = new int[maps.GetLength(0), maps.GetLength(1), maps.GetLength(2)];
		Buffer.BlockCopy(maps, 0, this.m_CloudGrid, 0, maps.Length * sizeof(int));
		var random = (int) Time.time;
		int x, y, z;
		for (z = 0; z < this.m_CloudGrid.GetLength(2); z++)
		{
			for (y = 0; y < this.m_CloudGrid.GetLength(1); y++)
			{
				for (x = 0; x < this.m_CloudGrid.GetLength(0); x++)
				{
					var value = this.m_CloudGrid[x, y, z];
					if (value > 0) {
						var cloud = Instantiate(this.m_CloudPrefab);
						cloud.transform.SetParent(this.m_Transform == null ? this.transform : this.m_Transform);
						cloud.transform.localPosition = new Vector3(center.x - x, center.y - y, center.z - z);
					}
				}
			}
		}
	}

	public void ResetCloudObject() {
		var parent = this.m_Transform == null ? this.transform : this.m_Transform;
		for (int i = 0; i < parent.childCount; i++)
		{
			var child = parent.GetChild(i);
			DestroyImmediate (child.gameObject);
			i--;
		}
	}

	#endregion

	#region Rotation

	public void Rotation(Vector3 value) {
		this.Rotation(value, null, null);
	}

	public void Rotation(Vector3 value, Action wait, Action callback) {
		if (this.m_IsRotationHandling)
			return;
		this.m_IsRotationHandling = true;
		var parent = this.m_Transform == null ? this.transform: this.m_Transform;
		var transAxis = parent.InverseTransformDirection(value);
		var intAxis = transAxis;
		intAxis.x = Mathf.Round(intAxis.x / 90f) * 90f;
		intAxis.y = Mathf.Round(intAxis.y / 90f) * 90f;
		intAxis.z = Mathf.Round(intAxis.z / 90f) * 90f;
		this.m_RotationSpot = parent.rotation * Quaternion.Euler (intAxis);
		StartCoroutine (this.HandleRotation(this.m_RotationSpot, wait, callback));
	}

	private IEnumerator HandleRotation(Quaternion value, Action wait, Action callback) {
		if (wait != null) {
			wait.Invoke();
		}
		var time = 0f;
		var limit = 0.2f;
		var deltaTime = Time.fixedDeltaTime;
		var timeDivLimit = 0f;
		while(time < limit) {
			yield return this.m_Wait;
			timeDivLimit =  time / limit;
			this.m_Transform.rotation = Quaternion.Lerp (this.m_Transform.rotation, value, timeDivLimit);
			time += deltaTime;
		}
		this.m_Transform.rotation = value;
		yield return this.m_Wait;
		this.m_IsRotationHandling = false;
		if (callback != null) {
			callback.Invoke();
		}
	}

	#endregion

	#region Move

	public void Move(Vector3 value) {
		this.Move(value, null, null);
	}

	public void Move(Vector3 value, Action wait, Action callback) {
		if (this.m_IsMoveHandling)
			return;
		this.m_IsMoveHandling = true;
		this.m_PositionSpot = value;
		StartCoroutine (this.HandleMove(this.m_PositionSpot, wait, callback));
	}

	private IEnumerator HandleMove(Vector3 value, Action wait, Action callback) {
		if (wait != null) {
			wait.Invoke();
		}
		var time = 0f;
		var limit = 0.2f;
		var deltaTime = Time.fixedDeltaTime;
		var timeDivLimit = 0f;
		while(time < limit) {
			yield return this.m_Wait;
			timeDivLimit =  time / limit;
			this.m_Transform.localPosition = Vector3.Lerp (this.m_Transform.localPosition, value, timeDivLimit);
			time += deltaTime;
		}
		this.m_Transform.localPosition = value;
		yield return this.m_Wait;
		this.m_IsMoveHandling = false;
		if (callback != null) {
			callback.Invoke();
		}
	}

	#endregion

	#region Combine

	public void Combine(Vector3 move, Vector3 rotation, Action wait, Action callback) {
		this.Rotation (rotation, null, () => {
			this.Move (move, wait, callback);
		});
		// this.Move (move, null, () => {
		// 	this.Rotation (rotation, wait, callback);
		// });
	}

	#endregion

	#region Getter and Setter

	public void SetPosition(Vector3 value) {
		this.m_Transform.position = value;
	}

	public void SetRotation(Vector3 value) {
		this.m_Transform.rotation = Quaternion.Euler (value);
	}

	public string GetRotationStr() {
		var rotate = this.transform.rotation.eulerAngles;
		return string.Format("{0},{1},{2}", 
			Mathf.RoundToInt (rotate.x), 
			Mathf.RoundToInt (rotate.y), 
			Mathf.RoundToInt (rotate.z)
		);
	}

	#endregion

}
