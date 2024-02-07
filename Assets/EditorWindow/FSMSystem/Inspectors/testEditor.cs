using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(test))]
public class testEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		test test = (test)target;
		for (int i = 0; i < test.options.Count; i++)
		{
			optionToObjectMap[test.options[i].GetStateName()] = test.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		test test = (test)target;
		string[] options = new string[test.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = test.options[i].GetStateName();
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
							FSMIOUtility.CreateJson(selectedObject, "test");
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(test);
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(iterator, true);
						if (EditorGUI.EndChangeCheck())
						{
							switch(iterator.type)
							{
								case "float":
									FSMIOUtility.UpdateJson("test",selectedOptionName, iterator.name, iterator.floatValue);
									break;
								case "int":
									FSMIOUtility.UpdateJson("test",selectedOptionName, iterator.name, iterator.intValue);
									break;
								case "bool":
									FSMIOUtility.UpdateJson("test",selectedOptionName, iterator.name, iterator.boolValue);
									break;
								case "string":
									FSMIOUtility.UpdateJson("test",selectedOptionName, iterator.name, iterator.stringValue);
									break;
								case "PPtr<$GameObject>":
									FSMIOUtility.UpdateJson("test",selectedOptionName, iterator.name, iterator.objectReferenceValue);
									break;
							}
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
	private void CreateAndAddGameObject(test test)
	{
		test.AddObjectToList();
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		test test = (test)target;
		test.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
}
