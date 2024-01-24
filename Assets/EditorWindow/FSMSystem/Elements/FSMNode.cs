using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMNode : Node
{
    public string Id { get; set; }
    public string StateName { get; set; }
    public List<FSMConnectionSaveData> Choices { get; set; }
    public ScriptableObject ScriptableObject { get; set; }
    public FSMDialogueType DialogueType { get; set; }
    public FSMGroup Group { get; set; }
    
    private FSMGraphView _graphView;
    
    private int _popupIndex;
    private VisualElement _tempElement;

    public virtual void Initialize(string nodeName, FSMGraphView graphView, Vector2 postition)
    {
        Id = Guid.NewGuid().ToString();
        StateName = nodeName;
        Choices = new List<FSMConnectionSaveData>();
        _graphView = graphView;
        SetPosition(new Rect(postition, Vector2.zero));
        _tempElement = null;
        _popupIndex = 0;
        AddManipulators();
    }
    
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Disconnect Input Ports", action => DisconnectPorts(inputContainer));
        evt.menu.AppendAction("Disconnect Output Ports", action => DisconnectPorts(outputContainer));
        base.BuildContextualMenu(evt);
    }

    public virtual void Draw()
    {
        TextField stateNameField = FSMElementUtility.CreateTextField(StateName, null, callback =>
        {
            if (Group == null)
            {
                _graphView.RemoveUngroupedNode(this);
                StateName = callback.newValue;
                _graphView.AddUngroupedNode(this);
                return;
            }
            FSMGroup currentGroup = Group;
            _graphView.RemoveGroupedNode(this, Group);
            StateName = callback.newValue;
            _graphView.AddGroupedNode(this, currentGroup);
        });
        stateNameField.AddClasses("fsm-node_textfield", "fsm-node_filename-textfield", "fsm-node_textfield_hidden");
        titleContainer.Insert(0, stateNameField);
        
        Port inputPort = this.CreatePort("Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
        inputContainer.Add(inputPort);
        
        VisualElement customDataContainer = new VisualElement();
        customDataContainer.AddToClassList("fsm-node_custom-data-container");
        
        Foldout foldout = FSMElementUtility.CreateFoldout("State Data");
        
        /*TextField textField = new TextField()
        {
            value = Text
        };
        textField.AddToClassList("fsm-node_textfield");
        textField.AddToClassList("fsm-node_quote-textfield");
        textField.AddToClassList("fsm-node_textfield_hidden");
        
        foldout.Add(textField);*/
    
        ObjectField objectField = new ObjectField("Enemy State");
        objectField.objectType = typeof(AttackState);

        List<string> choices = new List<string>() {"Patrol", "Attack", "Idle"};
        List<EnemyState> scriptableObjects = new List<EnemyState>() {ScriptableObject.CreateInstance<PatrolState>(), ScriptableObject.CreateInstance<AttackState>()};

        PopupField<string> popupField = FSMElementUtility.CreatePopupField(choices, _popupIndex, callback =>
        {
            if (_popupIndex != choices.IndexOf(callback.newValue))
            {
                _popupIndex = choices.IndexOf(callback.newValue);
                _tempElement.Clear();
                CreateStateAttribute(scriptableObjects[_popupIndex].InspectVariables(), foldout);
            }
        });
        foldout.Add(popupField);
        customDataContainer.Add(foldout);
        
        CreateStateAttribute(scriptableObjects[_popupIndex].InspectVariables(), foldout);
        
        extensionContainer.Add(customDataContainer);

        RefreshExpandedState();
    }

    private void CreateStateAttribute(List<string> attributes, Foldout foldout)
    {
        VisualElement stateAttributeContainer = new VisualElement();
        stateAttributeContainer.AddToClassList("fsm-node_state-attribute-container");

        foreach(string attribute in attributes)
        {
            string[] result = attribute.Split(',');
            
            Label stateAttributeLabel = new Label(result[0]);
            stateAttributeLabel.AddToClassList("fsm-node_state-attribute-label");
            stateAttributeContainer.Add(stateAttributeLabel);

            switch (result[1])
            {
                case "System.Single":
                    FloatField floatField = new FloatField()
                    {
                        value = float.Parse(result[2])
                    };
                    floatField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(floatField);
                    break;
                case "System.Int32":
                    IntegerField integerField = new IntegerField()
                    {
                        value = int.Parse(result[2])
                    };
                    integerField.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(integerField);
                    break;
                case "System.Boolean":
                    Toggle toggle = new Toggle()
                    {
                        value = bool.Parse(result[2])
                    };
                    toggle.AddToClassList("fsm-node_state-attribute-field");
                    stateAttributeContainer.Add(toggle);
                    break;
                case "System.String":
                    TextField textField = new TextField()
                    {
                        value = result[2]
                    };
                    textField.AddToClassList("fsm-node_state-attribute-field");
                    break;
                default:
                    break;
            }
        }
        _tempElement = stateAttributeContainer;
        foldout.Add(stateAttributeContainer);
    }

    private void AddManipulators()
    {
        this.AddManipulator(CreateNodeContectualMenu());
    }
    private IManipulator CreateNodeContectualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Create Transition", menuActionEvent => Debug.Log("Create Transition")));
        
        return contextualMenuManipulator;
    }
    
    public void DisconnectAllPorts()
    {
        DisconnectPorts(inputContainer);
        DisconnectPorts(outputContainer);
    }

    private void DisconnectPorts(VisualElement container)
    {
        foreach(Port port in container.Children())
        {
            if (!port.connected)
            {
                continue;
            }
            _graphView.DeleteElements(port.connections);
        }
    }
    
    public void SetErrorStyle(Color color)
    {
        mainContainer.style.backgroundColor = color;
    }
    
    public void ResetStyle()
    {
        mainContainer.style.backgroundColor = new Color(29f/255f, 29f/255f, 30f/255f);
    }
}
