using UnityEngine;

public class CameraTransformation : Transformation {

	// 1. Perspective Camera
	// public float focalLength = 1f;

	public override Matrix4x4 Matrix {
		get {
			Matrix4x4 matrix = new Matrix4x4();
			// 1. Perspective Camera
			// matrix.SetRow(0, new Vector4(focalLength, 0f, 0f, 0f));
			// matrix.SetRow(1, new Vector4(0f, focalLength, 0f, 0f));
			// matrix.SetRow(2, new Vector4(0f, 0f, 0f, 0f));
			// matrix.SetRow(3, new Vector4(0f, 0f, 1f, 0f));
			// 2. Orthographic Camera
			matrix.SetRow(0, new Vector4(1f, 0f, 0f, 0f));
			matrix.SetRow(1, new Vector4(0f, 1f, 0f, 0f));
			matrix.SetRow(2, new Vector4(0f, 0f, 0f, 0f));
			matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			return matrix;
		}
	}
}