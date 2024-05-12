#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using System.Collections.Generic;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Elements;
    using FSM.Enumerations;
    public class FsmSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private FsmGraphView _graphView;
        private Texture2D _indentationIcon;

        public void Initialize(FsmGraphView graphView)
        {
            _graphView = graphView;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, Color.clear);
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>()
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
                }
            };
            return tree;
        }

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
                default:
                    return false;
            }
        }
    }
}
#endif