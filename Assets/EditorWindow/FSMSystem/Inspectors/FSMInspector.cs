using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FSMGraph))]
public class FSMInspector : Editor
{
    public FSMGraphSaveData graphContainer;
    private SerializedProperty selectedOptionIndexProp;
    Dictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();
    void OnEnable()
    {
        selectedOptionIndexProp = serializedObject.FindProperty("selectedOptionIndex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGraphContainerArea();

        if (graphContainer == null)
        {
            StopDrawing("Select a FSM Graph Container to see the rest of the Inspector.");
            return;
        }
        
        if (GUILayout.Button("Open FSM Graph"))
        {
            FSMEditorWindow.OpenWithSaveData(graphContainer);
        }
        
        if (GUILayout.Button("Add FSM Graph Component"))
        {
            AddComponentToGameObject();
        }

        

        serializedObject.ApplyModifiedProperties();
    }
    
    private void AddComponentToGameObject()
    {
        FSMGraph fsmGraph = (FSMGraph)target;
        MonoScript script = GetScript(graphContainer.FileName);
        if (script != null)
        {
            foreach (var node in graphContainer.Nodes)
            {
                MonoBehaviour instance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(GetScript(node.Name).GetClass());
            }
            MonoBehaviour newScriptInstance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(Type.GetType(graphContainer.FileName));
            
            MethodInfo dynamicMethod = script.GetClass().GetMethod("SetVariableValue");
                    
            if (dynamicMethod != null)
            {
                for (int i = 0; i < graphContainer.Nodes.Count; i++)
                {
                    dynamicMethod.Invoke(newScriptInstance,new object[]
                    {
                        char.ToLowerInvariant(graphContainer.Nodes[i].Name[0]) + graphContainer.Nodes[i].Name.Substring(1), 
                        FSMIOUtility.LoadNode(graphContainer.Nodes[i], graphContainer.FileName).StateScript
                    });
                }
            }
        } 
    }
    private MonoScript GetScript(string className)
    {
        string[] guids = AssetDatabase.FindAssets("t:Script " + className);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        }
        return null;
    }

    private void DrawGraphContainerArea()
    {
        FSMInspectorUtility.DrawHeader("FSM Graph Container");
        graphContainer = (FSMGraphSaveData)EditorGUILayout.ObjectField("Graph Container", graphContainer, typeof(FSMGraphSaveData), true);
        FSMInspectorUtility.DrawSpace();
    }

    private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
    {
        FSMInspectorUtility.DrawHelpBox(reason, messageType);
        FSMInspectorUtility.DrawSpace();
        FSMInspectorUtility.DrawHelpBox("You need to select a Graph for this component to work properly at Runtime!", MessageType.Warning); 
        serializedObject.ApplyModifiedProperties();
    }

    #region IndexMethods

    private void UpdateIndexOnNamesListUpdate(List<string> optionNames, SerializedProperty indexProperty, int oldSelectedPropertyIndex, string oldPropertyName, bool isOldPropertyNull)
    {
        if (isOldPropertyNull)
        {
            indexProperty.intValue = 0;

            return;
        }

        bool oldIndexIsOutOfBoundsOfNamesListCount = oldSelectedPropertyIndex > optionNames.Count - 1;
        bool oldNameIsDifferentThanSelectedName = oldIndexIsOutOfBoundsOfNamesListCount || oldPropertyName != optionNames[oldSelectedPropertyIndex];

        if (oldNameIsDifferentThanSelectedName)
        {
            if (optionNames.Contains(oldPropertyName))
            {
                indexProperty.intValue = optionNames.IndexOf(oldPropertyName);

                return;
            }

            indexProperty.intValue = 0;
        }
    } 

    #endregion
    
    [MenuItem("Assets/Create/ScriptableObjects/FSMNodesContainer")]
    public static void CreateFSMNodesContainer()
    {
        FSMNodesContainerSO asset = ScriptableObject.CreateInstance<FSMNodesContainerSO>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(path))
        {
            path = "Assets";
        }
        else if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
        {
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string fileName = asset.FileName;
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "NewFSMNodesContainer";
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + fileName + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
