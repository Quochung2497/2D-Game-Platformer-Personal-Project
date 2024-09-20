using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MeshRenderer))]

public class MeshRendererSortingLayersEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		MeshRenderer renderer = target as MeshRenderer;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();

		string name = EditorGUILayout.TextField("Sorting Layer Name", renderer.sortingLayerName);

		if (EditorGUI.EndChangeCheck())
		{
			renderer.sortingLayerName = name;
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();

		int order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);

		if (EditorGUI.EndChangeCheck())
		{
			renderer.sortingOrder = order;
		}

		EditorGUILayout.EndHorizontal();
	}
}