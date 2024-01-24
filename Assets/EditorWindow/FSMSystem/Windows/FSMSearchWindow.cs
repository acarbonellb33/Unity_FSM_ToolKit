using System.Collections;
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
            new SearchTreeEntry(new GUIContent("State Node", _indentationIcon))
            {
                userData = FSMDialogueType.State,
                level = 1
            },
            new SearchTreeEntry(new GUIContent("Transition Node", _indentationIcon))
            {
                userData = FSMDialogueType.Transition,
                level = 1
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
            case FSMDialogueType.State:
                FSMStateNode stateNode = (FSMStateNode) _graphView.CreateNode("StateName",localMousePosition, FSMDialogueType.State);
                _graphView.AddElement(stateNode);
                return true;
            case FSMDialogueType.Transition:
                FSMTransitionNode transitionNode = (FSMTransitionNode) _graphView.CreateNode("TransitionName",localMousePosition, FSMDialogueType.Transition);
                _graphView.AddElement(transitionNode);
                return true;
            case Group _:
                _graphView.CreateGroup("New Group", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
