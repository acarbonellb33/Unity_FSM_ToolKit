using UnityEditor;
using UnityEngine;

public class FSMNodeCreationWindow : EditorWindow
{
    private enum CreationType { None, State, Condition }
    private CreationType creationType = CreationType.None;
    private string newStateName = "";

    [MenuItem("Window/FSM/Create State or Condition")]
    public static void ShowWindow()
    {
        GetWindow<FSMNodeCreationWindow>("Create State or Condition");
    }

    private void OnGUI()
    {
        GUILayout.Label("Choose what to create:");

        GUILayout.BeginHorizontal();

        // Button to create a new state
        if (GUILayout.Toggle(creationType == CreationType.State, "Create State", "Button"))
        {
            creationType = CreationType.State;
        }

        // Button to create a new condition
        if (GUILayout.Toggle(creationType == CreationType.Condition, "Create Condition", "Button"))
        {
            creationType = CreationType.Condition;
        }

        GUILayout.EndHorizontal();

        // Show textfield for state name if "Create State" button is selected
        if (creationType == CreationType.State)
        {
            GUILayout.Label("Enter the name for the new state:");
            newStateName = EditorGUILayout.TextField("State Name:", newStateName);
        }

        // Show textfield for condition details if "Create Condition" button is selected
        if (creationType == CreationType.Condition)
        {
            GUILayout.Label("Enter details for the new condition:");
            newStateName = EditorGUILayout.TextField("Condition Details:", newStateName);
        }

        // Button to create the new state or condition
        if (GUILayout.Button("Create"))
        {
            if (creationType == CreationType.State)
            {
                CreateNewState(newStateName);
            }
            else if (creationType == CreationType.Condition)
            {
                CreateNewCondition(newStateName);
            }
        }
    }

    private void CreateNewState(string stateName)
    {
        // Add logic to create a new state with the given name
        Debug.Log("Creating new state: " + stateName);
        FSMCreationNodesUtilities.CreateStateNode(stateName);
    }

    private void CreateNewCondition(string conditionName)
    {
        // Add logic to create a new condition
        Debug.Log("Creating new condition" + conditionName);
        FSMCreationNodesUtilities.CreateConditionNode(conditionName);
    }
}
