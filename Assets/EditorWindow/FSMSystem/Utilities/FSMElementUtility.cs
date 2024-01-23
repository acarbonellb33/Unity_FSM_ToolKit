using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public static class FSMElementUtility 
{
    public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textField = new TextField()
        {
            value = value,
            label = label
        };
        if (onValueChanged != null)
        {
            textField.RegisterValueChangedCallback(onValueChanged);
        }
        return textField;
    }

    public static TextField CreateTextArea(string value = null, string label = null,
        EventCallback<ChangeEvent<string>> onValueChanged = null)
    {
        TextField textArea = CreateTextField(value, label, onValueChanged);
        textArea.multiline = true;
        return textArea;
    }
    
    public static Foldout CreateFoldout(string title, bool collapsed = false)
    {
        Foldout foldout = new Foldout()
        {
            text = title,
            value = !collapsed
        };
        return foldout;
    }
    
    public static Button CreateButton(string text, Action onClick = null)
    {
        Button button = new Button(onClick)
        {
            text = text
        };
        return button;
    }
    
    public static Port CreatePort(this FSMNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Input, 
        Port.Capacity capacity = Port.Capacity.Single)
    {
        Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
        port.portName = portName;
        return port;
    }
    
}
