using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(finiteStateMachine))]
public class finiteStateMachineEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		finiteStateMachine finiteStateMachine = (finiteStateMachine)target;
		for (int i = 0; i < finiteStateMachine.options.Count; i++)
		{
			optionToObjectMap[finiteStateMachine.options[i].GetStateName()] = finiteStateMachine.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		finiteStateMachine finiteStateMachine = (finiteStateMachine)target;
		string[] options = new string[finiteStateMachine.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = finiteStateMachine.options[i].GetStateName();
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
							CreateAndAddGameObject(finiteStateMachine, iterator.arraySize);
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
	private void CreateAndAddGameObject(finiteStateMachine finiteStateMachine, int count)
	{
		GameObject newGameObject = new GameObject("Patrol Point " + count);
		string prefabPath = "Assets/Prefabs/PatrolPoint" + count + ".prefab";
		PrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);
		GameObject prefabToList = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
		GameObject prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath)) as GameObject;
		DestroyImmediate(newGameObject);
		if (prefabToList != null)
		{
			finiteStateMachine.AddObjectToList(prefabToList);
		}
		else
		{
			Debug.LogError("Failed to create a new GameObject");
		}
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		finiteStateMachine finiteStateMachine = (finiteStateMachine)target;
		finiteStateMachine.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
}
