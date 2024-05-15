using DefaultNamespace;
using UnityEditor.UIElements;

#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System;
    using System.Collections.Generic;
    using Elements;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    public static class FsmElementUtility
    {
        public static TextField CreateTextField(string value = null, string label = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
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

        public static PopupField<string> CreatePopupField(List<string> choices, int index = 0,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            PopupField<string> popupField = new PopupField<string>()
            {
                choices = choices,
                index = index
            };
            if (onValueChanged != null)
            {
                popupField.RegisterValueChangedCallback(onValueChanged);
            }

            return popupField;
        }

        public static Label CreateLabel(string text = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            Label label = new Label()
            {
                text = text
            };
            if (onValueChanged != null)
            {
                label.RegisterValueChangedCallback(onValueChanged);
            }

            return label;
        }

        public static TextField CreateTextArea(string value = null, string label = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }
        
        public static CustomObjectField CreateObjectField<T>(string label = null, T value = null,
            EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null) where T : UnityEngine.Object
        {
            CustomObjectField customObjectField = new CustomObjectField(label);
            customObjectField.objectType = typeof(T);
            customObjectField.value = value;
            customObjectField.label = label;
            customObjectField.allowSceneObjects = false;

            if (onValueChanged != null)
            {
                customObjectField.RegisterValueChangedCallback(onValueChanged);
            }

            return customObjectField;
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
        
        public static Toggle CreateToggle(string label = null, bool value = false, 
            EventCallback<ChangeEvent<bool>> onValueChanged = null)
        {
            Toggle toggle = new Toggle(label)
            {
                value = value
            };

            if (onValueChanged != null)
            {
                toggle.RegisterValueChangedCallback(onValueChanged);
            }

            return toggle;
        }


        public static Port CreatePort(this FsmNode node, string portName = "",
            Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Input,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));
            port.portName = portName;
            port.AddToClassList("fsm-node_port");
            return port;
        }
    }
}
#endif