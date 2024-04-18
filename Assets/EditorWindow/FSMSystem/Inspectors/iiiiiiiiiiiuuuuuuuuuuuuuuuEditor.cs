using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(iiiiiiiiiiiuuuuuuuuuuuuuuu))]
public class iiiiiiiiiiiuuuuuuuuuuuuuuuEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		iiiiiiiiiiiuuuuuuuuuuuuuuu iiiiiiiiiiiuuuuuuuuuuuuuuu = (iiiiiiiiiiiuuuuuuuuuuuuuuu)target;
		for (int i = 0; i < iiiiiiiiiiiuuuuuuuuuuuuuuu.options.Count; i++)
		{
			optionToObjectMap[iiiiiiiiiiiuuuuuuuuuuuuuuu.options[i].GetStateName()] = iiiiiiiiiiiuuuuuuuuuuuuuuu.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		iiiiiiiiiiiuuuuuuuuuuuuuuu iiiiiiiiiiiuuuuuuuuuuuuuuu = (iiiiiiiiiiiuuuuuuuuuuuuuuu)target;
		string[] options = new string[iiiiiiiiiiiuuuuuuuuuuuuuuu.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = iiiiiiiiiiiuuuuuuuuuuuuuuu.options[i].GetStateName();
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
								FSMIOUtility.CreateJson(selectedObject, "iiiiiiiiiiiuuuuuuuuuuuuuuu");
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(iiiiiiiiiiiuuuuuuuuuuuuuuu);
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(iterator, true);
						if (EditorGUI.EndChangeCheck())
						{
							selectedObjectSerialized.ApplyModifiedProperties();
							FSMIOUtility.CreateJson(selectedObject, "iiiiiiiiiiiuuuuuuuuuuuuuuu");
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
	private void CreateAndAddGameObject(iiiiiiiiiiiuuuuuuuuuuuuuuu iiiiiiiiiiiuuuuuuuuuuuuuuu)
	{
		iiiiiiiiiiiuuuuuuuuuuuuuuu.AddObjectToList();
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		iiiiiiiiiiiuuuuuuuuuuuuuuu iiiiiiiiiiiuuuuuuuuuuuuuuu = (iiiiiiiiiiiuuuuuuuuuuuuuuu)target;
		iiiiiiiiiiiuuuuuuuuuuuuuuu.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
}
