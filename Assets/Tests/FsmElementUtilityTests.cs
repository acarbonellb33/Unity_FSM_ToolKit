namespace Tests
{
    using NUnit.Framework;
    using UnityEngine;
    using System.Collections.Generic;
    using EditorWindow.FSMSystem.Utilities;
    
    public class FsmElementUtilityTests
    {
        [Test]
        public void CreateTextField_CreatesTextFieldWithInitialValueAndLabel()
        {
            var textField = FsmElementUtility.CreateTextField("Initial Value", "Text Field Label");

            Assert.AreEqual("Initial Value", textField.value);
            Assert.AreEqual("Text Field Label", textField.label);
        }
        [Test]
        public void CreatePopupField_CreatesPopupFieldWithChoicesAndDefaultIndex()
        {
            var choices = new List<string> { "Choice1", "Choice2", "Choice3" };
            var defaultIndex = 1;
            var popupField = FsmElementUtility.CreatePopupField(choices, defaultIndex);

            Assert.AreEqual(choices, popupField.choices);
            Assert.AreEqual(choices[defaultIndex], popupField.value);
        }
        [Test]
        public void CreateLabel_CreatesLabelWithText()
        {
            string labelText = "Label Text";
            var label = FsmElementUtility.CreateLabel(labelText);

            Assert.AreEqual(labelText, label.text);
        }
        [Test]
        public void CreateTextArea_CreatesTextAreaWithInitialValueAndLabel()
        {
            string initialValue = "Initial Value";
            string label = "Text Area Label";
            var textArea = FsmElementUtility.CreateTextArea(initialValue, label);

            Assert.AreEqual(initialValue, textArea.value);
            Assert.AreEqual(label, textArea.label);
            Assert.IsTrue(textArea.multiline);
        }
        [Test]
        public void CreateObjectField_CreatesObjectFieldWithLabelAndValue()
        {
            string label = "Object Field Label";
            var gameObject = new GameObject();
            var objectField = FsmElementUtility.CreateObjectField(label, gameObject);

            Assert.AreEqual(label, objectField.label);
            Assert.AreEqual(gameObject, objectField.value);
        }
        [Test]
        public void CreateFoldout_CreatesFoldoutWithTitleAndInitialState()
        {
            string title = "Foldout Title";
            bool collapsed = true;
            var foldout = FsmElementUtility.CreateFoldout(title, collapsed);

            Assert.AreEqual(title, foldout.text);
            Assert.AreEqual(!collapsed, foldout.value);
        }
        [Test]
        public void CreateToggle_CreatesToggleWithLabelAndInitialValue()
        {
            var toggle = FsmElementUtility.CreateToggle("Toggle Label", true);

            Assert.AreEqual("Toggle Label", toggle.label);
            Assert.AreEqual(true, toggle.value);
        }
    }
}