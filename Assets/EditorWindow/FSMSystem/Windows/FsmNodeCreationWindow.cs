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
            
            if (GUILayout.Toggle(_creationType == CreationType.State, "Create State", "Button"))
            {
                _creationType = CreationType.State;
            }

            if (GUILayout.Toggle(_creationType == CreationType.Condition, "Create Condition", "Button"))
            {
                _creationType = CreationType.Condition;
            }

            GUILayout.EndHorizontal();
            
            if (_creationType == CreationType.State)
            {
                GUILayout.Label("Enter the name for the new state:");
                _newStateName = EditorGUILayout.TextField("State Name:", _newStateName);
            }
            
            if (_creationType == CreationType.Condition)
            {
                GUILayout.Label("Enter details for the new condition:");
                _newStateName = EditorGUILayout.TextField("Condition Details:", _newStateName);
            }
            
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
            FsmCreationNodesUtilities.CreateStateNode(stateName);
        }

        private void CreateNewCondition(string conditionName)
        {
            FsmCreationNodesUtilities.CreateConditionNode(conditionName);
        }
    }
}
#endif