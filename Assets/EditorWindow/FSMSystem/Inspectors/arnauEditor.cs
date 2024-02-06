using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(arnau))]
public class arnauEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
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
						Debug.Log("iterator.arraySize: " + iterator.arraySize);
						for (int i = 0; i < iterator.arraySize; i++)
						{
							EditorGUILayout.BeginHorizontal();
							SerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);
							if (gameObjectElementProperty.objectReferenceValue != null)
							{
								GameObject gameObject = (GameObject)gameObjectElementProperty.objectReferenceValue;
								Vector3 position = gameObject.transform.position;
								EditorGUILayout.PropertyField(gameObjectElementProperty, GUIContent.none);
								//EditorGUILayout.LabelField("Position " + i, "X: " + position.x + "\tY: " + position.y + "\tZ: " + position.z);
								if (GUILayout.Button("Remove", GUILayout.Width(70)))
								{
									RemovePatrolPoint(gameObject);
								}

								FSMIOUtility.CreateJson(selectedObject, "arnau");
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(arnau);
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
									FSMIOUtility.UpdateJson("arnau",selectedOptionName, iterator.name, iterator.floatValue);
									break;
								case "int":
									FSMIOUtility.UpdateJson("arnau",selectedOptionName, iterator.name, iterator.intValue);
									break;
								case "bool":
									FSMIOUtility.UpdateJson("arnau",selectedOptionName, iterator.name, iterator.boolValue);
									break;
								case "string":
									FSMIOUtility.UpdateJson("arnau",selectedOptionName, iterator.name, iterator.stringValue);
									break;
								case "PPtr<$GameObject>":
									FSMIOUtility.UpdateJson("arnau",selectedOptionName, iterator.name, iterator.objectReferenceValue);
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
	private void CreateAndAddGameObject(arnau arnau)
	{
		GameObject newGameObject = new GameObject("Patrol Point");
		arnau.AddObjectToList(newGameObject);
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		arnau arnau = (arnau)target;
		arnau.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
}
