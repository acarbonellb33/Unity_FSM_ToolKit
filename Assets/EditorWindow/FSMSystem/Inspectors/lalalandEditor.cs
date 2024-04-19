using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;

[CustomEditor(typeof(lalaland))]
public class lalalandEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		lalaland lalaland = (lalaland)target;
		Type type = typeof(lalaland);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int j = 0;
		foreach (FieldInfo field in fields)
		{
			string nameField = FixName(field.Name);
			optionToObjectMap[nameField] = lalaland.options[j];
			j++;
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		lalaland lalaland = (lalaland)target;
		string[] options = new string[lalaland.options.Count];
		Type type = typeof(lalaland);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int x = 0;
		foreach (FieldInfo field in fields) 
		{
			string nameField = FixName(field.Name);
			options[x] = nameField;
			x++;
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
						selectedObject.SetStateName(selectedOptionName);
						selectedObjectSerialized.ApplyModifiedProperties();
						FSMIOUtility.CreateJson(selectedObject, "lalaland");
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
	private string FixName(string oldName)
	{
		return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
	}
}
