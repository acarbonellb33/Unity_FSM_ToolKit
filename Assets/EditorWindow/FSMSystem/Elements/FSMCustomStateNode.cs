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

public class FSMCustomStateNode : FSMNode
{
    private List<StateScriptData> _dataObjects;
    
    //Animator Trigger
    private bool _hasAnimatorTrigger = false;
    private string _animatorParameter;
    private string _parameterType;
    private string _animatorValue;
    
    //Custom State Attributes
    private GameObject selectedGameObject = default;
    private Component selectedComponent = default;
    private string selectedFunction;
    private DropdownField componentDropdown = new DropdownField();
    private DropdownField functionDropdown = new DropdownField();
    public override void Initialize(string nodeName, FSMGraphView graphView,Vector2 postition)
    {
        base.Initialize(nodeName, graphView, postition);
        NodeType = FSMNodeType.CustomState;
        
        _dataObjects = new List<StateScriptData>(){new EstelitaData(), new PatrolData(), new ChaseData(), new AttackData(), new SearchData()};

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
            outputPort = this.CreatePort(connection.Text, Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
            if (!outputPort.connected)
            {
                // Apply orange color to the port
                outputPort.portColor = Color.red;
            }
            outputPort.userData = connection;

            outputContainer.Add(outputPort);
            outputContainer.AddToClassList("fsm-node_input-output-container");
        }
        
        Toggle showPopupToggle = new Toggle()
        {
            label = "Enable Animator Trigger",
            value = _hasAnimatorTrigger,
        };
        showPopupToggle.RegisterValueChangedCallback(evt =>
        {
            ShowAnimatorParameterDropdown(showPopupToggle);
            
        });
        showPopupToggle.AddToClassList("fsm-node_toggle");
        
        
        VisualElement customDataContainer = new VisualElement();
        //customDataContainer.AddToClassList("fsm-node_custom-data-container");
        
        GetScriptableObject();

        CreateStateAttribute(StateScript.InspectVariables(), customDataContainer);
        
        extensionContainer.Add(showPopupToggle);
        if(showPopupToggle.value)AddDropdownFields();
        extensionContainer.Add(customDataContainer);
        
        mainContainer.style.backgroundColor = new Color(200f/255f, 250f/255f, 100f/255f);

        RefreshExpandedState();
    }

    #region Custom State Methods
    

    // Method to populate the dropdown menu with component options
    private void PopulateComponentDropdown()
    {
        if (selectedGameObject != null)
        {
            // Get all the components attached to the selected GameObject
            Component[] components = selectedGameObject.GetComponents<Component>();
            List<string> componentNames = new List<string>();

            // Add their names to the dropdown options
            foreach (Component comp in components)
            {
                componentNames.Add(comp.GetType().Name);
            }

            if (componentDropdown == null)
            {
                componentDropdown = new DropdownField("Select Component", new List<string>(), 0);
            }
            // Populate the dropdown options
            componentDropdown.choices = componentNames;
        }
    }

    // Method to populate the dropdown menu with function options
    private void PopulateFunctionDropdown(DropdownField dropdown)
    {
        if (selectedGameObject != null)
        {
            if (dropdown == null)
            {
                dropdown = new DropdownField("Select Function", new List<string>(), 0);
            }

            // Get all the methods (functions) of the selected component
            MethodInfo[] methods = selectedComponent.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn | BindingFlags.DeclaredOnly);
            List<string> methodNames = new List<string>();

            // Add their names to the dropdown options
            foreach (MethodInfo method in methods)
            {
                methodNames.Add(method.Name);
            }

            // Populate the dropdown options
            dropdown.choices = methodNames;

            // Register a callback for when a function is selected
            dropdown.RegisterValueChangedCallback(evt => { selectedFunction = evt.newValue; });
        }
    }

    #endregion

    #region ScriptableObject Attributes
    private void CreateStateAttribute(List<string> attributes, VisualElement customDataContainer)
    {

        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");
    
        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');

            switch (result[0]){
                case "selectedGameObject":

                    string input = result[2];
                    string output = Regex.Replace(input, @"\s*\([^()]*\)", "");
                    
                    ObjectField objectField = new ObjectField()
                    {
                        label = UpdateNameStyle(result[0]),
                        objectType = typeof(GameObject),
                        value = FSMIOUtility.FindGameObjectWithId<IDGenerator>(output)
                    };      
                    if(objectField.value != null)
                    {
                        StateScript.SetVariableValue(result[0], ((GameObject)objectField.value).GetComponent<IDGenerator>().GetUniqueID());
                        selectedGameObject = (GameObject) objectField.value;
                        PopulateComponentDropdown();
                    }
                    objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
                    {
                        StateScript.SetVariableValue(result[0], ((GameObject)objectField.value).GetComponent<IDGenerator>().GetUniqueID());
                        selectedGameObject = (GameObject) objectField.value;
                        PopulateComponentDropdown();
                    });
                    objectField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(objectField);
                    break;
                
                case "selectedComponent":
                    componentDropdown.label = UpdateNameStyle(result[0]);
                    componentDropdown.value = result[2];
        
                    if (componentDropdown.value != null)
                    {
                        selectedComponent = selectedGameObject.GetComponent(componentDropdown.value);
                        StateScript.SetVariableValue(result[0], componentDropdown.value);
                        PopulateFunctionDropdown(functionDropdown);
                    }
                    componentDropdown.RegisterValueChangedCallback(evt =>
                    {
                        string selectedName = evt.newValue;
                        selectedComponent = selectedGameObject.GetComponent(selectedName);
                        StateScript.SetVariableValue(result[0], componentDropdown.value);
                        PopulateFunctionDropdown(functionDropdown);
                    });
                    componentDropdown.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(componentDropdown);
                    break;
                case "selectedFunction":
                    functionDropdown.label = UpdateNameStyle(result[0]);
                    functionDropdown.value = result[2];
                    
                    functionDropdown.RegisterValueChangedCallback(evt =>
                    {
                        StateScript.SetVariableValue(result[0], functionDropdown.value);
                        selectedFunction = evt.newValue;
                    });
                    functionDropdown.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(functionDropdown);
                    break;
                default:
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
    
    private string GetNamespaceAndClassName(string fullTypeName)
    {
        // Split the string by '(' and ')' to get the class name
        string[] parts = fullTypeName.Split('(');
        if (parts.Length > 1)
        {
            string namespaceAndClassName = parts[1].Trim(')').Trim(); // Trim to remove ')' and any leading/trailing spaces
            return namespaceAndClassName;
        }
        return null;
    }

    #endregion

    #region Animator Methods

    private void ShowAnimatorParameterDropdown(Toggle toggle)
    {
        if (!toggle.value)
        {
            if (_hasAnimatorTrigger)
            {
                extensionContainer.RemoveAt(2);
                _hasAnimatorTrigger = false;
            }
            extensionContainer.RemoveAt(1);
            return;
        }

        // Get all animator parameters
        List<string> animatorParameters = GetAllAnimatorParameters();

        // Create a dropdown menu
        DropdownField dropdown = new DropdownField("Select Parameter", animatorParameters, 0);
        dropdown.RegisterValueChangedCallback(evt =>
        {
            // Show text field or float field based on the parameter type
            string selectedParameter = evt.newValue;
            string parameterType = GetParameterType(selectedParameter);

            if (_hasAnimatorTrigger)
            {
                extensionContainer.RemoveAt(2);
            }
            
            if (parameterType == "Float")
            {
                FloatField floatField = new FloatField()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = floatField.value.ToString();
                floatField.AddToClassList("fsm-node_textfield");
                floatField.RegisterCallback<ChangeEvent<float>>(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,floatField);
            }
            else if (parameterType == "Integer")
            {
                IntegerField integerField = new IntegerField()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = integerField.value.ToString();
                integerField.AddToClassList("fsm-node_textfield");
                integerField.RegisterCallback<ChangeEvent<int>>(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,integerField);
            }
            else if (parameterType == "Bool")
            {
                Toggle toggleField = new Toggle()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = toggleField.value.ToString();
                toggleField.AddToClassList("fsm-node_toggle");
                toggleField.RegisterValueChangedCallback(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,toggleField);
            }
            else if (parameterType == "Trigger")
            {
                Toggle toggleField = new Toggle()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = toggleField.value.ToString();
                toggleField.AddToClassList("fsm-node_toggle");
                toggleField.RegisterValueChangedCallback(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,toggleField);
            }
            _hasAnimatorTrigger = true;
        });

        // Add the dropdown menu below the toggle
        extensionContainer.Insert(1, dropdown);
        
        RefreshExpandedState();
    }

    private void AddDropdownFields()
    {
        List<string> animatorParameters = GetAllAnimatorParameters();
        
        DropdownField dropdown = new DropdownField("Select Parameter", animatorParameters, _animatorParameter);
 
        if (_parameterType == "Float")
        {
            FloatField floatField = new FloatField()
            {
                label = dropdown.value,
                value = float.Parse(_animatorValue)
            };
            floatField.AddToClassList("fsm-node_textfield");
            floatField.RegisterCallback<ChangeEvent<float>>(e =>
            {
                _animatorValue = e.newValue.ToString();
            });
            extensionContainer.Insert(1, floatField);
        }
        else if (_parameterType == "Integer")
        {
            IntegerField integerField = new IntegerField()
            {
                label = dropdown.value,
                value = int.Parse(_animatorValue)
            };
            integerField.AddToClassList("fsm-node_textfield");
            integerField.RegisterCallback<ChangeEvent<int>>(e =>
            {
                _animatorValue = e.newValue.ToString();
            });
            extensionContainer.Insert(1, integerField);
        }
        else if (_parameterType == "Bool")
        {
            Toggle toggleField = new Toggle()
            {
                label = dropdown.value,
                value = bool.Parse(_animatorValue)
            };
            toggleField.AddToClassList("fsm-node_toggle");
            toggleField.RegisterValueChangedCallback(e =>
            {
                _animatorValue = e.newValue.ToString();
            });
            extensionContainer.Insert(1, toggleField);
        }
        else if (_parameterType == "Trigger")
        {
            Toggle toggleField = new Toggle()
            {
                label = dropdown.value,
                value = bool.Parse(_animatorValue)
            };
            toggleField.AddToClassList("fsm-node_toggle");
            toggleField.RegisterValueChangedCallback(e =>
            {
                _animatorValue = e.newValue.ToString();
            });
            extensionContainer.Insert(1, toggleField);
        }

        dropdown.RegisterValueChangedCallback(evt =>
        {
            // Show text field or float field based on the parameter type
            string selectedParameter = evt.newValue;
            string parameterType = GetParameterType(selectedParameter);

            if (_hasAnimatorTrigger)
            {
                extensionContainer.RemoveAt(2);
            }
            
            if (parameterType == "Float")
            {
                FloatField floatField = new FloatField()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = floatField.value.ToString();
                floatField.AddToClassList("fsm-node_textfield");
                floatField.RegisterCallback<ChangeEvent<float>>(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,floatField);
            }
            else if (parameterType == "Integer")
            {
                IntegerField integerField = new IntegerField()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = integerField.value.ToString();
                integerField.AddToClassList("fsm-node_textfield");
                integerField.RegisterCallback<ChangeEvent<int>>(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,integerField);
            }
            else if (parameterType == "Bool")
            {
                Toggle toggleField = new Toggle()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = toggleField.value.ToString();
                toggleField.AddToClassList("fsm-node_toggle");
                toggleField.RegisterValueChangedCallback(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,toggleField);
            }
            else if (parameterType == "Trigger")
            {
                Toggle toggleField = new Toggle()
                {
                    label = selectedParameter
                };
                _animatorParameter = selectedParameter;
                _parameterType = parameterType;
                _animatorValue = toggleField.value.ToString();
                toggleField.AddToClassList("fsm-node_toggle");
                toggleField.RegisterValueChangedCallback(e =>
                {
                    _animatorValue = e.newValue.ToString();
                });
                extensionContainer.Insert(2,toggleField);
            }
            _hasAnimatorTrigger = true;
        });
        
        extensionContainer.Insert(1, dropdown);
        
        RefreshExpandedState();
    }

    private string GetParameterType(string parameterName)
    {
        // Get the animator component from your object
        Animator animator = GetAnimatorComponent();

        // Get the type of the parameter
        if (animator != null)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            foreach (AnimatorControllerParameter parameter in parameters)
            {
                if (parameter.name == parameterName)
                {
                    return parameter.type.ToString();
                }
            }
        }

        return "Unknown";
    }
    private void SetAnimatorParameter(string parameterName, object value, string parameterType)
    {
        // Set animator parameter value based on its type
        Animator animator = GetAnimatorComponent();
        if (animator != null)
        {
            switch (parameterType)
            {
                case "Float":
                    animator.SetFloat(parameterName, (float)value);
                    break;
                case "Integer":
                    animator.SetInteger(parameterName, (int)value);
                    break;
                case "Bool":
                    animator.SetBool(parameterName, (bool)value);
                    break;
                default:
                    break;
            }
        }
    }
    private List<string> GetAllAnimatorParameters()
    {
        // Get the animator component from your object
        Animator animator = GetAnimatorComponent();

        // Get all animator parameters
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
        string inspectorJson = EditorPrefs.GetString("FSMInspectorData");
        string pattern = @"\s*\([^)]*\)";
        string result = Regex.Replace(inspectorJson, pattern, "");

        GameObject gameObject = GameObject.Find(result);
        return gameObject.GetComponent<Animator>();
    }

    public override FSMAnimatorSaveData GetAnimatorSaveData()
    {
        FSMAnimatorSaveData animatorSaveData = new FSMAnimatorSaveData();
        animatorSaveData.Initialize(_hasAnimatorTrigger, _animatorParameter, _parameterType, _animatorValue);
        return animatorSaveData;
    }
    
    public override void SetAnimatorSaveData(FSMAnimatorSaveData animatorSaveData)
    {
        _hasAnimatorTrigger = animatorSaveData.TriggerEnable;
        _animatorParameter = animatorSaveData.ParameterName;
        _parameterType = animatorSaveData.ParameterType;
        _animatorValue = animatorSaveData.Value;
    }

    #endregion
}
