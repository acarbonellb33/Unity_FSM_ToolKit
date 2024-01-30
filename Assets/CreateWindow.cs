using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CreateWindow : EditorWindow
{
    private string className;
    private FSMGraphSaveData saveData;
    private ObjectField _gameObjectField;
    public void Initialize(string nameClass, FSMGraphSaveData data)
    {
        className = nameClass;
        saveData = data;
        var window = GetWindow<CreateWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        Label titleLabel = FSMElementUtility.CreateLabel("Assign New AI Enemy Script", null);
        Label infoLabel =FSMElementUtility.CreateLabel("Here you can add the new generated script to the enemy GameObject.\nNow select the gameObject from the scene that you want:", null);
        _gameObjectField = new ObjectField("Enemy GameObject:");
        _gameObjectField.objectType = typeof(GameObject);
        Button button = FSMElementUtility.CreateButton("Add Component", AddComponentToGameObject);
        
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(titleLabel);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(infoLabel);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(_gameObjectField);
        rootVisualElement.Add(new IMGUIContainer(() => { EditorGUILayout.Space(10); }));
        rootVisualElement.Add(button);
    }
    
    
    private void AddComponentToGameObject()
    {
        string[] guids = AssetDatabase.FindAssets("t:Script " + className);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            if (script != null)
            {
                if (_gameObjectField.value != null)
                {
                    MonoBehaviour newScriptInstance = (MonoBehaviour)((GameObject)_gameObjectField.value).AddComponent(script.GetClass());
                    MethodInfo dynamicMethod = script.GetClass().GetMethod("SetVariableValue");
                    
                    if (dynamicMethod != null)
                    {
                        for (int i = 0; i < saveData.Nodes.Count; i++)
                        {
                            dynamicMethod.Invoke(newScriptInstance,new object[]
                            {
                                char.ToLowerInvariant(saveData.Nodes[i].Name[0]) + saveData.Nodes[i].Name.Substring(1), saveData.Nodes[i].ScriptableObject
                            });
                        }
                    }
                }
            }
        }
        GetWindow<CreateWindow>().Close();
    }
}
