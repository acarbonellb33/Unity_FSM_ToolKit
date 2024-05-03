using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMNode : Node
{
    public string Id { get; set; }
    public string StateName { get; set; }
    public List<FSMConnectionSaveData> Choices { get; set; }
    public FSMNodeType NodeType { get; set; }
    public FSMGroup Group { get; set; }
    
    protected FSMGraphView graphView;
    public StateScriptData StateScript { get; set; }
    
    private Label stateNameField;

    private Port inputPort;
    protected Port outputPort;

    public virtual void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        Id = Guid.NewGuid().ToString();
        StateName = nodeName;
        Choices = new List<FSMConnectionSaveData>();
        this.graphView = graphView;
        SetPosition(new Rect(postition, Vector2.zero));
        
        StateScript = null;

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
        
        stateNameField = FSMElementUtility.CreateLabel(StateName, callback =>
        {
            if (Group == null)
            {
                //graphView.RemoveUngroupedNode(this);
                StateName = callback.newValue;
                //graphView.AddUngroupedNode(this);
                return;
            }
            FSMGroup currentGroup = Group;
            graphView.RemoveGroupedNode(this, Group);
            StateName = callback.newValue;
            graphView.AddGroupedNode(this, currentGroup);
        });
        stateNameField.AddClasses("fsm-node_label");
        titleContainer.Insert(0, stateNameField);
        
        inputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);
        inputContainer.AddToClassList("fsm-node_input-output-container");
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
            graphView.DeleteElements(port.connections);
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

    public void SetStateName(string stateName)
    {
        StateName = stateName;
        stateNameField.text = stateName;
    }
    
    public void SetPortColor(Color color, Direction direction)
    {
        if (direction == Direction.Input)
        {
            inputPort.portColor = color;
        }
        else
        {
            outputPort.portColor = color;
        }
    }

    public virtual void SetAnimatorSaveData(FSMAnimatorSaveData animatorSaveData) {}
    public virtual FSMAnimatorSaveData GetAnimatorSaveData() { return null; }
}
