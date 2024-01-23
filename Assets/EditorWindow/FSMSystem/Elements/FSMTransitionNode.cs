using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMTransitionNode : FSMNode
{
    public override void Initialize(FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(graphView, postition);
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

        Port outputPort = this.CreatePort("Next Transition", Orientation.Horizontal, Direction.Output);
        outputContainer.Add(outputPort);
        
        RefreshExpandedState();
    }
}
