#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Inspectors
{
    using System;
    using BehaviorScripts;
    using Data.Save;
    using UnityEditor;
    using UnityEngine;
    using Utilities;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes;
    using FSM.Nodes.States;
    /// <summary>
    /// Custom inspector for FSM graphs, enabling interaction with FSM graph data in the Unity Editor.
    /// </summary>
    [CustomEditor(typeof(FsmGraph))]
    public class FsmInspector : Editor
    {
        private SerializedProperty _graphContainerProperty;
        private FsmGraphSaveData _graphContainerData;

        private void OnEnable()
        {
            _graphContainerProperty = serializedObject.FindProperty("graphContainer");
        }
        /// <summary>
        /// Overrides the default Inspector GUI to add custom drawing logic. This way you can add your created FSM Graphs to the GameObject. If the FSM Graph has saved data, the behavior will be added to the GameObject as well. You can swap graph or edit the current one by clicking the button "Open FSM Graph".
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawGraphContainerArea();

            if (_graphContainerProperty == null)
            {
                StopDrawing("Select a FSM Graph Container to see the rest of the Inspector.");
                return;
            }

            if (GUILayout.Button("Open FSM Graph"))
            {
                ((FsmGraphSaveData)_graphContainerProperty.objectReferenceValue).GameObject =
                    ((FsmGraph)target).gameObject.name;
                FsmEditorWindow.OpenWithSaveData((FsmGraphSaveData)_graphContainerProperty.objectReferenceValue);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void AddComponentToGameObject()
        {
            var fsmGraph = (FsmGraph)target;
            _graphContainerData = (FsmGraphSaveData)_graphContainerProperty.objectReferenceValue;
            var script = GetScript(_graphContainerData.FileName);
            if (script != null)
            {
                foreach (var node in _graphContainerData.Nodes)
                {
                    if (node.NodeType != FsmNodeType.Initial)
                    {
                        //MonoBehaviour instance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(GetScript(node.Name).GetClass());
                    }
                }

                var newScriptInstance =
                    (MonoBehaviour)fsmGraph.gameObject.AddComponent(Type.GetType("EditorWindow.FSMSystem.BehaviorScripts."+_graphContainerData.FileName));

                var dynamicMethod = script.GetClass().GetMethod("SetVariableValue");

                if (dynamicMethod != null)
                {
                    for (var i = 0; i < _graphContainerData.Nodes.Count; i++)
                    {
                        if (_graphContainerData.Nodes[i].NodeType != FsmNodeType.Initial &&
                            _graphContainerData.Nodes[i].NodeType != FsmNodeType.Extension)
                        {
                            var nodeName = char.ToLowerInvariant(_graphContainerData.Nodes[i].Name[0]) +
                                              _graphContainerData.Nodes[i].Name.Substring(1);
                            nodeName = nodeName.Replace(" ", "");
                            dynamicMethod.Invoke(newScriptInstance, new object[]
                            {
                                nodeName,
                                FsmIOUtility.LoadNode(_graphContainerData.Nodes[i], _graphContainerData.FileName)
                                    .StateScript
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
            FsmGraph fsmGraph = (FsmGraph)target;

            EditorGUI.BeginChangeCheck();

            FsmInspectorUtility.DrawHeader("FSM Graph Container");
            _graphContainerProperty.DrawPropertyField();

            if (EditorGUI.EndChangeCheck())
            {
                _graphContainerData = (FsmGraphSaveData)_graphContainerProperty.objectReferenceValue;
                foreach (Component c in fsmGraph.gameObject.GetComponents<Component>())
                {
                    if (c is StateScript || c is BehaviorScript)
                    {
                        DestroyImmediate(c);
                    }
                }

                if (_graphContainerProperty.objectReferenceValue != null) AddComponentToGameObject();
            }

            FsmInspectorUtility.DrawSpace();
        }

        private void StopDrawing(string reason, MessageType messageType = MessageType.Info)
        {
            FsmInspectorUtility.DrawHelpBox(reason, messageType);
            FsmInspectorUtility.DrawSpace();
            FsmInspectorUtility.DrawHelpBox(
                "You need to select a Graph for this component to work properly at Runtime!", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif