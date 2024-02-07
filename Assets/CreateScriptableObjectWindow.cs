using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateScriptableObjectWindow : EditorWindow
{
    private string scriptableObjectName;
    private string savePath;
    
    [MenuItem("Window/FSM/FSM Graph")]
    public static void ShowWindow()
    {
        GetWindow<CreateScriptableObjectWindow>("Create Scriptable Object");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Scriptable Object", EditorStyles.boldLabel);

        scriptableObjectName = EditorGUILayout.TextField("Scriptable Object Name:", scriptableObjectName);
        savePath = "Assets/EditorWindow/FSMSystem/Graphs";

        if (GUILayout.Button("Create Scriptable Object"))
        {
            // Create the scriptable object instance
            FSMGraphSaveData newScriptableObject = ScriptableObject.CreateInstance<FSMGraphSaveData>();

            // Set its name using the provided name
            newScriptableObject.Initialize(scriptableObjectName, "");

            // Save it to the specified path
            string fullPath = $"{savePath}/{scriptableObjectName}.asset";
            AssetDatabase.CreateAsset(newScriptableObject, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Scriptable object created and saved at: {fullPath}");

            // Close the window
            Close();
        }
    }
}
