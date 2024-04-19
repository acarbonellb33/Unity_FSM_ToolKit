using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
[CustomEditor(typeof(testing))]
public class testingEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		testing testing = (testing)target;
		Type type = typeof(testing);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int i = 0;
		foreach (FieldInfo field in fields)
		{
			string nameField = FixName(field.Name);
			optionToObjectMap[nameField] = testing.options[i];
			i++;
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		testing testing = (testing)target;
		string[] options = new string[testing.options.Count];
		Type type = typeof(testing);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int i = 0;
		foreach (FieldInfo field in fields) 
		{
			string nameField = FixName(field.Name);
			options[i] = nameField;
			i++;
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
						FSMIOUtility.CreateJson(selectedObject, "testing");
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
