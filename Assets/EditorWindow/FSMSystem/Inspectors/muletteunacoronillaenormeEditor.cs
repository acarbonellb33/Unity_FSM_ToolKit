using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(muletteunacoronillaenorme))]
public class muletteunacoronillaenormeEditor : Editor
{

	private SerializedProperty selectedOptionIndexProp;
	private SerializedProperty currentStateProperty;
	Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		currentStateProperty = serializedObject.FindProperty("currentState");
		muletteunacoronillaenorme muletteunacoronillaenorme = (muletteunacoronillaenorme)target;
		for (int i = 0; i < muletteunacoronillaenorme.options.Count; i++)
		{
			optionToObjectMap[muletteunacoronillaenorme.options[i].GetStateName()] = muletteunacoronillaenorme.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		muletteunacoronillaenorme muletteunacoronillaenorme = (muletteunacoronillaenorme)target;
		string[] options = new string[muletteunacoronillaenorme.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = muletteunacoronillaenorme.options[i].GetStateName();
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
						if (GUILayout.Button("Create and Add GameObject"))
						{
							CreateAndAddGameObject(muletteunacoronillaenorme);
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
	
	private void CreateAndAddGameObject(muletteunacoronillaenorme muletteunacoronillaenorme)
	{
		GameObject newGameObject = new GameObject();
		muletteunacoronillaenorme.AddObjectToList(newGameObject);
	}
}
