using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateNode : FSMNode
{
    public override void Initialize(FSMGraphView graphView,Vector2 postition)
    {
        base.Initialize(graphView, postition);
        DialogueType = FSMDialogueType.State;

        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "New Transition",
        };
        Choices.Add(connectionSaveData);
        
        mainContainer.AddToClassList("fsm-node_main-container");
        extensionContainer.AddToClassList("fsm-node_extension-container");
    }
    
    public override void Draw()
    {
        base.Draw();
        
        Button addChoiceButton = FSMElementUtility.CreateButton("Add Transition", () =>
        {
            FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
            {
                Text = "New Transition",
            };
            Choices.Add(connectionSaveData);
            
            Port outputPort = CreateTransitionPort("New Transition");
            outputContainer.Add(outputPort);
        });
        addChoiceButton.AddToClassList("fsm-node_button");
        
        mainContainer.Insert(1,addChoiceButton);

        foreach (FSMConnectionSaveData choice in Choices)
        {
            Port outputPort = CreateTransitionPort(choice);
            outputContainer.Add(outputPort);
        }
        RefreshExpandedState();
    }

    #region CreateTransitionPort
    private Port CreateTransitionPort(object userData)
    {
        Port port = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
        port.userData = userData;
        FSMConnectionSaveData connectionData = (FSMConnectionSaveData) userData;
                
        Button deleteChoiceButton = FSMElementUtility.CreateButton("X");
        deleteChoiceButton.AddToClassList("fsm-node_button");
               
        TextField choiceTextField = FSMElementUtility.CreateTextField(connectionData.Text, null, callback =>
        {
            connectionData.Text = callback.newValue;
        });
        choiceTextField.AddClasses("fsm-node_textfield", "fsm-node_choice-textfield", "fsm-node_textfield_hidden");

        port.Add(choiceTextField);
        port.Add(deleteChoiceButton);
        return port;
    }
    #endregion
}
