using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(czzzzzzzzzzzzzzzzzzzzzzzzz))]
public class czzzzzzzzzzzzzzzzzzzzzzzzzEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		czzzzzzzzzzzzzzzzzzzzzzzzz czzzzzzzzzzzzzzzzzzzzzzzzz = (czzzzzzzzzzzzzzzzzzzzzzzzz)target;
		for (int i = 0; i < czzzzzzzzzzzzzzzzzzzzzzzzz.options.Count; i++)
		{
			optionToObjectMap[czzzzzzzzzzzzzzzzzzzzzzzzz.options[i].GetStateName()] = czzzzzzzzzzzzzzzzzzzzzzzzz.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		czzzzzzzzzzzzzzzzzzzzzzzzz czzzzzzzzzzzzzzzzzzzzzzzzz = (czzzzzzzzzzzzzzzzzzzzzzzzz)target;
		string[] options = new string[czzzzzzzzzzzzzzzzzzzzzzzzz.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = czzzzzzzzzzzzzzzzzzzzzzzzz.options[i].GetStateName();
		}
		selectedOptionIndexProp.intValue = EditorGUILayout.Popup("Selected Option", selectedOptionIndexProp.intValue, options);
		string selectedOptionName = options[selectedOptionIndexProp.intValue];
		if (optionToObjectMap.ContainsKey(selectedOptionName))
		{
			EditorGUILayout.LabelField($"{selectedOptionName} Attributes:");
			StateScript selectedObject = optionToObjectMap[selectedOptionName];
			SerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);
			selectedObjectSerialized.Update();
			EditorGUI.BeginChangeCheck();
			SerializedProperty iterator = selectedObjectSerialized.GetIterator();
			bool nextVisible = iterator.NextVisible(true);
			while (nextVisible)
			{
				if (iterator.name != "m_Script")
				{
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(iterator, true);
					if (EditorGUI.EndChangeCheck())
					{
						selectedObjectSerialized.ApplyModifiedProperties();
						FSMIOUtility.CreateJson(selectedObject, "czzzzzzzzzzzzzzzzzzzzzzzzz");
					}
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
