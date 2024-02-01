using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(mulecalva))]
public class mulecalvaEditor : Editor
{
	private SerializedProperty attackProperty;
	private SerializedProperty hearingProperty;
	private SerializedProperty distanceProperty;
	private SerializedProperty patrolProperty;
	private SerializedProperty optionsProp;
	private SerializedProperty selectedOptionIndexProp;
	private SerializedProperty currentStateProperty;
	Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
	void OnEnable()
	{
		attackProperty = serializedObject.FindProperty("attack");
		hearingProperty = serializedObject.FindProperty("hearing");
		distanceProperty = serializedObject.FindProperty("distance");
		patrolProperty = serializedObject.FindProperty("patrol");
		optionsProp = serializedObject.FindProperty("options");
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		currentStateProperty = serializedObject.FindProperty("currentState");
		mulecalva mulecalva = (mulecalva)target;
		for (int i = 0; i < mulecalva.options.Count; i++)
		{
			optionToObjectMap[mulecalva.options[i].GetStateName()] = mulecalva.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		mulecalva mulecalva = (mulecalva)target;
		string[] options = new string[mulecalva.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = mulecalva.options[i].GetStateName();
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
