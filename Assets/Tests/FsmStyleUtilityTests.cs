namespace Tests
{
    using NUnit.Framework;
    using UnityEngine.UIElements;
    using EditorWindow.FSMSystem.Utilities;
    public class FsmStyleUtilityTests
    {
        [Test]
        public void AddClasses_AddsClassesToVisualElement()
        {
            // Arrange
            var element = new VisualElement();

            // Act
            var result = FsmStyleUtility.AddClasses(element, "class1", "class2");

            // Assert
            Assert.IsTrue(element.ClassListContains("class1"));
            Assert.IsTrue(element.ClassListContains("class2"));
            Assert.AreEqual(element, result);
        }

        [Test]
        public void AddStyleSheets_AddsStyleSheetsToVisualElement()
        {
            // Arrange
            var element = new VisualElement();

            // Act
            var result = FsmStyleUtility.AddStyleSheets(element, "FSMSystem/FSMToolbarStyle.uss", "FSMSystem/FSMVariables.uss");

            // Assert
            // Since EditorGUIUtility.Load is not accessible in unit tests, we cannot verify the exact style sheet addition
            Assert.AreEqual(2, element.styleSheets.count);
            Assert.AreEqual(element, result);
        }
    }
}