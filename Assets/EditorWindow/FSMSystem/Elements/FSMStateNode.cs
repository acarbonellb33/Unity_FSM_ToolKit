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
    private List<State> _scriptableObjects;
    public override void Initialize(string nodeName, FSMGraphView graphView,Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.State;
        
        _scriptableObjects = new List<State>() {State.CreateInstance<PatrolState>(), State.CreateInstance<AttackState>(), State.CreateInstance<ChaseState>()};

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
            Debug.Log(attribute);

            switch (result[1]){
               
                case "UnityEngine.GameObject":
                    Debug.Log("GameObject"+GameObject.Find(result[2]));
                    ObjectField objectField = new ObjectField()
                    {
                        objectType = typeof(GameObject),
                        value = GameObject.Find(result[2].Substring(0,result[2].IndexOf(' ')))
                    };      
                    objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                    {
                        StateScriptableObject.SetVariableValue(result[0], objectField.value);
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
           
        foreach (State enemyState in _scriptableObjects)
        {
            if (enemyState.GetStateName() == StateName)
            {
                StateScriptableObject = enemyState;
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
