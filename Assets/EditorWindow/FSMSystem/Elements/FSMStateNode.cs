using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateNode : FSMNode
{
    private List<StateScript> _dataObjects;
    public override void Initialize(string nodeName, FSMGraphView graphView,Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.State;
        
        _dataObjects = new List<StateScript>(){new PatrolStateScript(), new ChaseStateScript(), new AttackStateScript()};

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

        foreach (FSMConnectionSaveData connection in Choices)
        {
            Port connectionPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);

            connectionPort.userData = connection;

            outputContainer.Add(connectionPort);
        }
        
        VisualElement customDataContainer = new VisualElement();
        //customDataContainer.AddToClassList("fsm-node_custom-data-container");
        
        GetScriptableObject();

        CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);
        
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
            Debug.Log(attribute);

            switch (result[1]){
               
                case "UnityEngine.GameObject":

                    string input = result[2];
                    string output = Regex.Replace(input, @"\s*\([^()]*\)", "");
                    
                    ObjectField objectField = new ObjectField()
                    {
                        objectType = typeof(GameObject),
                        value = GameObject.Find(output)
                    };      
                    objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], objectField.value);
                    });
                    objectField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(objectField);
                    break;
                case "System.Collections.Generic.List`1[UnityEngine.GameObject]":
                    
                    string inputList = result[2];
                    string outputList = Regex.Replace(inputList, @"\s*\([^()]*\)", "");
                    
                    ObjectField objectListField = new ObjectField()
                    {
                        objectType = typeof(GameObject),
                        value = GameObject.Find(outputList)
                    };
                    objectListField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], objectListField.value);
                    });
                    objectListField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(objectListField);
                    break;
                case "System.Single":
                    FloatField floatField = new FloatField()
                    {
                        value = float.Parse(result[2])
                    };
                    floatField.RegisterCallback<InputEvent>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], floatField.value);
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
                        StateScript.SetVariableValue(result[0], integerField.value);
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
                        StateScript.SetVariableValue(result[0], toggle.value);
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
                        StateScript.SetVariableValue(result[0], textField.value);
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
        try
        {
            StateScript.GetStateName();
        }
        catch (NullReferenceException e)
        {
            foreach (StateScript enemyState in _dataObjects)
            {
                if (enemyState.GetStateName() == StateName)
                {
                    StateScript = enemyState;
                }
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
