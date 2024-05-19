#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Class representing a popup window with instructions on enabling hit state override.
    /// </summary>
    public class FsmHoverPopupWindow : PopupWindowContent
    {
        /// <summary>
        /// Draws the GUI for the popup window.
        /// </summary>
        /// <param name="rect">The rectangle within which to draw the GUI.</param>
        public override void OnGUI(Rect rect)
        {
            // Display instruction text in the popup window
            EditorGUI.LabelField(new Rect(5, 0, rect.width, 20), 
                "To be able to override the hit state,", EditorStyles.label);
            EditorGUI.LabelField(new Rect(5, 15, rect.width, 20), 
                "you must enable it globally", EditorStyles.label);
        }

        /// <summary>
        /// Gets the size of the popup window.
        /// </summary>
        /// <returns>The size of the popup window.</returns>
        public override Vector2 GetWindowSize()
        {
            return new Vector2(210, 38); // Set the size of the popup window
        }
    }
}
#endif