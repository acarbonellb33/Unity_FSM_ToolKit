using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using UnityEngine;

[CustomEditor(typeof(newUpdate))]
public class newUpdateEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	private float lastClickedIndex = -1;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		newUpdate newUpdate = (newUpdate)target;
		Type type = typeof(newUpdate);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		int j = 0;
		foreach (FieldInfo field in fields)
		{
			string nameField = FixName(field.Name);
			optionToObjectMap[nameField] = newUpdate.options[j];
			j++;
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		newUpdate newUpdate = (newUpdate)target;
		Type type = typeof(newUpdate);
		FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		string[] options = new string[fields.Length];
		int x = 0;
		foreach (FieldInfo field in fields) 
		{
			string nameField = FixName(field.Name);
			options[x] = nameField;
			x++;
		}
		string selectedOptionName = options[selectedOptionIndexProp.intValue];
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.hover.textColor = Color.white;
		buttonStyle.fontSize = 14;
		buttonStyle.fixedHeight = 30;
		buttonStyle.fixedWidth = 150;
		buttonStyle.margin = new RectOffset(5, 5, 5, 5);
		buttonStyle.padding = new RectOffset(0, 0, 0, 0);
		buttonStyle.normal.background = MakeTex(2, 2, new Color(0.3f, 0.3f, 0.3f));
		buttonStyle.active.background = MakeTex(2, 2, new Color(0.1f, 0.6f, 0.1f));
		GUILayout.Space(10);
		EditorGUILayout.LabelField("Select the state you want to modify", new GUIStyle(EditorStyles.boldLabel)
		{
			alignment = TextAnchor.MiddleCenter,
			fontSize = 14
		});
		int buttonsPerRow = 3;
		int buttonCount = 0;
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		for (int i = 0; i < options.Length; i++)
		{
			if (buttonCount > 0 && buttonCount % buttonsPerRow == 0)
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}
			bool isSelected = selectedOptionIndexProp.intValue == i;
			if (isSelected || lastClickedIndex == i)
			{
				GUI.backgroundColor = isSelected ? Color.green : new Color(0.1f, 0.6f, 0.1f);
			}
			else
			{
				GUI.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(options[i], buttonStyle))
			{
				selectedOptionIndexProp.intValue = i;
				lastClickedIndex = i;
			}
			GUILayout.FlexibleSpace();
			buttonCount++;
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		if (optionToObjectMap.ContainsKey(selectedOptionName))
		{
			EditorGUILayout.LabelField($"{selectedOptionName} Attributes:", EditorStyles.boldLabel);
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
								FSMIOUtility.CreateJson(selectedObject, "newUpdate");
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(newUpdate);
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(iterator, true);
						if (EditorGUI.EndChangeCheck())
						{
							selectedObjectSerialized.ApplyModifiedProperties();
							FSMIOUtility.CreateJson(selectedObject, "newUpdate");
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
	private void CreateAndAddGameObject(newUpdate newUpdate)
	{
		newUpdate.AddObjectToList();
	}
	private void RemovePatrolPoint(GameObject patrolPoint)
	{
		newUpdate newUpdate = (newUpdate)target;
		newUpdate.patrol.RemovePatrolPoint(patrolPoint);
		if(GameObject.Find(patrolPoint.name) != null)
		{
			DestroyImmediate(patrolPoint);
		}
	}
	private string FixName(string oldName)
	{
		return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
	}
	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}
}
