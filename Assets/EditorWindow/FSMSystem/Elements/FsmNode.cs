#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using System;
    using System.Collections.Generic;
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utilities;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.States;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using UnityEditor;
    /// <summary>
    /// Represents the base class for all FSM node elements.
    /// </summary>
    public class FsmNode : Node
    {
        /// <summary>
        /// Gets or sets the unique identifier of the FSM node.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the state associated with the FSM node.
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// Gets or sets the list of connections associated with the FSM node.
        /// </summary>
        public List<FsmConnectionSaveData> Connections { get; set; }
        /// <summary>
        /// Gets or sets the type of the FSM node.
        /// </summary>
        public FsmNodeType NodeType { get; set; }
        /// <summary>
        /// Gets or sets the script data associated with the behaviour of the FSM node.
        /// </summary>
        public StateScriptData StateScript { get; set; }
        
        protected List<StateScriptData> DataObjects;
        protected Port InputPort;
        protected List<Port> OutputPort;
        
        private FsmGraphView _graphView;
        private Label _stateNameField;
        
        //Animator Trigger
        protected bool HasAnimatorTrigger;
        private string _animatorParameter;
        private string _parameterType;
        private string _animatorValue;
        
        //Hit State Override
        protected bool HasHitStateOverride;
        private bool _canGetHit;
        private float _timeToWait;
        private bool _canDie;
        /// <summary>
        /// Initializes the FSM node with the provided name, FSM graph view, and position.
        /// </summary>
        /// <param name="nodeName">The name of the FSM node.</param>
        /// <param name="graphView">The FSM graph view where the node will be placed.</param>
        /// <param name="vectorPos">The position of the node within the FSM graph view.</param>
        public virtual void Initialize(string nodeName, FsmGraphView graphView, Vector2 vectorPos)
        {
            Id = Guid.NewGuid().ToString();
            StateName = nodeName;
            Connections = new List<FsmConnectionSaveData>();
            _graphView = graphView;
            SetPosition(new Rect(vectorPos, Vector2.zero));

            OutputPort = new List<Port>();
            StateScript = null;

            AddManipulators();
        }
        /// <summary>
        /// Virtual method to draw the FSM node. This method will be overridden by the derived classes. Acts as a base node and the derived classes will implement their own logic.
        /// </summary>
        public virtual void Draw()
        {
            _stateNameField = FsmElementUtility.CreateLabel(StateName, callback =>
            {
                StateName = callback.newValue;
            });
            _stateNameField.AddClasses("fsm-node_label");
            titleContainer.Insert(0, _stateNameField);

            InputPort = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            InputPort.portColor = Color.red;
            inputContainer.Add(InputPort);
            inputContainer.AddToClassList("fsm-node_input-output-container");
        }

        /// <summary>
        /// Overrides the method to build a contextual menu for the FSM node. In this case, the menu will contain options to disconnect input and output ports.
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", _ => DisconnectPorts(inputContainer));
            evt.menu.AppendAction("Disconnect Output Ports", _ => DisconnectPorts(outputContainer));
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }
        
        #region Ports

        public void DisconnectAllPorts()
        {
            DisconnectPorts(inputContainer);
            DisconnectPorts(outputContainer);
        }
        private void DisconnectPorts(VisualElement container)
        {
            foreach (VisualElement childElement in container.Children())
            {
                if (childElement is Port port && port.connected)
                {
                    _graphView.DeleteElements(port.connections);
                }
            }
        }

        #endregion

        #region Styles
        /// <summary>
        /// Sets the error style of the main container.
        /// </summary>
        /// <param name="color">The color to set as the background color.</param>
        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }
        /// <summary>
        /// Resets the style of the main container to its default color.
        /// </summary>
        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
        }

        #endregion

        #region Utilities
        private void AddManipulators()
        {
            this.AddManipulator(CreateNodeContectualMenu());
        }
        private IManipulator CreateNodeContectualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                    menuEvent.menu.AppendAction("Create Transition",
                        _ => Debug.Log("Create Transition")));

            return contextualMenuManipulator;
        }
        /// <summary>
        /// Sets the name of the state and updates the displayed state name field.
        /// </summary>
        /// <param name="stateName">The new name of the state.</param>
        public virtual void SetStateName(string stateName)
        {
            StateName = stateName;
            _stateNameField.text = stateName;
        }
        /// <summary>
        /// Retrieves the StateScriptData object associated with the current state.
        /// </summary>
        protected void GetScriptableObject()
        {
            try
            {
                StateScript.GetStateName();
            }
            catch (NullReferenceException)
            {
                foreach (StateScriptData enemyState in DataObjects)
                {
                    if (enemyState.GetStateName() == StateName.Split(" ")[0])
                    {
                        StateScript = enemyState;
                    }
                }
            }
        }
        /// <summary>
        /// Updates the naming style of a string from camel case to a readable format with spaces between words.
        /// </summary>
        /// <param name="newName">The string to be formatted.</param>
        /// <returns>The formatted string.</returns>
        protected string UpdateNameStyle(string newName)
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
        /// <summary>
        /// Sets the color of a port based on its direction (input or output).
        /// </summary>
        /// <param name="color">The color to set.</param>
        /// <param name="direction">The direction of the port (input or output).</param>
        /// <param name="portName">The name of the port.</param>
        public void SetPortColor(Color color, Direction direction, string portName = "")
        {
            if (direction == Direction.Input)
            {
                InputPort.portColor = color;
            }
            else
            {
                foreach (var nodeOutputPort in OutputPort)
                {
                    if(nodeOutputPort.portName == portName)nodeOutputPort.portColor = color;
                }
            }
        }
        #endregion

        #region Animator Methods
        /// <summary>
        /// Shows a dropdown menu for selecting an animator parameter and provides fields for configuring its value based on the parameter type.
        /// </summary>
        /// <param name="toggle">The toggle representing the state of showing the dropdown.</param>
        protected void ShowAnimatorParameterDropdown(Toggle toggle)
        {
            if (!toggle.value)
            {
                if (HasAnimatorTrigger)
                {
                    HasAnimatorTrigger = false;
                }
                extensionContainer.RemoveAt(3);
                return;
            }
            
            var container = new VisualElement();
            
            var animatorParameters = GetAllAnimatorParameters();
            animatorParameters.Insert(0, "");
            var defaultIndex = _animatorParameter == null ? "" : _animatorParameter;
            if(!animatorParameters.Contains(_animatorParameter)) defaultIndex = "";
            var dropdown = new DropdownField("Select Parameter", animatorParameters, defaultIndex);
            dropdown.AddToClassList("fsm-node_animator-dropdown");
            container.Add(dropdown);
            
            if (_parameterType == "Float")
            {
                var floatField = new FloatField()
                {
                    label = dropdown.value,
                    value = float.Parse(_animatorValue)
                };
                floatField.AddToClassList("fsm-node_state-attribute-field");
                floatField.RegisterCallback<ChangeEvent<float>>(e => { _animatorValue = e.newValue.ToString(CultureInfo.InvariantCulture); });
                container.Add(floatField);
            }
            else if (_parameterType == "Integer")
            {
                var integerField = new IntegerField()
                {
                    label = dropdown.value,
                    value = int.Parse(_animatorValue)
                };
                integerField.AddToClassList("fsm-node_state-attribute-field");
                integerField.RegisterCallback<ChangeEvent<int>>(e => { _animatorValue = e.newValue.ToString(); });
                container.Add(integerField);
            }
            else if (_parameterType == "Bool")
            {
                var toggleField = new Toggle()
                {
                    label = dropdown.value,
                    value = bool.Parse(_animatorValue)
                };
                toggleField.AddToClassList("fsm-node_state-attribute-field");
                toggleField.RegisterValueChangedCallback(e => { _animatorValue = e.newValue.ToString(); });
                container.Add(toggleField);
            }

            dropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedParameter = evt.newValue;
                var parameterType = GetParameterType(selectedParameter);

                if(container.childCount > 1) container.RemoveAt(1);

                if (parameterType == "Float")
                {
                    var floatField = new FloatField()
                    {
                        label = selectedParameter
                    };
                    _animatorParameter = selectedParameter;
                    _parameterType = parameterType;
                    _animatorValue = floatField.value.ToString(CultureInfo.InvariantCulture);
                    floatField.AddToClassList("fsm-node_state-attribute-field");
                    floatField.RegisterCallback<ChangeEvent<float>>(e => { _animatorValue = e.newValue.ToString(CultureInfo.InvariantCulture);});
                    container.Insert(1, floatField);
                }
                else if (parameterType == "Integer")
                {
                    var integerField = new IntegerField()
                    {
                        label = selectedParameter
                    };
                    _animatorParameter = selectedParameter;
                    _parameterType = parameterType;
                    _animatorValue = integerField.value.ToString();
                    integerField.AddToClassList("fsm-node_state-attribute-field");
                    integerField.RegisterCallback<ChangeEvent<int>>(e => { _animatorValue = e.newValue.ToString(); });
                    container.Insert(1, integerField);
                }
                else if (parameterType == "Bool")
                {
                    var toggleField = new Toggle()
                    {
                        label = selectedParameter
                    };
                    _animatorParameter = selectedParameter;
                    _parameterType = parameterType;
                    _animatorValue = toggleField.value.ToString();
                    toggleField.AddToClassList("fsm-node_state-attribute-field");
                    toggleField.RegisterValueChangedCallback(e => { _animatorValue = e.newValue.ToString(); });
                    container.Insert(1, toggleField);
                }
                else if (parameterType == "Trigger")
                {
                    _animatorParameter = selectedParameter;
                    _parameterType = parameterType;
                    _animatorValue = "";
                }
                HasAnimatorTrigger = true;
            });

            container.Insert(0, dropdown);
            extensionContainer.Insert(3, container);

            RefreshExpandedState();
        }
        /// <summary>
        /// Gets the type of the specified animator parameter.
        /// </summary>
        /// <param name="parameterName">The name of the animator parameter.</param>
        /// <returns>The type of the animator parameter.</returns>
        private string GetParameterType(string parameterName)
        {
            var animator = GetAnimatorComponent();
            
            if (animator != null)
            {
                var parameters = animator.parameters;
                foreach (var parameter in parameters)
                {
                    if (parameter.name == parameterName)
                    {
                        return parameter.type.ToString();
                    }
                }
            }

            return "Unknown";
        }
        private List<string> GetAllAnimatorParameters()
        {
            Animator animator = GetAnimatorComponent();

            List<string> parameters = new List<string>();
            if (animator != null)
            {
                for (int i = 0; i < animator.parameterCount; i++)
                {
                    AnimatorControllerParameter parameter = animator.parameters[i];
                    parameters.Add(parameter.name);
                }
            }

            return parameters;
        }
        private Animator GetAnimatorComponent()
        {
            var gameObject = Selection.activeGameObject;
            return gameObject.GetComponent<Animator>();
        }
        /// <summary>
        /// Retrieves the saved animator data.
        /// </summary>
        /// <returns>The saved animator data.</returns>
        public FsmAnimatorSaveData GetAnimatorSaveData()
        {
            var animatorSaveData = new FsmAnimatorSaveData();
            animatorSaveData.Initialize(HasAnimatorTrigger, _animatorParameter, _parameterType, _animatorValue);
            return animatorSaveData;
        }
        /// <summary>
        /// Sets the animator save data.
        /// </summary>
        /// <param name="animatorSaveData">The animator save data to set.</param>
        public void SetAnimatorSaveData(FsmAnimatorSaveData animatorSaveData)
        {
            HasAnimatorTrigger = animatorSaveData.AnimationTrigger;
            _animatorParameter = animatorSaveData.ParameterName;
            _parameterType = animatorSaveData.ParameterType;
            _animatorValue = animatorSaveData.Value;
        }
        #endregion
        
        #region Hit State Override Methods
        private bool _secondIteration;
        /// <summary>
        /// If enabled adds the parameters to be able to override the hit values.
        /// </summary>
        /// <param name="toggle">The toggle to show override hit parameters.</param>
        protected void ShowHitStateOverrideToggle(Toggle toggle)
        {
            /*if (!_secondIteration)
            {
                if(!EditorPrefs.GetBool("EnableHitState"))
                {
                    EditorUtility.DisplayDialog(
                        $"Can Get Hit is not Enabled!",
                        "To be able to override the hit state, you have to enable it.",
                        "Continue"
                    );
                    toggle.value = false;
                    _secondIteration = true;
                    return;
                }
            }
            if(_secondIteration && !toggle.value)
            {
                _secondIteration = false;
                return;
            }*/
            
            if (!toggle.value)
            {
                if (HasHitStateOverride)
                {
                    HasHitStateOverride = false;
                }
                extensionContainer.RemoveAt(extensionContainer.childCount - 1);
                if(!EditorPrefs.GetBool("EnableHitState"))toggle.SetEnabled(false);
                return;
            }
            toggle.SetEnabled(true);
            var visualElement = new VisualElement();
            
            var canGetHitToggle = new Toggle
            {
                label = "Can Get Hit",
                value = _canGetHit
            };
            canGetHitToggle.AddToClassList("fsm-node_state-attribute-field");
            canGetHitToggle.RegisterValueChangedCallback(e =>
            {
                _canGetHit = e.newValue;
            });
            
            var timeToWaitField = new FloatField
            {
                label = "Time to Wait:",
                value = _timeToWait
            };
            timeToWaitField.AddToClassList("fsm-node_state-attribute-field");
            timeToWaitField.RegisterCallback<ChangeEvent<float>>(e =>
            {
                _timeToWait = e.newValue;
            });
            
            var canDieToggle = new Toggle
            {
                label ="Can Die",
                value = _canDie
            };
            canDieToggle.AddToClassList("fsm-node_state-attribute-field");
            canDieToggle.RegisterValueChangedCallback(e =>
            {
                _canDie = e.newValue;
            });
            
            visualElement.Add(canGetHitToggle);
            visualElement.Add(timeToWaitField);
            visualElement.Add(canDieToggle);

            extensionContainer.Add(visualElement);
            
            HasHitStateOverride = true;
            //toggle.SetEnabled(true);
            RefreshExpandedState();
        }
        /// <summary>
        /// Gets the save data from the hit information.
        /// </summary>
        /// <returns>The hit node save data.</returns>
        public FsmHitNodeSaveData GetHitNodeSaveData()
        {
            var hitNodeSaveData = new FsmHitNodeSaveData();
            hitNodeSaveData.Initialize(HasHitStateOverride, _canGetHit, _timeToWait, _canDie);
            return hitNodeSaveData;
        }
        /// <summary>
        /// Sets the hit node save data.
        /// </summary>
        /// <param name="animatorSaveData">The hit node save data to set.</param>
        public void SetHitNodeSaveData(FsmHitNodeSaveData animatorSaveData)
        {
            HasHitStateOverride = animatorSaveData.HasHitOverride;
            _canGetHit = animatorSaveData.CanGetHit;
            _timeToWait = animatorSaveData.TimeToWait;
            _canDie = animatorSaveData.CanDie;
        }
        public virtual void UpdateHitEnable(bool canGetHit){}
        #endregion
    }
}
#endif