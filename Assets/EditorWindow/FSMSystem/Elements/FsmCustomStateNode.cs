#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utilities;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.States.StatesData;
    using FSM.Utilities;
    using FSM.Nodes.States;
    using UnityEditor;
    /// <summary>
    /// Represents a custom state node in an FSM graph, inheriting from <see cref="FsmNode"/>. With this node you are able to add and execute a function from any GameObject.
    /// </summary>
    public class FsmCustomStateNode : FsmNode
    {
        private Toggle _hitPopupToggle;
        // Custom State Attributes
        private GameObject _selectedGameObject;
        private Component _selectedComponent;
        private string _selectedFunction;
        private DropdownField _componentDropdown = new();
        private readonly DropdownField _functionDropdown = new();

        /// <summary>
        /// Initializes the FSM custom state node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="fsmGraphView">The graph view this node belongs to.</param>
        /// <param name="vectorPos">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.CustomState;

            DataObjects = new List<StateScriptData> { new CustomData() };

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Condition",
            };
            Connections.Add(connectionSaveData);

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
            
            var horizontalLine = new VisualElement();
            horizontalLine.AddToClassList("horizontal-line");
            
            var horizontalLine2 = new VisualElement();
            horizontalLine2.AddToClassList("horizontal-line");

            var customDataContainer = new VisualElement();

            StateScript = new CustomData();

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

        #region Custom State Methods

        /// <summary>
        /// Populates the component dropdown based on the selected GameObject.
        /// </summary>
        private void PopulateComponentDropdown()
        {
            if (_selectedGameObject == null)
            {
                return;
            }
            var components = _selectedGameObject.GetComponents<Component>();
            var componentNames = new List<string>();

            foreach (var comp in components)
            {
                componentNames.Add(comp.GetType().Name);
            }

            _componentDropdown ??= new DropdownField("Select Component", new List<string>(), 0);
            _componentDropdown.choices = componentNames;
        }

        /// <summary>
        /// Populates the function dropdown based on the selected component.
        /// </summary>
        /// <param name="dropdown">The dropdown field to populate.</param>
        private void PopulateFunctionDropdown(DropdownField dropdown)
        {
            if (_selectedGameObject == null)
            {
                return;
            }

            dropdown ??= new DropdownField("Select Function", new List<string>(), 0);

            var methods = _selectedComponent.GetType().GetMethods(BindingFlags.Instance |
                                                                  BindingFlags.Public |
                                                                  BindingFlags.IgnoreReturn |
                                                                  BindingFlags.DeclaredOnly);
            List<string> methodNames = new();
            foreach (var method in methods)
            {
                methodNames.Add(method.Name);
            }

            dropdown.choices = methodNames;
            dropdown.RegisterValueChangedCallback(evt => { _selectedFunction = evt.newValue; });
        }

        #endregion

        #region ScriptableObject Attributes

        /// <summary>
        /// Creates state attributes in the custom data container.
        /// </summary>
        /// <param name="attributes">The list of state attributes.</param>
        /// <param name="customDataContainer">The custom data container element.</param>
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

                switch (result[0])
                {
                    case "selectedGameObject":
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
                            var fieldValue = evt.newValue as GameObject;
                            if (fieldValue != null)
                            {
                                StateScript.SetVariableValue(result[0], fieldValue.GetComponent<IDGenerator>().GetUniqueID());
                                _selectedGameObject = fieldValue;
                                PopulateComponentDropdown();
                            }
                        });
                        if (objectField.value != null)
                        {
                            StateScript.SetVariableValue(result[0], ((GameObject)objectField.value).GetComponent<IDGenerator>().GetUniqueID());
                            _selectedGameObject = (GameObject)objectField.value;
                            PopulateComponentDropdown();
                        }
                        objectField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(objectField);
                        break;

                    case "selectedComponent":
                        _componentDropdown.label = UpdateNameStyle(result[0]);
                        _componentDropdown.value = result[2];
                        if (!string.IsNullOrEmpty(_componentDropdown.value))
                        {
                            _selectedComponent = _selectedGameObject.GetComponent(_componentDropdown.value);
                            StateScript.SetVariableValue(result[0], _componentDropdown.value);
                            PopulateFunctionDropdown(_functionDropdown);
                        }

                        _componentDropdown.RegisterValueChangedCallback(evt =>
                        {
                            var selectedName = evt.newValue;
                            _selectedComponent = _selectedGameObject.GetComponent(selectedName);
                            StateScript.SetVariableValue(result[0], _componentDropdown.value);
                            PopulateFunctionDropdown(_functionDropdown);
                        });
                        _componentDropdown.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(_componentDropdown);
                        break;

                    case "selectedFunction":
                        _functionDropdown.label = UpdateNameStyle(result[0]);
                        _functionDropdown.value = result[2];

                        _functionDropdown.RegisterValueChangedCallback(evt =>
                        {
                            StateScript.SetVariableValue(result[0], _functionDropdown.value);
                            _selectedFunction = evt.newValue;
                        });
                        _functionDropdown.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(_functionDropdown);
                        break;
                }
            }

            customDataContainer.Add(stateAttributeContainer);
        }

        #endregion
    }
}
#endif
