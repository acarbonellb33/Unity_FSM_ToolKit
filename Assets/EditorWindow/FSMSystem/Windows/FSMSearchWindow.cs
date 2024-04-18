using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FSMSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private FSMGraphView _graphView;
    private Texture2D _indentationIcon;
    public void Initialize(FSMGraphView graphView)
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
                userData = FSMNodeType.State,
                level = 2,
                
            },new SearchTreeEntry(new GUIContent("Patrol", _indentationIcon))
            {
                userData = FSMNodeType.State,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Idle", _indentationIcon))
            {
                userData = FSMNodeType.State,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Chase", _indentationIcon))
            {
                userData = FSMNodeType.State,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Search", _indentationIcon))
            {
                userData = FSMNodeType.State,
                level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Transition Node"), 1),
            new SearchTreeEntry(new GUIContent("Hearing", _indentationIcon))
            {
                userData = FSMNodeType.Transition,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Seeing", _indentationIcon))
            {
                userData = FSMNodeType.Transition,
                level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Dual Transition Node"), 1),
            new SearchTreeEntry(new GUIContent("Hearing", _indentationIcon))
            {
                userData = FSMNodeType.DualTransition,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Seeing", _indentationIcon))
            {
                userData = FSMNodeType.DualTransition,
                level = 2
            },
            new SearchTreeEntry(new GUIContent("Create Group", _indentationIcon))
            {
                userData = new Group(),
                level = 1
            }
        };
        return tree;
    }
    
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        Vector2 localMousePosition = _graphView.GetLocalMousePosition(context.screenMousePosition,true);
        switch (searchTreeEntry.userData)
        {
            case FSMNodeType.State:
                FSMStateNode stateNode = (FSMStateNode) _graphView.CreateNode(searchTreeEntry.name,localMousePosition, FSMNodeType.State);
                _graphView.AddElement(stateNode);
                return true;
            case FSMNodeType.Transition:
                FSMTransitionNode transitionNode = (FSMTransitionNode) _graphView.CreateNode(searchTreeEntry.name,localMousePosition, FSMNodeType.Transition);
                _graphView.AddElement(transitionNode);
                return true;
            case FSMNodeType.DualTransition:
                FSMDualTransitionNode dualTransitionNode = (FSMDualTransitionNode) _graphView.CreateNode(searchTreeEntry.name,localMousePosition, FSMNodeType.DualTransition);
                _graphView.AddElement(dualTransitionNode);
                return true;
            case Group _:
                _graphView.CreateGroup("New Group", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
