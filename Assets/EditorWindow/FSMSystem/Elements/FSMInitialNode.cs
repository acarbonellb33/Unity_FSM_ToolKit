using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMInitialNode : FSMNode
{
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.Initial;
        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "Initial Node",
        };
        Choices.Add(connectionSaveData);
        mainContainer.AddToClassList("fsm-node_main-container-initial");
        extensionContainer.AddToClassList("fsm-node_extension-container");
    }

    public override void Draw()
    {
        Label stateNameField = FSMElementUtility.CreateLabel(StateName, callback =>
        {
            if (Group == null)
            {
                graphView.RemoveUngroupedNode(this);
                StateName = callback.newValue;
                graphView.AddUngroupedNode(this);
                return;
            }
            FSMGroup currentGroup = Group;
            graphView.RemoveGroupedNode(this, Group);
            StateName = callback.newValue;
            graphView.AddGroupedNode(this, currentGroup);
        });
        stateNameField.AddClasses("fsm-node_label");
        titleContainer.Insert(0, stateNameField);
        
        foreach (FSMConnectionSaveData connection in Choices)
        {
            outputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);
            if (!outputPort.connected)
            {
                // Apply orange color to the port
                outputPort.portColor = Color.red;
            }
            outputPort.userData = connection;

            outputContainer.Add(outputPort);
            outputContainer.AddToClassList("fsm-node_input-output-container");
        }
        RefreshExpandedState();
    }
}
