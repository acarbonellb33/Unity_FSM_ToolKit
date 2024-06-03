#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Elements;
    using FSM.Enumerations;
    /// <summary>
    /// ScriptableObject used to create the search window for adding nodes to the FSM graph.
    /// </summary>
    public class FsmSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private FsmGraphView _graphView;
        private Texture2D _indentationIcon;
        /// <summary>
        /// Initializes the search window with the specified FSM graph view.
        /// </summary>
        /// <param name="graphView">The FSM graph view to initialize the search window with.</param>
        public void Initialize(FsmGraphView graphView)
        {
            _graphView = graphView;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, Color.clear);
            _indentationIcon.Apply();
        }
        /// <summary>
        /// Creates the search tree for the search window.
        /// </summary>
        /// <param name="context">The search window context.</param>
        /// <returns>The search tree entries.</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node")),
                new SearchTreeGroupEntry(new GUIContent("State Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Attack", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2,

                },
                new SearchTreeEntry(new GUIContent("Patrol", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Idle", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Chase", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Flee", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Search", _indentationIcon))
                {
                    userData = FsmNodeType.State,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Transition Node"), 1),
                new SearchTreeEntry(new GUIContent("Hearing", _indentationIcon))
                {
                    userData = FsmNodeType.Transition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Seeing", _indentationIcon))
                {
                    userData = FsmNodeType.Transition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Distance", _indentationIcon))
                {
                    userData = FsmNodeType.Transition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Health", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("NextState", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Dual Transition Node"), 1),
                new SearchTreeEntry(new GUIContent("Hearing", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Seeing", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Distance", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Health", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("NextState", _indentationIcon))
                {
                    userData = FsmNodeType.DualTransition,
                    level = 2
                },
                new SearchTreeEntry(new GUIContent("Create Custom State", _indentationIcon))
                {
                    userData = FsmNodeType.CustomState,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Create Custom Condition", _indentationIcon))
                {
                    userData = FsmNodeType.CustomCondition,
                    level = 1
                },
                new SearchTreeEntry(new GUIContent("Create Variable Node", _indentationIcon))
                {
                    userData = FsmNodeType.Variable,
                    level = 1
                }
            };
            return tree;
        }
        /// <summary>
        /// Handles the selection of an entry in the search window. Creating a node based on the selected entry.
        /// </summary>
        /// <param name="searchTreeEntry">The selected search tree entry.</param>
        /// <param name="context">The search window context.</param>
        /// <returns>True if the entry was successfully selected, otherwise false.</returns>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = _graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (searchTreeEntry.userData)
            {
                case FsmNodeType.State:
                    FsmStateNode stateNode = (FsmStateNode)_graphView.CreateNode(searchTreeEntry.name,
                        localMousePosition, FsmNodeType.State);
                    _graphView.AddElement(stateNode);
                    return true;
                case FsmNodeType.Transition:
                    FsmTransitionNode transitionNode = (FsmTransitionNode)_graphView.CreateNode(searchTreeEntry.name,
                        localMousePosition, FsmNodeType.Transition);
                    _graphView.AddElement(transitionNode);
                    return true;
                case FsmNodeType.DualTransition:
                    FsmDualTransitionNode dualTransitionNode =
                        (FsmDualTransitionNode)_graphView.CreateNode(searchTreeEntry.name, localMousePosition,
                            FsmNodeType.DualTransition);
                    _graphView.AddElement(dualTransitionNode);
                    return true;
                case FsmNodeType.CustomState:
                    FsmCustomStateNode customStateNode = (FsmCustomStateNode)_graphView.CreateNode(searchTreeEntry.name,
                        localMousePosition, FsmNodeType.CustomState);
                    _graphView.AddElement(customStateNode);
                    return true;
                case FsmNodeType.CustomCondition:
                    FsmCustomConditionNode customConditionNode = (FsmCustomConditionNode)_graphView.CreateNode(searchTreeEntry.name,
                        localMousePosition, FsmNodeType.CustomCondition);
                    _graphView.AddElement(customConditionNode);
                    return true;
                case FsmNodeType.Variable:
                    FsmVariableNode variableNode = (FsmVariableNode)_graphView.CreateNode(searchTreeEntry.name,
                        localMousePosition, FsmNodeType.Variable);
                    _graphView.AddElement(variableNode);
                    return true;
                default:
                    return false;
            }
        }
    }
}
#endif