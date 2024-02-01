using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(test))]
public class testEditor : Editor
{
    private SerializedProperty attackProperty;
    private SerializedProperty distanceProperty;
    private SerializedProperty hearingProperty;
    private SerializedProperty patrolProperty;
    
    SerializedProperty optionsProp;
    SerializedProperty selectedOptionIndexProp;
    
    private SerializedProperty currentStateProperty;
    
    Dictionary<string, State> optionToObjectMap = new Dictionary<string, State>();
    
    private void OnEnable()
    {
        attackProperty = serializedObject.FindProperty("attack");
        distanceProperty = serializedObject.FindProperty("distance");
        hearingProperty = serializedObject.FindProperty("hearing");
        patrolProperty = serializedObject.FindProperty("patrol");
        
        optionsProp = serializedObject.FindProperty("options");
        selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
        
        currentStateProperty = serializedObject.FindProperty("currentState");
        
        test test = (test)target;
        for (int i = 0; i < test.options.Count; i++)
        {
            optionToObjectMap[test.options[i].GetStateName()] = test.options[i];
        }
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        test test = (test)target;
        
        string[] options = new string[test.options.Count];
        for (int i = 0; i < test.options.Count; i++)
        {
            options[i] = test.options[i].GetStateName();
        }

        selectedOptionIndexProp.intValue = EditorGUILayout.Popup("Selected Option", selectedOptionIndexProp.intValue, options);

        string selectedOptionName = options[selectedOptionIndexProp.intValue];

        if (optionToObjectMap.ContainsKey(selectedOptionName))
        {
            EditorGUILayout.LabelField($"{selectedOptionName} Attributes:");
            ScriptableObject selectedObject = optionToObjectMap[selectedOptionName];
            SerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);
            selectedObjectSerialized.Update();
            EditorGUI.BeginChangeCheck(); // Begin change check
            SerializedProperty iterator = selectedObjectSerialized.GetIterator();
            bool nextVisible = iterator.NextVisible(true);
            while (nextVisible)
            { 
                if (iterator.name != "m_Script")
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                nextVisible = iterator.NextVisible(false);
            }
            if (EditorGUI.EndChangeCheck()) // End change check
            {
                selectedObjectSerialized.ApplyModifiedProperties(); // Apply changes to the ScriptableObject
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
