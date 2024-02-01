using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(arnau))]
public class arnauEditor : Editor
{
	private SerializedProperty attackProperty;
	private SerializedProperty hearingProperty;
	private SerializedProperty patrolProperty;
	private SerializedProperty distanceProperty;
	private SerializedProperty optionsProp;
	private SerializedProperty selectedOptionIndexProp;
	private SerializedProperty currentStateProperty;
	Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
	void OnEnable()
	{
		attackProperty = serializedObject.FindProperty("attack");
		hearingProperty = serializedObject.FindProperty("hearing");
		patrolProperty = serializedObject.FindProperty("patrol");
		distanceProperty = serializedObject.FindProperty("distance");
		optionsProp = serializedObject.FindProperty("options");
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		currentStateProperty = serializedObject.FindProperty("currentState");
		arnau arnau = (arnau)target;
		for (int i = 0; i < arnau.options.Count; i++)
		{
			optionToObjectMap[arnau.options[i].GetStateName()] = arnau.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		arnau arnau = (arnau)target;
		string[] options = new string[arnau.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = arnau.options[i].GetStateName();
		}
		selectedOptionIndexProp.intValue = EditorGUILayout.Popup("Selected Option", selectedOptionIndexProp.intValue, options);
		string selectedOptionName = options[selectedOptionIndexProp.intValue];
		if (optionToObjectMap.ContainsKey(selectedOptionName))
		{
			EditorGUILayout.LabelField($"{selectedOptionName} Attributes:");
			ScriptableObject selectedObject = optionToObjectMap[selectedOptionName];
			SerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);
			selectedObjectSerialized.Update();
			EditorGUI.BeginChangeCheck();
			SerializedProperty iterator = selectedObjectSerialized.GetIterator();
			bool nextVisible = iterator.NextVisible(true);
			while (nextVisible)
			{
				if (iterator.name != "m_Script")
				{
					EditorGUILayout.PropertyField(iterator, true);
				}
				nextVisible = iterator.NextVisible(false);
			}
			if (EditorGUI.EndChangeCheck())
			{
				selectedObjectSerialized.ApplyModifiedProperties();
			}
		}
		serializedObject.ApplyModifiedProperties();
	}
}
