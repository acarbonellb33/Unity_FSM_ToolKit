using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMExtensionNode : FSMNode
{
    private bool isSwapped = false;
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.Extension;
        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "Extension Node",
        };
        Choices.Add(connectionSaveData);
        mainContainer.AddToClassList("fsm-node_main-container");
        extensionContainer.AddToClassList("fsm-node_extension-container");
    }

    public override void Draw()
    {
        Port inputPort = this.CreatePort("", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);
        inputContainer.AddToClassList("fsm-node_input-output-container");
        foreach (FSMConnectionSaveData connection in Choices)
        {
            outputPort = this.CreatePort("", Orientation.Horizontal, Direction.Output);

            outputPort.userData = connection;

            outputContainer.Add(outputPort);
            outputContainer.AddToClassList("fsm-node_input-output-container");
        }
        Button swapPortsButton = new Button(() => SwapPorts()) { text = "Swap Ports" };
        extensionContainer.Add(swapPortsButton);
        RefreshExpandedState();
    }
    
    private void SwapPorts()
    {
        // Get the children of input and output containers
        List<VisualElement> inputPorts = inputContainer.Children().ToList();
        List<VisualElement> outputPorts = outputContainer.Children().ToList();

        // Clear the containers
        inputContainer.Clear();
        outputContainer.Clear();

        // Add output ports to input container and vice versa
        if (isSwapped)
        {
            foreach (var port in outputPorts)
            {
                // Change port direction to input
                Port newPort = this.CreatePort(port.name);
                newPort.userData = port.userData;
                inputContainer.Add(newPort);
            }
            foreach (var port in inputPorts)
            {
                // Change port direction to output
                Port newPort = this.CreatePort(port.name, Orientation.Horizontal, Direction.Output);
                newPort.userData = port.userData;
                outputContainer.Add(newPort);
            }
        }
        else
        {
            foreach (var port in outputPorts)
            {
                // Change port direction to input
                Port newPort = this.CreatePort(port.name);
                newPort.userData = port.userData;
                outputContainer.Add(newPort);
            }
            foreach (var port in inputPorts)
            {
                // Change port direction to output
                Port newPort = this.CreatePort(port.name, Orientation.Horizontal, Direction.Output);
                newPort.userData = port.userData;
                inputContainer.Add(newPort);
            }
        }
        isSwapped = !isSwapped;
    }
    
}
