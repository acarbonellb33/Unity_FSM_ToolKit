using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenCover.Framework.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

public class FSMTransitionNode : FSMNode
{
    private List<State> _scriptableObjects;
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        DialogueType = FSMDialogueType.Transition;
        
        _scriptableObjects = new List<State>() {ScriptableObject.CreateInstance<HearingCondition>()};

        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "New State",
        };
        Choices.Add(connectionSaveData);
        mainContainer.AddToClassList("fsm-node_main-container");
        extensionContainer.AddToClassList("fsm-node_extension-container");
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

        VisualElement customDataContainer = new VisualElement();
        //customDataContainer.AddToClassList("fsm-node_custom-data-container");

        GetScriptableObject();
        
        CreateStateAttribute(StateScriptableObject.InspectVariables(), customDataContainer);

        extensionContainer.Add(customDataContainer);
        
        RefreshExpandedState();
    }
    
    #region ScriptableObject Attributes
    private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
    {
        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");
    
        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');
                
            Label stateAttributeLabel = new Label(UpdateNameStyle(result[0]));
            stateAttributeLabel.AddToClassList("fsm-node_state-attribute-label");
            stateAttributeContainer.Add(stateAttributeLabel);
    
            switch (result[1])
            {
                case "System.Single":
                    FloatField floatField = new FloatField()
                    {
                        value = float.Parse(result[2])
                    };
                    floatField.RegisterCallback<InputEvent>(evt =>
                    {
                        StateScriptableObject.SetVariableValue(result[0], floatField.value);
                    });
                    floatField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(floatField);
                    break;
                case "System.Int32":
                    IntegerField integerField = new IntegerField()
                    {
                        value = int.Parse(result[2])
                    };
                    integerField.RegisterCallback<InputEvent>(evt =>
                    {
                        StateScriptableObject.SetVariableValue(result[0], integerField.value);
                    });
                    integerField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(integerField);
                    break;
                case "System.Boolean":
                    Toggle toggle = new Toggle()
                    { 
                        value = bool.Parse(result[2])
                    };
                    toggle.RegisterCallback<ClickEvent>(evt =>
                    {
                        StateScriptableObject.SetVariableValue(result[0], toggle.value);
                    });
                    toggle.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(toggle);
                    break;
                case "System.String":
                    TextField textField = new TextField()
                    {
                        value = result[2]
                    };
                    textField.RegisterCallback<InputEvent>(evt =>
                    {
                        StateScriptableObject.SetVariableValue(result[0], textField.value);
                    });
                    textField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(textField);
                    break;
                default:
                    break;
            }
        }
        customDataContainer.Add(stateAttributeContainer);
    }
    private void GetScriptableObject()
    {
        if(StateScriptableObject != null) return;
        
        foreach (State conditionState in _scriptableObjects)
        {
            if (conditionState.GetStateName() == StateName)
            {
                StateScriptableObject = conditionState;
            }
        }
    }
    private string UpdateNameStyle(string name)
    {
        string[] fullName = Regex.Split(name, @"(?=[A-Z])");
        string resultString = "";

        for(int i = 0; i < fullName.Length; i++)
        {
            if (i == 0)
            {
                resultString = char.ToUpper(fullName[i][0]) + fullName[i].Substring(1);
            }
            else
            {
                resultString += " " + fullName[i];
            }
        }
        return resultString;
    }
    
    #endregion
}
