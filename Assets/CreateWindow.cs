using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateWindow : EditorWindow
{
    private Type selectedType;
    private FSMGraphSaveData saveData;
    private ObjectField _gameObjectField;
    public void Initialize(Type type, FSMGraphSaveData saveData)
    {
        selectedType = type;
        CreateWindow window = EditorWindow.GetWindow<CreateWindow>();
        window.Show();
        this.saveData = saveData;
    }

    private void OnEnable()
    {
        Label titleLabel = FSMElementUtility.CreateLabel("Assign New AI Enemy Script", null);
        Label infoLabel =FSMElementUtility.CreateLabel("Here you can add the new generated script to the enemy GameObject.\nNow select the gameObject from the scene that you want:", null);
        _gameObjectField = new ObjectField("Enemy GameObject:");
        _gameObjectField.objectType = typeof(GameObject);
        Button button = FSMElementUtility.CreateButton("Add Component", () => AddComponentToGameObject());
        
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(titleLabel);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(infoLabel);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(_gameObjectField);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(button);
    }
    
    
    void AddComponentToGameObject()
    {
        string scriptName = "alt"; // Replace "YourScriptName" with the name of the script you want to search for

        string[] guids = AssetDatabase.FindAssets("t:Script " + scriptName); // Searches for assets of type Script with the specified name
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            //Debug.Log("Found script '" + scriptName + "' at path: " + path);
                
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                // Check if the script is not null
            if (script != null)
            {
                if (_gameObjectField.value != null)
                {
                        // Add the script as a component to the GameObject
                    MonoBehaviour newScriptInstance = (MonoBehaviour)((GameObject)_gameObjectField.value).AddComponent(script.GetClass());

                    MethodInfo dynamicMethod = script.GetClass().GetMethod("SetVariableValue");

                    Debug.Log(dynamicMethod);
                    if (dynamicMethod != null)
                    {
                        for (int i = 0; i < saveData.Nodes.Count; i++)
                        {
                            Debug.Log(saveData.Nodes[i].Name);
                            dynamicMethod.Invoke(newScriptInstance,new object[]{char.ToLowerInvariant(saveData.Nodes[i].Name[0]) + saveData.Nodes[i].Name.Substring(1), saveData.Nodes[i].ScriptableObject});
                        }
                    }
                    //Debug.Log("Script '" + scriptName + "' assigned to GameObject: " + selectedGameObject.name);
                }
                else
                {
                        //Debug.LogWarning("GameObject not found.");
                }
            }
            else
            {
                //Debug.LogWarning("Failed to load script '" + scriptName + "'.");
            }
        }
        else
        {
            //Debug.Log("Script '" + scriptName + "' not found.");
        }
    }
}
