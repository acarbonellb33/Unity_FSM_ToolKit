#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utilities;
    using Toggle = UnityEngine.UIElements.Toggle;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.States;
    using FSM.Nodes.States.StatesData;
    public class FsmTransitionNode : FsmNode
    {
        private List<StateScriptData> _dataObjects;

        public override void Initialize(string nodeName, FsmGraphView graphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, graphView, vectorPos);
            NodeType = FsmNodeType.Transition;

            _dataObjects = new List<StateScriptData>()
                { new NextStateData(), new SeeingData(), new HearingData(), new DistanceData(), new HealthData() };

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Next Action",
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
                OutputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);
                if (!OutputPort.connected)
                {
                    OutputPort.portColor = Color.red;
                }

                OutputPort.userData = connection;

                outputContainer.Add(OutputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }

            var customDataContainer = new VisualElement();

            GetScriptableObject();

            CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);

            extensionContainer.Add(customDataContainer);

            mainContainer.style.backgroundColor = new Color(151f / 255f, 90f / 255f, 245f / 255f);

            RefreshExpandedState();
        }

        #region ScriptableObject Attributes

        private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
        {
            var stateName = StateName.Split(' ')[0];
            if (stateName == "Distance")
            {
                CreateDistanceAttributes(attributes, customDataContainer);
                return;
            }

            var stateAttributeContainer = new VisualElement();
            stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");

            foreach (var attribute in attributes)
            {
                var result = attribute.Split(',');

                switch (result[1])
                {
                    case "FSM.Enumerations.FsmOperands":
                        var enumField = new EnumField();
                        enumField.Init(new FsmOperands());
                        enumField.value = (Enum)Enum.Parse(typeof(FsmOperands), result[2]);
                        enumField.label = UpdateNameStyle(result[0]);
                        enumField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], (FsmOperands)evt.newValue);
                        });
                        enumField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(enumField);
                        break;
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
                    case "System.Single":
                        var floatField = new FloatField()
                        {
                            label = UpdateNameStyle(result[0]),
                            value = float.Parse(result[2])
                        };
                        floatField.RegisterCallback<InputEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], float.Parse(evt.newData));
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
                            StateScript.SetVariableValue(result[0], int.Parse(evt.newData));
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

        private void CreateDistanceAttributes(List<string> attributes, VisualElement customDataContainer)
        {
            var stateAttributeContainer = new VisualElement();
            stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container-hearing");

            var label = FsmElementUtility.CreateLabel("Distance From Player");
            label.AddToClassList("fsm-node_state-attribute-container-hearing-label");
            stateAttributeContainer.Add(label);

            foreach (var attribute in attributes)
            {
                var result = attribute.Split(',');
                switch (result[1])
                {
                    case "FSM.Enumerations.FsmOperands":
                        var enumField = new EnumField();
                        enumField.Init(new FsmOperands());
                        enumField.value = (Enum)Enum.Parse(typeof(FsmOperands), result[2]);
                        enumField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], (FsmOperands)evt.newValue);
                        });
                        enumField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(enumField);
                        break;
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
                    case "System.Single":
                        var floatField = new FloatField()
                        {
                            value = float.Parse(result[2])
                        };
                        floatField.RegisterCallback<InputEvent>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], float.Parse(evt.newData));
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
                            StateScript.SetVariableValue(result[0], int.Parse(evt.newData));
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
                foreach (var enemyState in _dataObjects)
                {
                    var stateName = StateName.Split(' ')[0];
                    if (enemyState.GetStateName() == stateName)
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