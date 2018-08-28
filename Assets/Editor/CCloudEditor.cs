using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CCloud))]
public class CCloudEditor: Editor {

	private Vector3 m_MovePot = new Vector3(2f, 0f, 0f);

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		var cloud = target as CCloud;
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
		if (GUILayout.Button("Rotation X")) {
			cloud.Rotation(new Vector3(90f, 0f, 0f));
		}
		if (GUILayout.Button("Rotation Y")) {
			cloud.Rotation(new Vector3(0f, 90f, 0f));
		}
		if (GUILayout.Button("Rotation Z")) {
			cloud.Rotation(new Vector3(0f, 0f, -90f));
		}
		this.m_MovePot = EditorGUILayout.Vector3Field("Move position", this.m_MovePot);
		if (GUILayout.Button("Move")) {
			cloud.Move(this.m_MovePot);
		}
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Generate", EditorStyles.boldLabel);
		if (GUILayout.Button("Generate random 3x3 20%")) {
			var generatedGrid = cloud.GenerateRandom3x3WithRate(20);
			Debug.Log (generatedGrid);
			GUIUtility.systemCopyBuffer = generatedGrid;
		}
		if (GUILayout.Button("Generate random 3x3 40%")) {
			var generatedGrid = cloud.GenerateRandom3x3WithRate(40);
			Debug.Log (generatedGrid);
			GUIUtility.systemCopyBuffer = generatedGrid;
		}
		if (GUILayout.Button("Generate random 3x3 60%")) {
			var generatedGrid = cloud.GenerateRandom3x3WithRate(60);
			Debug.Log (generatedGrid);
			GUIUtility.systemCopyBuffer = generatedGrid;
		}
		EditorGUILayout.Space();
		if (GUILayout.Button("Show clipboard")) {
			Debug.Log (GUIUtility.systemCopyBuffer.Normalize());
		}
	}

}
