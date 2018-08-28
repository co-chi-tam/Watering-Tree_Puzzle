using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CMapGenerate))]
public class CMapGenerateEditor : Editor {

	public override void OnInspectorGUI() {
		var map = target as CMapGenerate;
		EditorGUILayout.Space();
		if (GUILayout.Button("Generate Json")) {
			map.GenerateFullInfo();
		}
		if (GUILayout.Button("Renderer Map")) {
			map.LoadMapDataWithAsset();
		}
		EditorGUILayout.Space();
		DrawDefaultInspector();
	}

}
