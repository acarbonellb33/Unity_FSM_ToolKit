using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FSMGraph))]
public class FSMInspector : Editor
{
    private SerializedProperty graphContainerProperty;
    private FSMGraphSaveData graphContainerData;
    
    private void OnEnable()
    {
        graphContainerProperty = serializedObject.FindProperty("graphContainer");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawGraphContainerArea();

        if (graphContainerProperty == null)
        {
            StopDrawing("Select a FSM Graph Container to see the rest of the Inspector.");
            return;
        }

        if (GUILayout.Button("Open FSM Graph"))
        {
            ((FSMGraphSaveData)graphContainerProperty.objectReferenceValue).GameObject = ((FSMGraph)target).gameObject.name;
            FSMEditorWindow.OpenWithSaveData(graphContainerProperty.objectReferenceValue as FSMGraphSaveData);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddComponentToGameObject()
    {
        FSMGraph fsmGraph = (FSMGraph)target;
        graphContainerData = (FSMGraphSaveData)graphContainerProperty.objectReferenceValue;
        MonoScript script = GetScript(graphContainerData.FileName);
        if (script != null)
        {
            foreach (var node in graphContainerData.Nodes)
            {
                if (node.NodeType != FSMNodeType.Initial)
                {
                    //MonoBehaviour instance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(GetScript(node.Name).GetClass());
                }
            }
            MonoBehaviour newScriptInstance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(Type.GetType(graphContainerData.FileName));
            
            MethodInfo dynamicMethod = script.GetClass().GetMethod("SetVariableValue");
                    
            if (dynamicMethod != null)
            {
                for (int i = 0; i < graphContainerData.Nodes.Count; i++)
                {
                    if (graphContainerData.Nodes[i].NodeType != FSMNodeType.Initial)
                    {
                        dynamicMethod.Invoke(newScriptInstance,new object[]
                        { 
                            char.ToLowerInvariant(graphContainerData.Nodes[i].Name[0]) + graphContainerData.Nodes[i].Name.Substring(1), 
                            FSMIOUtility.LoadNode(graphContainerData.Nodes[i], graphContainerData.FileName).StateScript
                        });
                    }
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
        FSMGraph fsmGraph = (FSMGraph)target;

        EditorGUI.BeginChangeCheck();

        FSMInspectorUtility.DrawHeader("FSM Graph Container");
        graphContainerProperty.DrawPropertyField();

        if (EditorGUI.EndChangeCheck())
        {
            graphContainerData = (FSMGraphSaveData)graphContainerProperty.objectReferenceValue;
            foreach(Component c in fsmGraph.gameObject.GetComponents<Component>())
            {
                if (c is StateScript || c is BehaviorScript)
                {
                    DestroyImmediate(c);
                }
            }
            if(graphContainerProperty.objectReferenceValue != null)AddComponentToGameObject();
        }
        FSMInspectorUtility.DrawSpace();
    }

    private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
    {
        FSMInspectorUtility.DrawHelpBox(reason, messageType);
        FSMInspectorUtility.DrawSpace();
        FSMInspectorUtility.DrawHelpBox("You need to select a Graph for this component to work properly at Runtime!", MessageType.Warning); 
        serializedObject.ApplyModifiedProperties();
    }
}
