#if UNITY_EDITOR
namespace FSM.Nodes
{
    using System;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.AI;
    using EditorWindow.FSMSystem.BehaviorScripts;
    using EditorWindow.FSMSystem.Data.Save;
    using EditorWindow.FSMSystem.Utilities;
    using Enemy;
    using Enumerations;
    using States;
    
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(EnemyHealthSystem))]
    [AddComponentMenu("FSM AI/FSM AI")]
    public class FSMGraph : MonoBehaviour
    {
        [SerializeField] private FSMGraphSaveData graphContainer;

        public void UpdateComponentOfGameObject()
        {
            FSMGraph fsmGraph = this;
            foreach (Component c in fsmGraph.gameObject.GetComponents<Component>())
            {
                if (c is StateScript || c is BehaviorScript)
                {
                    DestroyImmediate(c);
                }
            }

            if (graphContainer != null)
            {
                MonoScript script = GetScript(graphContainer.FileName);
                if (script != null)
                {
                    /*foreach (var node in graphContainer.Nodes)
                    {
                        if (node.NodeType != FSMNodeType.Initial)
                        {
                            MonoBehaviour instance = (MonoBehaviour)fsmGraph.gameObject.AddComponent(GetScript(node.Name).GetClass());
                        }
                    }*/

                    MonoBehaviour newScriptInstance =
                        (MonoBehaviour)fsmGraph.gameObject.AddComponent(Type.GetType("EditorWindow.FSMSystem.BehaviorScripts."+graphContainer.FileName));

                    MethodInfo dynamicMethod = script.GetClass().GetMethod("SetVariableValue");

                    if (dynamicMethod != null)
                    {
                        for (int i = 0; i < graphContainer.Nodes.Count; i++)
                        {
                            if (graphContainer.Nodes[i].NodeType != FSMNodeType.Initial &&
                                graphContainer.Nodes[i].NodeType != FSMNodeType.Extension)
                            {
                                dynamicMethod.Invoke(newScriptInstance, new object[]
                                {
                                    char.ToLowerInvariant(graphContainer.Nodes[i].Name[0]) +
                                    graphContainer.Nodes[i].Name.Substring(1),
                                    FSMIOUtility.LoadNode(graphContainer.Nodes[i], graphContainer.FileName).StateScript
                                });
                            }
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
    }
}
#endif