#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System;
    using System.Collections.Generic;
    using Elements;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using FSM.Utilities;

    /// <summary>
    /// Utility class for creating various UI elements for the FSM editor.
    /// </summary>
    public static class FsmElementUtility
    {
        /// <summary>
        /// Creates a text field.
        /// </summary>
        /// <param name="value">Initial value of the text field.</param>
        /// <param name="label">Label for the text field.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created TextField.</returns>
        public static TextField CreateTextField(string value = null, string label = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField
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

        /// <summary>
        /// Creates a popup field.
        /// </summary>
        /// <param name="choices">List of choices for the popup field.</param>
        /// <param name="index">Index of the default choice.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created PopupField.</returns>
        public static PopupField<string> CreatePopupField(List<string> choices, int index = 0,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            PopupField<string> popupField = new PopupField<string>(choices, index);
            if (onValueChanged != null)
            {
                popupField.RegisterValueChangedCallback(onValueChanged);
            }
            return popupField;
        }

        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <param name="text">Text of the label.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created Label.</returns>
        public static Label CreateLabel(string text = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            Label label = new Label
            {
                text = text
            };
            if (onValueChanged != null)
            {
                label.RegisterValueChangedCallback(onValueChanged);
            }
            return label;
        }

        /// <summary>
        /// Creates a text area.
        /// </summary>
        /// <param name="value">Initial value of the text area.</param>
        /// <param name="label">Label for the text area.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created TextField as a text area.</returns>
        public static TextField CreateTextArea(string value = null, string label = null,
            EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;
            return textArea;
        }

        /// <summary>
        /// Creates a custom object field.
        /// </summary>
        /// <typeparam name="T">Type of the object field.</typeparam>
        /// <param name="label">Label for the object field.</param>
        /// <param name="value">Initial value of the object field.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created CustomObjectField.</returns>
        public static CustomObjectField CreateObjectField<T>(string label = null, T value = null,
            EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null) where T : UnityEngine.Object
        {
            CustomObjectField customObjectField = new CustomObjectField(label)
            {
                objectType = typeof(T),
                value = value,
                allowSceneObjects = false
            };
            if (onValueChanged != null)
            {
                customObjectField.RegisterValueChangedCallback(onValueChanged);
            }
            return customObjectField;
        }

        /// <summary>
        /// Creates a foldout element.
        /// </summary>
        /// <param name="title">Title of the foldout.</param>
        /// <param name="collapsed">Whether the foldout is collapsed initially.</param>
        /// <returns>Returns the created Foldout.</returns>
        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            return new Foldout
            {
                text = title,
                value = !collapsed
            };
        }

        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="text">Text of the button.</param>
        /// <param name="onClick">Action to be performed on button click.</param>
        /// <returns>Returns the created Button.</returns>
        public static Button CreateButton(string text, Action onClick = null)
        {
            return new Button(onClick)
            {
                text = text
            };
        }

        /// <summary>
        /// Creates a toggle.
        /// </summary>
        /// <param name="label">Label for the toggle.</param>
        /// <param name="value">Initial value of the toggle.</param>
        /// <param name="onValueChanged">Event callback for value change.</param>
        /// <returns>Returns the created Toggle.</returns>
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

        /// <summary>
        /// Creates a port for a node.
        /// </summary>
        /// <param name="node">The node to which the port is added.</param>
        /// <param name="portName">Name of the port.</param>
        /// <param name="orientation">Orientation of the port.</param>
        /// <param name="direction">Direction of the port.</param>
        /// <param name="capacity">Capacity of the port.</param>
        /// <returns>Returns the created Port.</returns>
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
