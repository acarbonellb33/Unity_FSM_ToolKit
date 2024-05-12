#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System;
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
    public class FsmCustomStateNode : FsmNode
    {
        //Custom State Attributes
        private GameObject _selectedGameObject;
        private Component _selectedComponent;
        private string _selectedFunction;
        private DropdownField _componentDropdown = new();
        private readonly DropdownField _functionDropdown = new();

        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.CustomState;

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

            GetScriptableObject();

            CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);

            extensionContainer.Add(showPopupToggle);
            if (showPopupToggle.value) AddDropdownFields();
            extensionContainer.Add(customDataContainer);

            mainContainer.style.backgroundColor = new Color(200f / 255f, 250f / 255f, 100f / 255f);

            RefreshExpandedState();
        }

        #region Custom State Methods

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


        // Method to populate the dropdown menu with function options
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

        private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
        {

            var stateAttributeContainer = new VisualElement();
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
                            StateScript.SetVariableValue(result[0],
                                ((GameObject)objectField.value).GetComponent<IDGenerator>().GetUniqueID());
                            _selectedGameObject = (GameObject)objectField.value;
                            PopulateComponentDropdown();
                        }
                        objectField.AddToClassList("fsm-node_state-attribute-field");
                        stateAttributeContainer.Add(objectField);
                        break;

                    case "selectedComponent":
                        _componentDropdown.label = UpdateNameStyle(result[0]);
                        _componentDropdown.value = result[2];
                        if (!String.IsNullOrEmpty(_componentDropdown.value))
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

        private void GetScriptableObject()
        {
            try
            {
                StateScript.GetStateName();
            }
            catch (NullReferenceException)
            {
                StateScript = new CustomData();
            }
        }

        private string UpdateNameStyle(string newName)
        {
            var fullName = Regex.Split(newName, @"(?=[A-Z])");
            var resultString = "";

            for (var i = 0; i < fullName.Length; i++)
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