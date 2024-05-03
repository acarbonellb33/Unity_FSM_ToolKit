using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

public class FSMDualTransitionNode : FSMNode
{
    private List<StateScriptData> _dataObjects;
    public override void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.Transition;
        
        _dataObjects = new List<StateScriptData>(){new SeeingData(), new HearingData(), new DistanceData(), new HealthData()};
     
        FSMConnectionSaveData connectionSaveData1 = new FSMConnectionSaveData()
        {
            Text = "True",
        };
        FSMConnectionSaveData connectionSaveData2 = new FSMConnectionSaveData()
        {
            Text = "False",
        };
        Choices.Add(connectionSaveData1);
        Choices.Add(connectionSaveData2);
        mainContainer.AddToClassList("fsm-node_main-container");
        extensionContainer.AddToClassList("fsm-node_extension-container");
    }
    
    public override void Draw()
    {
        base.Draw();
        
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
        if (StateName == "Hearing")
        {
            CreateHearingAttributes(attributes, customDataContainer);
            return;
        }
        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");
    
        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');

            switch (result[1])
            {
                case "FSMOperands":
                    EnumField enumField = new EnumField();
                    enumField.Init(new FSMOperands());
                    enumField.value = (Enum)Enum.Parse(typeof(FSMOperands), result[2]);
                    enumField.label = UpdateNameStyle(result[0]);
                    enumField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], enumField.value);
                    });
                    enumField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(enumField);
                    break;
                case "UnityEngine.GameObject":

                    string input = result[2];
                    string output = Regex.Replace(input, @"\s*\([^()]*\)", "");
                    
                    ObjectField objectField = new ObjectField()
                    {
                        label = UpdateNameStyle(result[0]),
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
                case "System.Single":
                    FloatField floatField = new FloatField()
                    {
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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

    private void CreateHearingAttributes(List<string> attributes, VisualElement customDataContainer)
    {
        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container-hearing");
        
        Label label = FSMElementUtility.CreateLabel("Hearing Range");
        label.AddToClassList("fsm-node_state-attribute-container-hearing-label");
        stateAttributeContainer.Add(label);
    
        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');

            switch (result[1])
            {
                case "FSMOperands":
                    EnumField enumField = new EnumField();
                    enumField.Init(new FSMOperands());
                    enumField.value = (Enum)Enum.Parse(typeof(FSMOperands), result[2]);
                    //enumField.label = UpdateNameStyle(result[0]);
                    enumField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], enumField.value);
                    });
                    enumField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(enumField);
                    break;
                case "UnityEngine.GameObject":

                    string input = result[2];
                    string output = Regex.Replace(input, @"\s*\([^()]*\)", "");
                    
                    ObjectField objectField = new ObjectField()
                    {
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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
                        label = UpdateNameStyle(result[0]),
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
        catch (NullReferenceException)
        {
            foreach (StateScriptData enemyState in _dataObjects)
            {
                string name = StateName.Split(' ')[0];
                if (enemyState.GetStateName() == name)
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
