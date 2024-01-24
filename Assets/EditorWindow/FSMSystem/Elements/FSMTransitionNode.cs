using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMTransitionNode : FSMNode
{
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        DialogueType = FSMDialogueType.Transition;
        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "New State",
        };
        Choices.Add(connectionSaveData);
    }
    
    public override void Draw()
    {
        base.Draw();
        
        foreach (FSMConnectionSaveData connection in Choices)
        {
            Port connectionPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);

            connectionPort.userData = connection;

            outputContainer.Add(connectionPort);
        }

        RefreshExpandedState();
    }
}
