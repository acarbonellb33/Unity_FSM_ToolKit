#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System.IO;
    using UnityEditor.Compilation;
    using UnityEngine;
    public static class FSMCreationNodesUtilities
    {
        #region NodeCreationMethods

        public static void CreateStateNode(string stateName)
        {
            GenerateDataScript(stateName);
            GenerateStateScript(stateName);
            ModifyIOUtility(stateName,
                "private static StateScriptData LoadFromJson(FSMNode node, string nodeName, string path)");
            ModifyIOUtility(stateName, "private static StateScriptData LoadFromJson(FSMNode node)");
            ModifyFSMGraphSaveDataState(stateName);
            ModifyStateNode(stateName);
            CompilationPipeline.RequestScriptCompilation();
        }

        public static void CreateConditionNode(string stateName)
        {
            GenerateDataScript(stateName);
            GenerateConditionScript(stateName);
            ModifyIOUtility(stateName,
                "private static StateScriptData LoadFromJson(FSMNode node, string nodeName, string path)");
            ModifyIOUtility(stateName, "private static StateScriptData LoadFromJson(FSMNode node)");
            ModifyFSMGraphSaveDataCondition(stateName);
            ModifyFSMGraphSaveDataDoubleCondition(stateName);
            ModifyTransitionNode(stateName);
            ModifyDualTransitionNode(stateName);
            CompilationPipeline.RequestScriptCompilation();
        }

        private static void ModifyIOUtility(string stateName, string indexText)
        {
            // Path to the FSMIOUtility script
            string scriptPath = "Assets/EditorWindow/FSMSystem/Utilities/FSMIOUtility.cs";

            // Read the content of the script
            string scriptContent = System.IO.File.ReadAllText(scriptPath);

            // Find the index of the LoadFromJson method
            int loadFromJsonIndex = scriptContent.IndexOf(indexText);

            if (loadFromJsonIndex != -1)
            {
                // Find the index of the switch statement inside the LoadFromJson method
                int switchIndex = scriptContent.IndexOf("switch (newName)", loadFromJsonIndex);

                if (switchIndex != -1)
                {
                    // Find the index of the closing curly brace of the switch statement
                    int switchEndIndex = scriptContent.IndexOf("}", switchIndex);

                    if (switchEndIndex != -1)
                    {
                        // Define the new case for the state type
                        string newCase = $@"
            case ""{stateName}"":
                {stateName}Data {char.ToLowerInvariant(stateName[0]) + stateName.Substring(1)}Data = JsonUtility.FromJson<{stateName}Data>(json);
                node.StateScript = {char.ToLowerInvariant(stateName[0]) + stateName.Substring(1)}Data;
                break;
            ";

                        // Insert the new case before the closing curly brace of the switch statement
                        scriptContent = scriptContent.Insert(switchEndIndex, newCase);

                        // Write the modified content back to the script file
                        System.IO.File.WriteAllText(scriptPath, scriptContent);
                    }
                }
            }
        }

        private static void ModifyFSMGraphSaveDataState(string stateName)
        {
            // Path to the FSMGraphView script
            string scriptPath = "Assets/EditorWindow/FSMSystem/Windows/FSMGraphView.cs";

            // Read the content of the script
            string scriptContent = File.ReadAllText(scriptPath);

            // Modify the content to add the desired line
            string newLineToAdd = $"\t\tthis.AddManipulator(CreateStateItemMenu(\"{stateName}\"));\n";
            int insertionIndex = scriptContent.IndexOf("private void AddManipulators()");

            if (insertionIndex != -1)
            {
                // Find the end of the AddManipulators() method
                int endIndex = scriptContent.IndexOf("private ", insertionIndex + 1);
                if (endIndex == -1)
                {
                    endIndex = scriptContent
                        .Length; // If "private " is not found, set the end index to the end of the file
                }
                else
                {
                    // Adjust the index to include the "private " keyword
                    endIndex -= 1;
                }

                // Insert the new line of code before the end of the method
                scriptContent = scriptContent.Insert(endIndex - 10, newLineToAdd);
            }

            // Write the modified content back to the script file
            File.WriteAllText(scriptPath, scriptContent);
        }

        private static void ModifyFSMGraphSaveDataCondition(string stateName)
        {
            // Path to the FSMGraphView script
            string scriptPath = "Assets/EditorWindow/FSMSystem/Windows/FSMGraphView.cs";

            // Read the content of the script
            string scriptContent = File.ReadAllText(scriptPath);

            // Modify the content to add the desired line
            string newLineToAdd = $"\t\tthis.AddManipulator(CreateTransitionItemMenu(\"{stateName}\"));\n";
            int insertionIndex = scriptContent.IndexOf("private void AddManipulators()");

            if (insertionIndex != -1)
            {
                // Find the end of the AddManipulators() method
                int endIndex = scriptContent.IndexOf("private ", insertionIndex + 1);
                if (endIndex == -1)
                {
                    endIndex = scriptContent
                        .Length; // If "private " is not found, set the end index to the end of the file
                }
                else
                {
                    // Adjust the index to include the "private " keyword
                    endIndex -= 1;
                }

                // Insert the new line of code before the end of the method
                scriptContent = scriptContent.Insert(endIndex - 10, newLineToAdd);
            }

            // Write the modified content back to the script file
            File.WriteAllText(scriptPath, scriptContent);
        }

        private static void ModifyFSMGraphSaveDataDoubleCondition(string stateName)
        {
            // Path to the FSMGraphView script
            string scriptPath = "Assets/EditorWindow/FSMSystem/Windows/FSMGraphView.cs";

            // Read the content of the script
            string scriptContent = File.ReadAllText(scriptPath);

            // Modify the content to add the desired line
            string newLineToAdd = $"\t\tthis.AddManipulator(CreateDualTransitionStateItemMenu(\"{stateName}\"));\n";
            int insertionIndex = scriptContent.IndexOf("private void AddManipulators()");

            if (insertionIndex != -1)
            {
                // Find the end of the AddManipulators() method
                int endIndex = scriptContent.IndexOf("private ", insertionIndex + 1);
                if (endIndex == -1)
                {
                    endIndex = scriptContent
                        .Length; // If "private " is not found, set the end index to the end of the file
                }
                else
                {
                    // Adjust the index to include the "private " keyword
                    endIndex -= 1;
                }

                // Insert the new line of code before the end of the method
                scriptContent = scriptContent.Insert(endIndex - 10, newLineToAdd);
            }

            // Write the modified content back to the script file
            File.WriteAllText(scriptPath, scriptContent);
        }

        private static void ModifyTransitionNode(string stateName)
        {
            // Path to the script file
            string scriptPath = "Assets/EditorWindow/FSMSystem/Elements/FSMTransitionNode.cs";

            try
            {
                // Read the content of the script file
                string scriptContent = File.ReadAllText(scriptPath);

                // Find the line where _dataObjects list is initialized
                int dataObjectsIndex = scriptContent.IndexOf("_dataObjects = new List<StateScriptData>()");

                if (dataObjectsIndex != -1)
                {
                    // Construct the code for the new state
                    string newStateLine = "new " + stateName + "Data(), "; // Example: new YourNewStateData(),

                    // Insert the new state code into the _dataObjects list initialization line
                    int insertionIndex = scriptContent.IndexOf("{", dataObjectsIndex) + 1;
                    scriptContent = scriptContent.Insert(insertionIndex, newStateLine);

                    // Write the modified content back to the script file
                    File.WriteAllText(scriptPath, scriptContent);
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"An error occurred while modifying the script: {e.Message}");
            }
        }

        private static void ModifyDualTransitionNode(string stateName)
        {
            // Path to the script file
            string scriptPath = "Assets/EditorWindow/FSMSystem/Elements/FSMDualTransitionNode.cs";

            try
            {
                // Read the content of the script file
                string scriptContent = File.ReadAllText(scriptPath);

                // Find the line where _dataObjects list is initialized
                int dataObjectsIndex = scriptContent.IndexOf("_dataObjects = new List<StateScriptData>()");

                if (dataObjectsIndex != -1)
                {
                    // Construct the code for the new state
                    string newStateLine = "new " + stateName + "Data(), "; // Example: new YourNewStateData(),

                    // Insert the new state code into the _dataObjects list initialization line
                    int insertionIndex = scriptContent.IndexOf("{", dataObjectsIndex) + 1;
                    scriptContent = scriptContent.Insert(insertionIndex, newStateLine);

                    // Write the modified content back to the script file
                    File.WriteAllText(scriptPath, scriptContent);
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"An error occurred while modifying the script: {e.Message}");
            }
        }

        private static void ModifyStateNode(string stateName)
        {
            // Path to the script file
            string scriptPath = "Assets/EditorWindow/FSMSystem/Elements/FSMStateNode.cs";

            try
            {
                // Read the content of the script file
                string scriptContent = File.ReadAllText(scriptPath);

                // Find the line where _dataObjects list is initialized
                int dataObjectsIndex = scriptContent.IndexOf("_dataObjects = new List<StateScriptData>()");

                if (dataObjectsIndex != -1)
                {
                    // Construct the code for the new state
                    string newStateLine = "new " + stateName + "Data(), "; // Example: new YourNewStateData(),

                    // Insert the new state code into the _dataObjects list initialization line
                    int insertionIndex = scriptContent.IndexOf("{", dataObjectsIndex) + 1;
                    scriptContent = scriptContent.Insert(insertionIndex, newStateLine);

                    // Write the modified content back to the script file
                    File.WriteAllText(scriptPath, scriptContent);
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"An error occurred while modifying the script: {e.Message}");
            }
        }

        private static void GenerateDataScript(string stateName)
        {
            // Define the content of the script
            string scriptContent = $@"public class {stateName}Data : StateScriptData
{{
    public {stateName}Data()
    {{
        SetStateName(""{stateName}"");
    }}
}}
";
            File.WriteAllText($"Assets/FSM/Nodes/States/StatesData/{stateName}Data.cs", scriptContent);
        }

        private static void GenerateStateScript(string stateName)
        {
            // Define the content of the script
            string scriptContent = $@"public class {stateName}StateScript : StateScript, IAction
{{
    //Add any properties specific to this state

    public {stateName}StateScript()
    {{
        // Set the state name to '{stateName}' using the SetStateName method inherited from StateScript
        SetStateName(""{stateName}"");
    }}

    // Override the Execute method from the base StateScript class
    public void Execute()
    {{
        // Add the logic for this state
    }}
}}
";
            // Write the content to the specified file path
            File.WriteAllText($"Assets/FSM/Nodes/States/StateScripts/{stateName}StateScript.cs", scriptContent);
        }

        private static void GenerateConditionScript(string stateName)
        {
            // Define the content of the script
            string scriptContent = $@"using UnityEngine;

public class {stateName}StateScript : StateScript, ICondition
{{
    //Add any properties specific to this state

     public {stateName}StateScript()
    {{
        // Set the state name to '{stateName}' using the SetStateName method inherited from StateScript
        SetStateName(""{stateName}"");
    }}

    // Override the Execute method from the base StateScript class
    public bool Condition()
    {{
        // Add the logic for this state
        return true; // Now returning true, replace with your logic
    }}
}}
";
            // Write the content to the specified file path
            File.WriteAllText($"Assets/FSM/Nodes/States/StateScripts/{stateName}ConditionScript.cs", scriptContent);
        }

        #endregion
    }
}
#endif