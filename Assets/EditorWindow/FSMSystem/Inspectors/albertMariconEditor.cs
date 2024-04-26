using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;

[CustomEditor(typeof(albertMaricon))]
public class albertMariconEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		albertMaricon albertMaricon = (albertMaricon)target;
		Type type = typeof(albertMaricon);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int j = 0;
		foreach (FieldInfo field in fields)
		{
			string nameField = FixName(field.Name);
			optionToObjectMap[nameField] = albertMaricon.options[j];
			j++;
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		albertMaricon albertMaricon = (albertMaricon)target;
		string[] options = new string[albertMaricon.options.Count];
		Type type = typeof(albertMaricon);
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
					if (iterator.isArray)
					{
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("Create a Patrol Waypoint", EditorStyles.boldLabel);
						EditorGUILayout.Space();
						for (int i = 0; i < iterator.arraySize; i++)
						{
							EditorGUILayout.BeginHorizontal();
							SerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);
							if (gameObjectElementProperty.objectReferenceValue != null)
							{
								GameObject gameObject = (GameObject)gameObjectElementProperty.objectReferenceValue;
								EditorGUILayout.PropertyField(gameObjectElementProperty, GUIContent.none);
								if (GUILayout.Button("Remove", GUILayout.Width(70)))
								{
									RemovePatrolPoint(gameObject);
								}
								FSMIOUtility.CreateJson(selectedObject, "albertMaricon");
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(albertMaricon);
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(iterator, true);
						if (EditorGUI.EndChangeCheck())
						{
							selectedObjectSerialized.ApplyModifiedProperties();
							FSMIOUtility.CreateJson(selectedObject, "albertMaricon");
						}
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
	private void CreateAndAddGameObject(albertMaricon albertMaricon)
	{
		albertMaricon.AddObjectToList();
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		albertMaricon albertMaricon = (albertMaricon)target;
		albertMaricon.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
	private string FixName(string oldName)
	{
		return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
	}
}