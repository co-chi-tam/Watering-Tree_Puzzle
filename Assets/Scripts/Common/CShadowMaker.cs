using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(CameraTransformation))]
[RequireComponent(typeof(PositionTransformation))]
[RequireComponent(typeof(RotationTransformation))]
public class CShadowMaker : MonoBehaviour {

	[Header("Object")]
	[SerializeField]	protected GameObject m_Root;
	[SerializeField]	protected MeshFilter m_MeshRoot;

	protected MeshFilter m_MeshFilter;
	protected CameraTransformation m_CameraTransform;
	protected PositionTransformation m_Position;
	protected RotationTransformation m_Rotation;
	protected Transformation[] transformations;
	protected Matrix4x4 transformation;
	protected Vector3[] m_Vertices;

	protected virtual void Awake() {
		this.m_MeshFilter = this.GetComponent<MeshFilter>();
		this.m_CameraTransform = this.GetComponent<CameraTransformation>();
		this.transformation = this.m_CameraTransform.Matrix;
		this.m_Position = this.GetComponent<PositionTransformation>();
		this.m_Rotation = this.GetComponent<RotationTransformation>();
		this.transformations = this.GetComponents<Transformation>();
	}

	protected virtual void Start() {
		this.GenerateNewMesh();
	}

	protected virtual void LateUpdate() {
		UpdateParamater();
		UpdateTransformation();
		UpdateVerticles();
	}

	public virtual void GenerateNewMesh() {
		// Set up vertices
		this.m_Vertices = new Vector3[this.m_MeshRoot.mesh.vertices.Length];
		Array.Copy (this.m_MeshRoot.mesh.vertices, this.m_Vertices, this.m_MeshRoot.mesh.vertices.Length);
		// Generate new mesh
		var newMesh = new Mesh();
		// newMesh.vertices = new Vector3[this.m_MeshRoot.mesh.vertices.Length];
		// Array.Copy (newMesh.vertices, this.m_MeshRoot.mesh.vertices, this.m_MeshRoot.mesh.vertices.Length);
		// newMesh.triangles = new int[this.m_MeshRoot.mesh.triangles.Length];
		// Array.Copy (newMesh.triangles, this.m_MeshRoot.mesh.triangles, this.m_MeshRoot.mesh.triangles.Length);
		// newMesh.normals = new Vector3[this.m_MeshRoot.mesh.normals.Length];
		// Array.Copy (newMesh.normals, this.m_MeshRoot.mesh.normals, this.m_MeshRoot.mesh.normals.Length);
		newMesh.vertices = this.m_MeshRoot.mesh.vertices;
        newMesh.triangles = this.m_MeshRoot.mesh.triangles;
        newMesh.uv = this.m_MeshRoot.mesh.uv;
        newMesh.normals = this.m_MeshRoot.mesh.normals;
        newMesh.colors = this.m_MeshRoot.mesh.colors;
        newMesh.tangents = this.m_MeshRoot.mesh.tangents;
		this.m_MeshFilter.mesh = newMesh;
		this.m_MeshFilter.mesh.RecalculateBounds();
		this.m_MeshFilter.mesh.RecalculateNormals();
		this.m_MeshFilter.mesh.RecalculateTangents();
	}

	public virtual void UpdateParamater() {
		var newRotation = this.m_Root.transform.rotation.eulerAngles;
		this.m_Rotation.rotation = new Vector3 (newRotation.x, newRotation.y, newRotation.z);
		var newPosition = this.m_MeshRoot.transform.localPosition;
		this.m_Position.position = new Vector3 (newPosition.x, newPosition.y, newPosition.z);
	}

	public virtual void UpdateVerticles() {
		var verticles = new Vector3[this.m_Vertices.Length];
		for (int i = 0; i < verticles.Length; i++)
		{
			verticles[i] = this.TransformPoint (this.m_Vertices[i]);
		}
		this.m_MeshFilter.mesh.vertices = verticles;
		this.m_MeshFilter.mesh.RecalculateBounds();
		this.m_MeshFilter.mesh.RecalculateNormals();
	}

	void UpdateTransformation () {
		transformation = transformations[0].Matrix;
		for (int i = 1; i < transformations.Length; i++) {
			transformation = transformations[i].Matrix * transformation;
		}
	}

	public Vector3 TransformPoint (int x, int y, int z) {
		Vector3 coordinates = new Vector3 (x, y, z);
		return transformation.MultiplyPoint(coordinates);
	}

	public Vector3 TransformPoint (Vector3 coordinates) {
		return transformation.MultiplyPoint(coordinates);
	}
	
}
