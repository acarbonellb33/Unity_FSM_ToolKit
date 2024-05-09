using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMExtensionNode : FSMNode
{
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.Extension;
        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "Extension Node",
        };
        Choices.Add(connectionSaveData);
        mainContainer.AddToClassList("fsm-node_main-container-extension");
        extensionContainer.AddToClassList("fsm-node_extension-container");
    }

    public override void Draw()
    {
        mainContainer.Remove(titleContainer);
        // Add input port to the input container
        inputPort = this.CreatePort();
        inputContainer.Add(inputPort);
        inputContainer.AddToClassList("fsm-node_input-output-container-extension");

        // Add output ports to the output container
        foreach (FSMConnectionSaveData connection in Choices)
        {
            outputPort = this.CreatePort("", Orientation.Horizontal, Direction.Output);
            outputPort.userData = connection;
            outputContainer.Add(outputPort);
            outputContainer.AddToClassList("fsm-node_input-output-container-extension");
        }

        // Add the input and output containers to the main container
        mainContainer.Add(inputContainer);
        mainContainer.Add(outputContainer);

        // Add class to the main container for styling purposes
        mainContainer.AddToClassList("fsm-node_main-container");

        // Refresh the expanded state of the node
        RefreshExpandedState();
    }
    
    public override void SetStateName(string stateName)
    {
        StateName = stateName;
    }
}
