#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System;
    using UnityEditor;

    /// <summary>
    /// Utility class for drawing custom inspector elements in the Unity Editor.
    /// </summary>
    public static class FsmInspectorUtility
    {
        /// <summary>
        /// Draws fields in a disabled state.
        /// </summary>
        /// <param name="action">The action that draws the fields.</param>
        public static void DrawDisabledFields(Action action)
        {
            EditorGUI.BeginDisabledGroup(true);
            action.Invoke();
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws a header label with bold styling.
        /// </summary>
        /// <param name="label">The label text.</param>
        public static void DrawHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draws a help box with the specified message and type.
        /// </summary>
        /// <param name="message">The help box message.</param>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="wide">Whether the help box should be wide.</param>
        public static void DrawHelpBox(string message, MessageType messageType = MessageType.Info, bool wide = true)
        {
            EditorGUILayout.HelpBox(message, messageType, wide);
        }

        /// <summary>
        /// Draws a property field for the given serialized property.
        /// </summary>
        /// <param name="serializedProperty">The serialized property to draw.</param>
        /// <returns>True if the property has been modified; otherwise, false.</returns>
        public static bool DrawPropertyField(this SerializedProperty serializedProperty)
        {
            return EditorGUILayout.PropertyField(serializedProperty);
        }

        /// <summary>
        /// Draws a space of the specified amount.
        /// </summary>
        /// <param name="amount">The amount of space to draw.</param>
        public static void DrawSpace(int amount = 4)
        {
            EditorGUILayout.Space(amount);
        }
    }
}
#endif