using UnityEngine;

public class ScaleTransformation : Transformation {

	public Vector3 scale = new Vector3(1f, 1f, 1f);
	// public Vector3 scale {
	// 	get { return this.m_Transform.lossyScale; }
	// 	set { this.m_Transform.localScale = value; }
	// }
	protected Transform m_Transform;

	protected virtual void Awake()
	{
		this.m_Transform = this.transform;
	}

	public override Matrix4x4 Matrix {
		get {
			Matrix4x4 matrix = new Matrix4x4();
			matrix.SetRow(0, new Vector4(scale.x, 0f, 0f, 0f));
			matrix.SetRow(1, new Vector4(0f, scale.y, 0f, 0f));
			matrix.SetRow(2, new Vector4(0f, 0f, scale.z, 0f));
			matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			return matrix;
		}
	}
}