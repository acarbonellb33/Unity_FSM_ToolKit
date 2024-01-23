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
                level = 1,
                userData = FSMDialogueType.State
            },
            new SearchTreeEntry(new GUIContent("Transition Node", _indentationIcon))
            {
                level = 1,
                userData = FSMDialogueType.Transition
            },
            new SearchTreeEntry(new GUIContent("Create Group", _indentationIcon))
            {
                level = 1,
                userData = new Group()
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
                FSMStateNode stateNode = _graphView.CreateState(localMousePosition);
                _graphView.AddElement(stateNode);
                return true;
            case FSMDialogueType.Transition:
                FSMTransitionNode transitionNode = _graphView.CreateTransition(localMousePosition);
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
