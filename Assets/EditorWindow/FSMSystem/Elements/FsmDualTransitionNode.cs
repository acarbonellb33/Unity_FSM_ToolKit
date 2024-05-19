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
    using FSM.Utilities;
    /// <summary>
    /// Represents a dual transition node in an FSM graph, inheriting from <see cref="FsmNode"/>. In this case, there are two possible transitions, one for true and one for false.
    /// </summary>
    public class FsmDualTransitionNode : FsmNode
    {
        /// <summary>
        /// Initializes the FSM dual transition node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="graphView">The graph view this node belongs to.</param>
        /// <param name="vectorPos">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Transition;

            DataObjects = new List<StateScriptData>()
                { new NextStateData(), new SeeingData(), new HearingData(), new DistanceData(), new HealthData() };

            var connectionSaveData1 = new FsmConnectionSaveData()
            {
                Text = "True",
            };
            var connectionSaveData2 = new FsmConnectionSaveData()
            {
                Text = "False",
            };
            Connections.Add(connectionSaveData1);
            Connections.Add(connectionSaveData2);
            mainContainer.AddToClassList("fsm-node_main-container");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }
        /// <summary>
        /// Draws the node, adding ports and custom state attributes.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            foreach (var connection in Connections)
            {
                OutputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);
                if (!OutputPort.connected)
                {
                    // Apply orange color to the port
                    OutputPort.portColor = Color.red;
                }

                OutputPort.userData = connection;

                outputContainer.Add(OutputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }

            var customDataContainer = new VisualElement();
            //customDataContainer.AddToClassList("fsm-node_custom-data-container");

            GetScriptableObject();

            CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);

            extensionContainer.Add(customDataContainer);

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
                            value = FsmIOUtility.FindGameObjectWithId<IDGenerator>(output)
                        };
                        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], ((GameObject)evt.previousValue).GetComponent<IDGenerator>()
                                .GetUniqueID());
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
                            value = FsmIOUtility.FindGameObjectWithId<IDGenerator>(output)
                        };
                        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                        {
                            StateScript.SetVariableValue(result[0], ((GameObject)evt.previousValue).GetComponent<IDGenerator>()
                                .GetUniqueID());
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
        
        #endregion
    }
}
#endif