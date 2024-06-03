#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System.Collections.Generic;
    using System.Reflection;
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utilities;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.States.StatesData;
    using FSM.Nodes.States;
    using System;
    /// <summary>
    /// Represents an override node in an FSM graph, inheriting from <see cref="FsmNode"/>. With this node you are able to override the variable of a state node.
    /// </summary>
    public class FsmVariableNode : FsmNode
    {
        // Custom State Attributes
        private StateScriptData _selectedStateScript;
        private DropdownField _attributesDropdown = new();
        private string _selectedVariable;
        private string _value;
        
        /// <summary>
        /// Initializes the FSM override node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="fsmGraphView">The graph view this node belongs to.</param>
        /// <param name="vectorPos">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Variable;

            DataObjects = new List<StateScriptData> { new VariableData() };

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Override",
            };
            Connections.Add(connectionSaveData);

            mainContainer.AddToClassList("fsm-node_main-container");
            extensionContainer.AddToClassList("fsm-node_extension-container");

            RefreshExpandedState();
        }

        /// <summary>
        /// Draws the node, adding ports and custom state attributes.
        /// </summary>
        public override void Draw()
        {
            base.Draw();

            foreach (var connection in Connections)
            {
                var outputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output);
                if (!outputPort.connected)
                {
                    outputPort.portColor = Color.red;
                }

                outputPort.userData = connection;

                OutputPort.Add(outputPort);
                outputContainer.Add(outputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container");
            }
            

            var customDataContainer = new VisualElement();

            GetScriptableObject();

            CreateStateAttribute(customDataContainer);

            extensionContainer.Add(customDataContainer);
            
            mainContainer.style.backgroundColor = new Color(200f / 255f, 250f / 255f, 100f / 255f);

            RefreshExpandedState();
        }
        
        public void ConnectToStateScript(StateScriptData stateScript)
        {
            _selectedStateScript = stateScript;
            Dictionary<string,object> dic = StateScript.GetVariables();
            foreach (var item in dic)
            {
                if(item.Key == "selectedVariable" && item.Value != null)
                {
                    _selectedVariable = item.Value.ToString();
                }
                if(item.Key == "value" && item.Value != null)
                {
                    _value = item.Value.ToString();
                }
            }
            PopulateAttributesDropdown();
        }

        private void PopulateAttributesDropdown()
        {
            if (_selectedStateScript == null) return;

            var fields = _selectedStateScript.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var fieldNames = new List<string>();
            foreach (var field in fields)
            {
                fieldNames.Add(field.Name);
            }
            
            _attributesDropdown.choices = fieldNames;
            if(_selectedVariable != null)
            {
                _attributesDropdown.value = _selectedVariable;
            }
        }
        

        #region ScriptableObject Attributes

        /// <summary>
        /// Creates state attributes in the custom data container.
        /// </summary>
        /// <param name="customDataContainer">The custom data container element.</param>
        private void CreateStateAttribute(VisualElement customDataContainer)
        {
            var stateAttributeContainer = new VisualElement();
            stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");
            _attributesDropdown = new DropdownField("Select Attribute", new List<string>(), 0);
            _attributesDropdown.RegisterValueChangedCallback(evt =>
            {
                if(stateAttributeContainer.childCount > 1)stateAttributeContainer.RemoveAt(1);
                _selectedVariable = evt.newValue;
                StateScript.SetVariableValue("selectedVariable", _selectedVariable);
                var field = _selectedStateScript.GetType().GetField(_selectedVariable);
                if (field == null) return;
                if (field.FieldType == typeof(float))
                {
                    var floatField = new FloatField(field.Name)
                    {
                        value = String.IsNullOrEmpty(_value) ? (float)field.GetValue(_selectedStateScript) : float.Parse(_value)
                    };
                    Debug.Log(field.Name);
                    floatField.RegisterValueChangedCallback(evt =>
                    {
                        _value = evt.newValue.ToString();
                        StateScript.SetVariableValue("value", _value);
                    });
                    floatField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(floatField);
                }
                else if (field.FieldType == typeof(bool))
                {
                    var toggle = new Toggle(field.Name)
                    {
                        value = _value == "" ? (bool)field.GetValue(_selectedStateScript) : bool.Parse(_value)
                    };
                    toggle.RegisterValueChangedCallback(evt =>
                    {
                        _value = evt.newValue.ToString();
                        StateScript.SetVariableValue("value", _value);
                    });
                    toggle.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(toggle);
                }
                else if (field.FieldType == typeof(string))
                {
                    var textField = new TextField(field.Name)
                    {
                        value = _value == "" ? (string)field.GetValue(_selectedStateScript) : _value
                    };
                    textField.RegisterValueChangedCallback(evt =>
                    {
                        _value = evt.newValue;
                        StateScript.SetVariableValue("value", _value);
                    });
                    textField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(textField);
                }
                else if (field.FieldType == typeof(int))
                {
                    var intField = new IntegerField(field.Name)
                    {
                        value = _value == "" ? (int)field.GetValue(_selectedStateScript) : int.Parse(_value)
                    };
                    intField.RegisterValueChangedCallback(evt =>
                    {
                        _value = evt.newValue.ToString();
                        StateScript.SetVariableValue("value", _value);
                    });
                    intField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(intField);
                }
            });
            _attributesDropdown.AddToClassList("fsm-node_state-attribute-field");
            stateAttributeContainer.Add(_attributesDropdown);

            customDataContainer.Add(stateAttributeContainer);
        }

        #endregion
    }
}
#endif