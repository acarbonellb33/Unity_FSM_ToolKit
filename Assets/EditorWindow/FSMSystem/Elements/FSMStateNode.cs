using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateNode : FSMNode
{
    private List<StateScriptData> _dataObjects;
    public override void Initialize(string nodeName, FSMGraphView graphView,Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.State;
        
        _dataObjects = new List<StateScriptData>(){new PatrolData(), new ChaseData(), new AttackData(), new SearchData()};

        FSMConnectionSaveData connectionSaveData = new FSMConnectionSaveData()
        {
            Text = "Condition",
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

        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");
    
        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');

            switch (result[1]){
               
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
                case "System.Collections.Generic.List`1[UnityEngine.GameObject]":
                    
                    string inputList = result[2];
                    if (inputList != "")
                    {
                        List<GameObject> outputGameObjectsList = new List<GameObject>();
                        outputGameObjectsList = inputList.Split('/').Select(GameObject.Find).ToList();
                        foreach (var value in outputGameObjectsList)
                        {
                            string outputValue = Regex.Replace(value.ToString(), @"\s*\([^()]*\)", "");
                            
                            ObjectField objectListField = new ObjectField()
                            {
                                label = UpdateNameStyle(result[0]),
                                objectType = typeof(GameObject),
                                value = GameObject.Find(outputValue)
                            };
                            objectListField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                            {
                                StateScript.SetVariableValue(result[0], objectListField.value);
                            });
                                                
                            Button deleteChoiceButton = FSMElementUtility.CreateButton("X", () =>
                            {
                                RemovePatrolPoint(objectListField);
                                stateAttributeContainer.Remove(objectListField);
                            });
                        
                            deleteChoiceButton.AddToClassList("fsm-node_button");
                            objectListField.AddToClassList("fsm-node_state-attribute-field");
                                                
                            objectListField.Add(deleteChoiceButton);
                            stateAttributeContainer.Add(objectListField);
                        }
                    }
                    
                    Button addChoiceButton = null;
                    addChoiceButton = FSMElementUtility.CreateButton("Add Patrol Point", () =>
                    {
                        int indexToAdd = stateAttributeContainer.IndexOf(addChoiceButton);
                        if (indexToAdd != -1)
                        {
                            ObjectField objectListField = new ObjectField()
                            {
                                label = UpdateNameStyle(result[0]),
                                objectType = typeof(GameObject)
                            };
                            objectListField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                            {
                                StateScript.SetVariableValue(result[0], objectListField.value);
                            });

                            Button deleteChoiceButton = FSMElementUtility.CreateButton("X", () =>
                            {
                                RemovePatrolPoint(objectListField);
                                stateAttributeContainer.Remove(objectListField);
                            });

                            deleteChoiceButton.AddToClassList("fsm-node_button");
                            objectListField.AddToClassList("fsm-node_state-attribute-field");
                            
                            objectListField.Add(deleteChoiceButton);
                            stateAttributeContainer.Insert(indexToAdd, objectListField);
                            
                            CreateAndAddGameObject(objectListField);
                        }
                    });
                    
                    addChoiceButton.AddToClassList("fsm-node_button");
                    stateAttributeContainer.Add(addChoiceButton);
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
    private void CreateAndAddGameObject(ObjectField objectListField)
    {
        FSMGraphSaveData graphContainerData = GetGraphData(graphView.GetWindow().GetFileName());
        MonoScript script = GetScript(graphView.GetWindow().GetFileName());
        if (script != null)
        {
            BehaviorScript newScriptInstance = (BehaviorScript)GameObject.Find(graphContainerData.GameObject).GetComponent(Type.GetType(graphContainerData.FileName));
            MethodInfo dynamicMethod = script.GetClass().GetMethod("AddObjectToList");
            if (dynamicMethod != null)
            {
                object o = dynamicMethod.Invoke(newScriptInstance,new object[] {});
                if (o != null)
                {
                    objectListField.value = (GameObject)o;
                }
            }
        }
    }
    private void RemovePatrolPoint(ObjectField objectListField)
    { 
        Debug.Log("RemovePatrolPoint");
        FSMGraphSaveData graphContainerData = GetGraphData(graphView.GetWindow().GetFileName());
        MonoScript script = GetScript(graphView.GetWindow().GetFileName());
        if (script != null)
        {
            BehaviorScript newScriptInstance = (BehaviorScript)GameObject.Find(graphContainerData.GameObject).GetComponent(Type.GetType(graphContainerData.FileName));
            MethodInfo dynamicMethod = script.GetClass().GetMethod("RemoveObjectFromList");
            if (dynamicMethod != null)
            {
                dynamicMethod.Invoke(newScriptInstance,new object[]
                {
                    (GameObject)objectListField.value
                });
            }
        }
    }
    private MonoScript GetScript(string className)
    {
        string[] guids = AssetDatabase.FindAssets("t:Script " + className);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        }
        return null;
    }
    private FSMGraphSaveData GetGraphData(string className)
    {
        string[] guids = AssetDatabase.FindAssets("t:FSMGraphSaveData  " + className);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<FSMGraphSaveData>(path);
        }
        return null;
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
