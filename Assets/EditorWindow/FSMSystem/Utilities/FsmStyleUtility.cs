#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// Utility class for adding classes and style sheets to VisualElements in Unity's UI Toolkit.
    /// </summary>
    public static class FsmStyleUtility
    {
        /// <summary>
        /// Adds one or more classes to a VisualElement.
        /// </summary>
        /// <param name="element">The VisualElement to which classes will be added.</param>
        /// <param name="classNames">The names of the classes to add.</param>
        /// <returns>The modified VisualElement.</returns>
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (var className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        /// <summary>
        /// Adds one or more style sheets to a VisualElement.
        /// </summary>
        /// <param name="element">The VisualElement to which style sheets will be added.</param>
        /// <param name="styleSheetNames">The names of the style sheets to add.</param>
        /// <returns>The modified VisualElement.</returns>
        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (var styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }
    }
}
#endif