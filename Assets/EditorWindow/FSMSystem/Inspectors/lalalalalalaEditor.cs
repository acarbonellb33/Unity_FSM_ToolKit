#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Inspectors
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using System.Reflection;
	using BehaviorScripts;
	using Utilities;
	using UnityEngine;
	using FSM.Nodes.States;
	using FSM.Utilities;

	[CustomEditor(typeof(lalalalalala))]
	public class lalalalalalaEditor : Editor
	{
		private SerializedProperty selectedOptionIndexProp;
		private float lastClickedIndex = -1;
		Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
		void OnEnable()
		{
			selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
			lalalalalala lalalalalala = (lalalalalala)target;
			Type type = typeof(lalalalalala);
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			int j = 0;
			foreach (FieldInfo field in fields)
			{
				string nameField = FixName(field.Name);
				try
				{
					optionToObjectMap[nameField] = lalalalalala.options[j];
				}
				catch (ArgumentOutOfRangeException) { }
				j++;
			}
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			lalalalalala lalalalalala = (lalalalalala)target;
			Type type = typeof(lalalalalala);
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
			GUILayout.Space(10);
			EditorGUILayout.LabelField("SELECT THE STATE YOU WANT TO MODIFY", new GUIStyle(EditorStyles.boldLabel)
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
					GUI.backgroundColor = new Color(0.89f, 0.716f, 0.969f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.575f, 0f, 0.671f);
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
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(iterator, true);
						if (EditorGUI.EndChangeCheck())
						{
							selectedObject.SetStateName(selectedOptionName);
							selectedObjectSerialized.ApplyModifiedProperties();
							FsmIOUtility.CreateJson(selectedObject, "lalalalalala");
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
}
#endif
