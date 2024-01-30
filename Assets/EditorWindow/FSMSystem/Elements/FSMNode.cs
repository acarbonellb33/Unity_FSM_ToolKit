using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMNode : Node
{
    public string Id { get; set; }
    public string StateName { get; set; }
    public List<FSMConnectionSaveData> Choices { get; set; }
    public FSMNodeType NodeType { get; set; }
    public FSMGroup Group { get; set; }
    
    private FSMGraphView _graphView;
    public State StateScriptableObject { get; set; }

    public virtual void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        Id = Guid.NewGuid().ToString();
        StateName = nodeName;
        Choices = new List<FSMConnectionSaveData>();
        _graphView = graphView;
        SetPosition(new Rect(postition, Vector2.zero));
        
        StateScriptableObject = null;

        AddManipulators();
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", action => DisconnectPorts(inputContainer));
        evt.menu.AppendAction("Disconnect Output Ports", action => DisconnectPorts(outputContainer));
        evt.menu.AppendSeparator();
        base.BuildContextualMenu(evt);
    }

    public virtual void Draw()
    {
        TextField stateNameField = FSMElementUtility.CreateTextField(StateName, null, callback =>
        {
            if (Group == null)
            {
                _graphView.RemoveUngroupedNode(this);
                StateName = callback.newValue;
                _graphView.AddUngroupedNode(this);
                return;
            }
            FSMGroup currentGroup = Group;
            _graphView.RemoveGroupedNode(this, Group);
            StateName = callback.newValue;
            _graphView.AddGroupedNode(this, currentGroup);
        });
        stateNameField.AddClasses("fsm-node_textfield", "fsm-node_filename-textfield", "fsm-node_textfield_hidden");
        titleContainer.Insert(0, stateNameField);
        
        Port inputPort = this.CreatePort("Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);
    }

    #region Ports
    public void DisconnectAllPorts()
    {
        DisconnectPorts(inputContainer);
        DisconnectPorts(outputContainer);
    }
    private void DisconnectPorts(VisualElement container)
    {
        foreach(Port port in container.Children())
        {
            if (!port.connected)
            {
                continue;
            }
            _graphView.DeleteElements(port.connections);
        }
    }
    #endregion

    #region Styles
    public void SetErrorStyle(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }
    public void ResetStyle()
    {
        mainContainer.style.backgroundColor = new Color(29f/255f, 29f/255f, 30f/255f);
    }

    #endregion
    
    private void AddManipulators()
    {
        this.AddManipulator(CreateNodeContectualMenu());
    }
    private IManipulator CreateNodeContectualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Create Transition", menuActionEvent => Debug.Log("Create Transition")));
        
        return contextualMenuManipulator;
    }
}
