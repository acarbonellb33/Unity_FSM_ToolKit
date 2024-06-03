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
	using FSM.Nodes.States.StateScripts;

	[CustomEditor(typeof(PatrolTest))]
	public class PatrolTestEditor : Editor
	{
		private SerializedProperty selectedOptionIndexProp;
		private float lastClickedIndex = -1;
		Dictionary<string, StateScript> optionToObjectMap = new();
		private GameObject _selectedGameObject;
		private Component _selectedComponent;
		private string _selectedFunction;
		private readonly List<string> _componentNames = new();
		private readonly List<string> _functionNames = new();
		private int _selectedComponentIndex = 0;
		private int _selectedFunctionIndex = 0;
		void OnEnable()
		{
			selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
			PatrolTest patrolTest = (PatrolTest)target;
			Type type = typeof(PatrolTest);
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			int j = 0;
			foreach (FieldInfo field in fields)
			{
				string nameField = FixName(field.Name);
				try
				{
					optionToObjectMap[nameField] = patrolTest.options[j];
				}
				catch (ArgumentOutOfRangeException) { }
				j++;
			}
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			PatrolTest patrolTest = (PatrolTest)target;
			Type type = typeof(PatrolTest);
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
					GUI.backgroundColor = new Color(0.666f, 0.768f, 1f);
				}
				else
				{
					GUI.backgroundColor = new Color(0.91f, 0.91f, 0.91f);
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
				StateScript selectedObject = optionToObjectMap[selectedOptionName];
				SerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);
				selectedObjectSerialized.Update();
				EditorGUI.BeginChangeCheck();
				SerializedProperty iterator = selectedObjectSerialized.GetIterator();
				bool nextVisible = iterator.NextVisible(true);
				nextVisible = iterator.NextVisible(false);
				if (nextVisible)EditorGUILayout.LabelField($"{selectedOptionName} Attributes:", EditorStyles.boldLabel);
				while (nextVisible)
				{
					if (iterator.isArray && iterator.name != "selectedGameObject" && iterator.name != "selectedComponent" && iterator.name != "selectedFunction")
					{
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("Create a Patrol Waypoint", EditorStyles.boldLabel);
						EditorGUILayout.Space();
						for (int i = 0; i < iterator.arraySize; i++)
						{
							EditorGUILayout.BeginHorizontal();
							SerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);
							if (gameObjectElementProperty.stringValue != null)
							{
								GameObject gameObject = FsmIOUtility.FindGameObjectWithId<IDGenerator>(gameObjectElementProperty.stringValue);
								EditorGUI.BeginChangeCheck();
								gameObject = EditorGUILayout.ObjectField("Patrol Point", gameObject, typeof(GameObject), true) as GameObject;
								if (EditorGUI.EndChangeCheck())
								{
									if (gameObject != null)
									{
										IDGenerator idGenerator = gameObject.GetComponent<IDGenerator>();
										if (idGenerator.GetUniqueID() != null)
										{
											gameObjectElementProperty.stringValue = idGenerator.GetUniqueID();
										}
									}
									else
									{
										gameObjectElementProperty.stringValue = string.Empty;
									}
									selectedObjectSerialized.ApplyModifiedProperties();
									FsmIOUtility.CreateJson(selectedObject,  "PatrolTest");
								}
								if (GUILayout.Button("Remove", GUILayout.Width(70)))
								{
									RemovePatrolPoint(gameObjectElementProperty.stringValue);
									selectedObjectSerialized.ApplyModifiedProperties();
									FsmIOUtility.CreateJson(selectedObject,  "PatrolTest");
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						if (GUILayout.Button("Create and Add a Patrol Point"))
						{
							CreateAndAddGameObject(patrolTest);
							selectedObjectSerialized.ApplyModifiedProperties();
							FsmIOUtility.CreateJson(selectedObject,  "PatrolTest");
						}
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						if (iterator.name == "selectedGameObject")
						{
							_selectedGameObject = FsmIOUtility.FindGameObjectWithId<IDGenerator>(iterator.stringValue);
							_selectedGameObject = (GameObject)EditorGUILayout.ObjectField("Selected GameObject", _selectedGameObject, typeof(GameObject), true);
							if (_selectedGameObject != null)
							{
								PopulateComponentDropdown();
							}
						}
						else if (iterator.name == "selectedComponent")
						{
							if (_componentNames.Count > 0)
							{
								var count = 0;
								foreach(var comp in _componentNames)
								{
									if (comp == iterator.stringValue)
									{
										_selectedComponentIndex = count;
									}
									count++;
								}
								_selectedComponentIndex = EditorGUILayout.Popup("Select Component", _selectedComponentIndex, _componentNames.ToArray());
								if (_selectedComponentIndex >= 0 && _selectedComponentIndex < _componentNames.Count)
								{
									_selectedComponent = _selectedGameObject.GetComponent(_componentNames[_selectedComponentIndex]);
									PopulateFunctionDropdown();
								}
							}
						}
						else if (iterator.name == "selectedFunction")
						{
							if (_functionNames.Count > 0)
							{
								var count = 0;
								foreach(var comp in _functionNames)
								{
									if (comp == iterator.stringValue)
									{
										_selectedFunctionIndex = count;
									}
									count++;
								}
								_selectedFunctionIndex = EditorGUILayout.Popup("Select Function", _selectedFunctionIndex, _functionNames.ToArray());
								if (_selectedFunctionIndex >= 0 && _selectedFunctionIndex < _functionNames.Count)
								{
									_selectedFunction = _functionNames[_selectedFunctionIndex];
								}
							}
						}
						else
						{
							EditorGUILayout.PropertyField(iterator, true);
						}
						if (EditorGUI.EndChangeCheck())
						{
							selectedObject.SetStateName(selectedOptionName);
							if (selectedObject.GetStateName().StartsWith("CustomCondition"))
							{
								CustomConditionScript customCondition = (CustomConditionScript)selectedObject;
								customCondition.selectedGameObject = _selectedGameObject.GetComponent<IDGenerator>().GetUniqueID();
								if (_componentNames.Count > 0)
									customCondition.selectedComponent = _componentNames[_selectedComponentIndex];
								if (_functionNames.Count > 0)
									customCondition.selectedFunction = _functionNames[_selectedFunctionIndex];
								FsmIOUtility.CreateJson(selectedObject, "PatrolTest");
							}
							else if (selectedObject.GetStateName().StartsWith("Custom"))
							{
								CustomStateScript customState = (CustomStateScript)selectedObject;
								customState.selectedGameObject = _selectedGameObject.GetComponent<IDGenerator>().GetUniqueID();
								if (_componentNames.Count > 0)
									customState.selectedComponent = _componentNames[_selectedComponentIndex];
								if (_functionNames.Count > 0)
									customState.selectedFunction = _functionNames[_selectedFunctionIndex];
								FsmIOUtility.CreateJson(selectedObject, "PatrolTest");
							}
							selectedObjectSerialized.ApplyModifiedProperties();
							FsmIOUtility.CreateJson(selectedObject, "PatrolTest");
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
		private void CreateAndAddGameObject(PatrolTest patrolTest)
		{
			patrolTest.AddObjectToList();
		}
		private void RemovePatrolPoint(string patrolPoint)
		{
			PatrolTest patrolTest = (PatrolTest)target;
			patrolTest.patrol.RemovePatrolPoint(patrolPoint);
			GameObject patrolPointObject = FsmIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);
			if(patrolPointObject != null)
			{
				DestroyImmediate(patrolPointObject);
			}
		}
		private string FixName(string oldName)
		{
			return char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);
		}
		private void PopulateComponentDropdown()
		{
			var components = _selectedGameObject.GetComponents<Component>();
			_componentNames.Clear();
			foreach (var comp in components)
			{
				_componentNames.Add(comp.GetType().Name);
			}
			_selectedComponentIndex = 0;
			if (_componentNames.Count > 0)
			{
				_selectedComponent = _selectedGameObject.GetComponent(_componentNames[_selectedComponentIndex]);
				PopulateFunctionDropdown();
			}
		}
		private void PopulateFunctionDropdown()
		{
			if (_selectedComponent == null)
			{
				return;
			}
			var methods = _selectedComponent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			_functionNames.Clear();
			foreach (var method in methods)
			{
				_functionNames.Add(method.Name);
			}
		}
	}
}
#endif
