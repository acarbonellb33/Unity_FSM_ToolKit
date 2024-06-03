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
    using UnityEditor;
    /// <summary>
    /// Represents a state node in an FSM graph, inheriting from <see cref="FsmNode"/>.
    /// </summary>
    public class FsmStateNode : FsmNode
    {
        private Toggle _hitPopupToggle;
        private readonly FsmHoverPopupWindow _hoverPopup = new();
        /// <summary>
        /// Initializes the FSM state node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="graphView">The graph view this node belongs to.</param>
        /// <param name="posVector2">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView graphView, Vector2 posVector2)
        {
            base.Initialize(nodeName, graphView, posVector2);
            NodeType = FsmNodeType.State;
            // Inside your Initialize method
            
            DataObjects = new List<StateScriptData>()
                {new IdleData(), new PatrolData(), new ChaseData(), new FleeData(), new AttackData(), new SearchData() };

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Condition",
            };
            Connections.Add(connectionSaveData);

            mainContainer.AddToClassList("fsm-node_main-container");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }
        /// <summary>
        /// Draws the node, adding ports, toggles, and any state attributes.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            foreach (var connection in Connections)
            {
                var outputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output,
                    Port.Capacity.Multi);
                if (!outputPort.connected)
                {
                    outputPort.portColor = Color.red;
                }

                outputPort.userData = connection;

                OutputPort.Add(outputPort);
                outputContainer.Add(outputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }

            var animatorPopupToggle = new Toggle()
            {
                label = "Animator Trigger",
                value = HasAnimatorTrigger,
            };
            animatorPopupToggle.RegisterValueChangedCallback(_ => { ShowAnimatorParameterDropdown(animatorPopupToggle); });
            animatorPopupToggle.AddToClassList("fsm-node_toggle-bold");

            _hitPopupToggle = new Toggle()
            {
                label = "Override Hit State",
                value = HasHitStateOverride,
            };
            _hitPopupToggle.RegisterValueChangedCallback(_ => { ShowHitStateOverrideToggle(_hitPopupToggle); });
            _hitPopupToggle.AddToClassList("fsm-node_toggle-bold");
            _hitPopupToggle.RegisterCallback<MouseEnterEvent>(_ => ShowPopupWindow(_hitPopupToggle));
            _hitPopupToggle.RegisterCallback<MouseLeaveEvent>(_ => ClosePopupWindow());

            var horizontalLine = new VisualElement();
            horizontalLine.AddToClassList("horizontal-line");
            
            var horizontalLine2 = new VisualElement();
            horizontalLine2.AddToClassList("horizontal-line");

            var customDataContainer = new VisualElement();

            GetScriptableObject();

            CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);

            extensionContainer.Add(customDataContainer);
            extensionContainer.Add(horizontalLine);
            extensionContainer.Add(animatorPopupToggle);
            extensionContainer.Add(horizontalLine2);
            if (animatorPopupToggle.value) ShowAnimatorParameterDropdown(animatorPopupToggle);
            extensionContainer.Add(_hitPopupToggle);
            if (EditorPrefs.GetBool("EnableHitState"))
            {
                _hitPopupToggle.SetEnabled(true);
                if (_hitPopupToggle.value) ShowHitStateOverrideToggle(_hitPopupToggle);
            }
            else _hitPopupToggle.SetEnabled(false);
            
            
            mainContainer.style.backgroundColor = new Color(200f / 255f, 250f / 255f, 100f / 255f);

            RefreshExpandedState();
        }
        private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
        {
            var stateAttributeContainer = new VisualElement();
            
            var stateAttributeLabel = FsmElementUtility.CreateLabel("State Attributes Fields");
            stateAttributeLabel.AddToClassList("fsm-node_state-attribute-label");
            stateAttributeContainer.Add(stateAttributeLabel);
            
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
        public override void UpdateHitEnable(bool value)
        {
            if (!value)
            {
                if (!_hitPopupToggle.value)
                {
                    _hitPopupToggle.SetEnabled(false);
                }
                _hitPopupToggle.value = false;
            }
            else
            {
                _hitPopupToggle.SetEnabled(true);
            }
        }
        private void ShowPopupWindow(VisualElement targetElement)
        {
            if (!_hitPopupToggle.value && !_hitPopupToggle.enabledSelf)
            {
                var elementRect = targetElement.worldBound;
                UnityEditor.PopupWindow.Show(new Rect(elementRect.x + 10f, elementRect.y - 77.5f, 250, 100), _hoverPopup);
            }
        }
        private void ClosePopupWindow()
        {
            if (!_hitPopupToggle.value && !_hitPopupToggle.enabledSelf)
            {
                if (_hoverPopup != null)
                {
                    _hoverPopup.editorWindow.Close();
                }
            }
        }
    }
}
#endif