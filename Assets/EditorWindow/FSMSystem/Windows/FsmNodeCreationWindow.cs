#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using Utilities;
    public class FsmNodeCreationWindow : EditorWindow
    {
        private enum CreationType
        {
            None,
            State,
            Condition
        }

        private CreationType _creationType = CreationType.None;
        private string _newStateName = "";

        [MenuItem("Window/FSM/Create State or Condition")]
        public static void ShowWindow()
        {
            GetWindow<FsmNodeCreationWindow>("Create State or Condition");
        }

        private void OnGUI()
        {
            GUILayout.Label("Choose what to create:");

            GUILayout.BeginHorizontal();

            // Button to create a new state
            if (GUILayout.Toggle(_creationType == CreationType.State, "Create State", "Button"))
            {
                _creationType = CreationType.State;
            }

            // Button to create a new condition
            if (GUILayout.Toggle(_creationType == CreationType.Condition, "Create Condition", "Button"))
            {
                _creationType = CreationType.Condition;
            }

            GUILayout.EndHorizontal();

            // Show textfield for state name if "Create State" button is selected
            if (_creationType == CreationType.State)
            {
                GUILayout.Label("Enter the name for the new state:");
                _newStateName = EditorGUILayout.TextField("State Name:", _newStateName);
            }

            // Show textfield for condition details if "Create Condition" button is selected
            if (_creationType == CreationType.Condition)
            {
                GUILayout.Label("Enter details for the new condition:");
                _newStateName = EditorGUILayout.TextField("Condition Details:", _newStateName);
            }

            // Button to create the new state or condition
            if (GUILayout.Button("Create"))
            {
                if (_creationType == CreationType.State)
                {
                    CreateNewState(_newStateName);
                }
                else if (_creationType == CreationType.Condition)
                {
                    CreateNewCondition(_newStateName);
                }
            }
        }

        private void CreateNewState(string stateName)
        {
            // Add logic to create a new state with the given name
            FsmCreationNodesUtilities.CreateStateNode(stateName);
        }

        private void CreateNewCondition(string conditionName)
        {
            // Add logic to create a new condition
            FsmCreationNodesUtilities.CreateConditionNode(conditionName);
        }
    }
}
#endif