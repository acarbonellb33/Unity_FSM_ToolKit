using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(testing))]
public class testingEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	private SerializedProperty currentStateProperty;
	Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		currentStateProperty = serializedObject.FindProperty("currentState");
		testing testing = (testing)target;
		for (int i = 0; i < testing.options.Count; i++)
		{
			optionToObjectMap[testing.options[i].GetStateName()] = testing.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		testing testing = (testing)target;
		string[] options = new string[testing.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = testing.options[i].GetStateName();
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
					if (iterator.isArray)
					{
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("Create a Patrol Waypoint", EditorStyles.boldLabel);
						EditorGUILayout.Space();
						EditorGUILayout.PropertyField(iterator, true);
						for (int i = 0; i < iterator.arraySize; i++)
						{
							EditorGUILayout.BeginHorizontal();
							SerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);
							if (gameObjectElementProperty.objectReferenceValue != null)
							{
								GameObject gameObject = (GameObject)gameObjectElementProperty.objectReferenceValue;
								EditorGUILayout.Vector3Field("Position " + i, gameObject.transform.position);
								if (GUILayout.Button("Remove", GUILayout.Width(70)))
								{
									RemovePatrolPoint(gameObject);
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(testing, iterator.arraySize);
						}
					}
					else
					{
						EditorGUILayout.PropertyField(iterator, true);
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
	private void CreateAndAddGameObject(testing testing, int count)
	{
		GameObject newGameObject = new GameObject("Patrol Point "+count);
		testing.AddObjectToList(newGameObject);
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		testing script = (testing)target;
		script.patrol.RemovePatrolPoint(patrolPoint);
		DestroyImmediate(patrolPoint);
	}
}
