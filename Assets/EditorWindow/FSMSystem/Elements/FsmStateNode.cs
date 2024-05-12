#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Data.Save;
    using Utilities;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.States;
    using FSM.Nodes.States.StatesData;
    using FSM.Utilities;
    public class FsmStateNode : FsmNode
    {
        private List<StateScriptData> _dataObjects;

        public override void Initialize(string nodeName, FsmGraphView graphView, Vector2 postition)
        {
            base.Initialize(nodeName, graphView, postition);
            NodeType = FsmNodeType.State;

            _dataObjects = new List<StateScriptData>()
                { new PatrolData(), new ChaseData(), new AttackData(), new SearchData() };

            var connectionSaveData = new FsmConnectionSaveData()
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

            foreach (var connection in Choices)
            {
                OutputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Multi);
                if (!OutputPort.connected)
                {
                    OutputPort.portColor = Color.red;
                }

                OutputPort.userData = connection;

                outputContainer.Add(OutputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }

            var showPopupToggle = new Toggle()
            {
                label = "Enable Animator Trigger",
                value = HasAnimatorTrigger,
            };
            showPopupToggle.RegisterValueChangedCallback(_ => { ShowAnimatorParameterDropdown(showPopupToggle); });
            showPopupToggle.AddToClassList("fsm-node_toggle");


            var customDataContainer = new VisualElement();
            //customDataContainer.AddToClassList("fsm-node_custom-data-container");

            GetScriptableObject();

            CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);

            extensionContainer.Add(showPopupToggle);
            if (showPopupToggle.value) AddDropdownFields();
            extensionContainer.Add(customDataContainer);

            mainContainer.style.backgroundColor = new Color(200f / 255f, 250f / 255f, 100f / 255f);

            RefreshExpandedState();
        }

        #region ScriptableObject Attributes

        private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
        {

            var stateAttributeContainer = new VisualElement();
            stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");

            foreach (var attribute in attributes)
            {
                var result = attribute.Split(',');
                switch (result[1])
                {

                    case "UnityEngine.GameObject":

                        var input = result[2];
                        var output = Regex.Replace(input, @"\s*\([^()]*\)", "");

                        var objectField = new ObjectField()
                        {
                            label = UpdateNameStyle(result[0]),
                            objectType = typeof(GameObject),
                            value = GameObject.Find(output)
                        };
                        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], evt.newValue);
                        });
                        objectField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(objectField);
                        break;
                    case "System.Collections.Generic.List`1[System.String]":

                        var inputList = result[2];
                        if (inputList != "")
                        {
                            var outputGameObjectsList = inputList.Split('/')
                                .Select(FsmIOUtility.FindGameObjectWithId<IDGenerator>).ToList();

                            foreach (var value in outputGameObjectsList)
                            {
                                var objectListField = new ObjectField()
                                {
                                    label = UpdateNameStyle(result[0]),
                                    objectType = typeof(GameObject),
                                    value = value
                                };
                                objectListField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                                {
                                    try
                                    {
                                        if (evt.previousValue == null)
                                        {
                                            StateScript.RemoveVariable(result[0], "");
                                        }
                                        else
                                        {
                                            StateScript.RemoveVariable(result[0],
                                                ((GameObject)evt.previousValue).GetComponent<IDGenerator>()
                                                .GetUniqueID());
                                        }
                                        StateScript.SetVariableValue(result[0],
                                            ((GameObject)objectListField.value).GetComponent<IDGenerator>()
                                            .GetUniqueID());
                                    }
                                    catch (Exception)
                                    {
                                        StateScript.SetVariableValue(result[0], "");
                                    }
                                });

                                var deleteChoiceButton = FsmElementUtility.CreateButton("X", () =>
                                {
                                    if (objectListField.value != null)
                                    {
                                        StateScript.RemoveVariable(result[0],
                                            ((GameObject)objectListField.value).GetComponent<IDGenerator>()
                                            .GetUniqueID());
                                    }
                                    else
                                    {
                                        StateScript.RemoveVariable(result[0], "");
                                    }

                                    stateAttributeContainer.Remove(objectListField);
                                });

                                deleteChoiceButton.AddToClassList("fsm-node_button");
                                objectListField.AddToClassList("fsm-node_state-attribute-field");

                                objectListField.Add(deleteChoiceButton);
                                stateAttributeContainer.Add(objectListField);
                            }
                        }

                        Button addChoiceButton = null;
                        addChoiceButton = FsmElementUtility.CreateButton("Add Patrol Point", () =>
                        {
                            var indexToAdd = stateAttributeContainer.IndexOf(addChoiceButton);
                            if (indexToAdd != -1)
                            {
                                var objectListField = new ObjectField()
                                {
                                    label = UpdateNameStyle(result[0]),
                                    objectType = typeof(GameObject)
                                };
                                StateScript.SetVariableValue(result[0], "");
                                objectListField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                                {
                                    try
                                    {
                                        if (evt.previousValue == null)
                                        {
                                            StateScript.RemoveVariable(result[0], "");
                                        }
                                        else
                                        {
                                            StateScript.RemoveVariable(result[0],
                                                ((GameObject)evt.previousValue).GetComponent<IDGenerator>()
                                                .GetUniqueID());
                                        }
                                        StateScript.SetVariableValue(result[0],
                                            ((GameObject)objectListField.value).GetComponent<IDGenerator>()
                                            .GetUniqueID());
                                    }
                                    catch (Exception)
                                    {
                                        StateScript.SetVariableValue(result[0], "");
                                    }
                                });

                                var deleteChoiceButton = FsmElementUtility.CreateButton("X", () =>
                                {
                                    if (objectListField.value != null)
                                    {
                                        StateScript.RemoveVariable(result[0],
                                            ((GameObject)objectListField.value).GetComponent<IDGenerator>()
                                            .GetUniqueID());
                                    }
                                    else
                                    {
                                        StateScript.RemoveVariable(result[0], "");
                                    }

                                    stateAttributeContainer.Remove(objectListField);
                                });

                                deleteChoiceButton.AddToClassList("fsm-node_button");
                                objectListField.AddToClassList("fsm-node_state-attribute-field");

                                objectListField.Add(deleteChoiceButton);
                                stateAttributeContainer.Insert(indexToAdd, objectListField);
                            }
                        });

                        addChoiceButton.AddToClassList("fsm-node_button");
                        stateAttributeContainer.Add(addChoiceButton);
                        break;
                    case "System.Single":
                        var floatField = new FloatField()
                        {
                            label = UpdateNameStyle(result[0]),
                            value = float.Parse(result[2])
                        };
                        floatField.RegisterCallback<InputEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], evt.newData);
                        });
                        floatField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(floatField);
                        break;
                    case "System.Int32":
                        var integerField = new IntegerField()
                        {
                            label = UpdateNameStyle(result[0]),
                            value = int.Parse(result[2])
                        };
                        integerField.RegisterCallback<InputEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], evt.newData);
                        });
                        integerField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(integerField);
                        break;
                    case "System.Boolean":
                        var toggle = new Toggle()
                        {
                            label = UpdateNameStyle(result[0]),
                            value = bool.Parse(result[2])
                        };
                        toggle.RegisterCallback<ClickEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], evt.bubbles);
                        });
                        toggle.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(toggle);
                        break;
                    case "System.String":
                        var textField = new TextField()
                        {
                            label = UpdateNameStyle(result[0]),
                            value = result[2]
                        };
                        textField.RegisterCallback<InputEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], evt.newData);
                        });
                        textField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(textField);
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
                    if (enemyState.GetStateName() == StateName)
                    {
                        StateScript = enemyState;
                    }
                }
            }
        }

        private string UpdateNameStyle(string newName)
        {
            var fullName = Regex.Split(newName, @"(?=[A-Z])");
            var resultString = "";

            for (int i = 0; i < fullName.Length; i++)
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
}
#endif