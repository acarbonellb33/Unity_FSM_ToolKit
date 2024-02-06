using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(testing))]
public class testingEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
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
								Vector3 position = gameObject.transform.position;
								EditorGUILayout.LabelField("Position " + i, "X: " + position.x + "\tY: " + position.y + "\tZ: " + position.z);
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
		GameObject newGameObject = new GameObject("Patrol Point " + count);
		string prefabPath = "Assets/Prefabs/PatrolPoint" + count + ".prefab";
		PrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);
		GameObject prefabToList = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		GameObject prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath)) as GameObject;
		DestroyImmediate(newGameObject);
		if (prefabToList != null)
		{
			testing.AddObjectToList(prefabToList);
		}
		else
		{
			Debug.LogError("Failed to create a new GameObject");
		}
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		testing testing = (testing)target;
		testing.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
}
