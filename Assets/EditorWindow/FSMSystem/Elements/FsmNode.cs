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
    public class FsmNode : Node
    {
        public string Id { get; set; }
        public string StateName { get; set; }
        public List<FsmConnectionSaveData> Choices { get; set; }
        public FsmNodeType NodeType { get; set; }
        private FsmGraphView _graphView;
        protected List<StateScriptData> DataObjects;
        public StateScriptData StateScript { get; set; }
        private Label _stateNameField;
        protected Port InputPort, OutputPort;
        
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

        public virtual void Initialize(string nodeName, FsmGraphView graphView, Vector2 vectorPos)
        {
            Id = Guid.NewGuid().ToString();
            StateName = nodeName;
            Choices = new List<FsmConnectionSaveData>();
            _graphView = graphView;
            SetPosition(new Rect(vectorPos, Vector2.zero));

            StateScript = null;

            AddManipulators();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", _ => DisconnectPorts(inputContainer));
            evt.menu.AppendAction("Disconnect Output Ports", _ => DisconnectPorts(outputContainer));
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);
        }

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

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

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
        public virtual void SetStateName(string stateName)
        {
            StateName = stateName;
            _stateNameField.text = stateName;
        }
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
        public void SetPortColor(Color color, Direction direction)
        {
            if (direction == Direction.Input)
            {
                InputPort.portColor = color;
            }
            else
            {
                OutputPort.portColor = color;
            }
        }
        #endregion

        #region Animator Methods
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
            else if (_parameterType == "Trigger")
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
                    var toggleField = new Toggle()
                    {
                        label = selectedParameter
                    };
                    _animatorParameter = selectedParameter;
                    _parameterType = parameterType;
                    _animatorValue = toggleField.value.ToString();
                    toggleField.AddToClassList("fsm-node_state-attribute-field");
                    toggleField.RegisterValueChangedCallback(e => { _animatorValue = e.newValue.ToString();});
                    container.Insert(1, toggleField);
                }
                HasAnimatorTrigger = true;
            });

            container.Insert(0, dropdown);
            extensionContainer.Insert(3, container);

            RefreshExpandedState();
        }
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
        public FsmAnimatorSaveData GetAnimatorSaveData()
        {
            var animatorSaveData = new FsmAnimatorSaveData();
            animatorSaveData.Initialize(HasAnimatorTrigger, _animatorParameter, _parameterType, _animatorValue);
            return animatorSaveData;
        }
        public void SetAnimatorSaveData(FsmAnimatorSaveData animatorSaveData)
        {
            HasAnimatorTrigger = animatorSaveData.TriggerEnable;
            _animatorParameter = animatorSaveData.ParameterName;
            _parameterType = animatorSaveData.ParameterType;
            _animatorValue = animatorSaveData.Value;
        }
        #endregion
        
        #region Hit State Override Methods
        private bool _secondIteration;
        protected void ShowHitStateOverrideToggle(Toggle toggle)
        {
            if (!_secondIteration)
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
            }
            
            if (!toggle.value)
            {
                if (HasHitStateOverride)
                {
                    HasHitStateOverride = false;
                }

                extensionContainer.RemoveAt(extensionContainer.childCount - 1);
                return;
            }
            
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

            RefreshExpandedState();
        }
        public FsmHitNodeSaveData GetHitNodeSaveData()
        {
            var hitNodeSaveData = new FsmHitNodeSaveData();
            hitNodeSaveData.Initialize(HasHitStateOverride, _canGetHit, _timeToWait, _canDie);
            return hitNodeSaveData;
        }
        public void SetHitNodeSaveData(FsmHitNodeSaveData animatorSaveData)
        {
            HasHitStateOverride = animatorSaveData.HasHitOverride;
            _canGetHit = animatorSaveData.CanGetHit;
            _timeToWait = animatorSaveData.TimeToWait;
            _canDie = animatorSaveData.CanDie;
        }
        #endregion
    }
}
#endif