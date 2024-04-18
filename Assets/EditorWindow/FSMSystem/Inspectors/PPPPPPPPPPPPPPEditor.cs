using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using UnityEngine;
[CustomEditor(typeof(PPPPPPPPPPPPPP))]
public class PPPPPPPPPPPPPPEditor : Editor
{
	private SerializedProperty selectedOptionIndexProp;
	Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
	void OnEnable()
	{
		selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
		PPPPPPPPPPPPPP pPPPPPPPPPPPPP = (PPPPPPPPPPPPPP)target;
		for (int i = 0; i < pPPPPPPPPPPPPP.options.Count; i++)
		{
			optionToObjectMap[pPPPPPPPPPPPPP.options[i].GetStateName()] = pPPPPPPPPPPPPP.options[i];
		}
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		PPPPPPPPPPPPPP pPPPPPPPPPPPPP = (PPPPPPPPPPPPPP)target;
		string[] options = new string[pPPPPPPPPPPPPP.options.Count];
		for (int i = 0; i < options.Length; i++)
		{
			options[i] = pPPPPPPPPPPPPP.options[i].GetStateName();
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
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(iterator, true);
					if (EditorGUI.EndChangeCheck())
					{
						selectedObjectSerialized.ApplyModifiedProperties();
						FSMIOUtility.CreateJson(selectedObject, "PPPPPPPPPPPPPP");
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
}
