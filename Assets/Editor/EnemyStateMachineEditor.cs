using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;

public class EnemyStateMachineEditor : EditorWindow
{
    [System.Serializable]
    public class EnemyState : ScriptableObject
    {
        public string name;
        public string condition;
        public string nextState;

        public static EnemyState CreateInstance()
        {
            return CreateInstance<EnemyState>();
        }
    }

    // Serialized data for the overall enemy state machine
    public class EnemyStateMachineData : ScriptableObject
    {
        public List<EnemyState> states = new List<EnemyState>();
        
        public List<string> stateNames = new List<string>();
        
        public List<string> proximityConditionNames = new List<string>();

        public void AddState(string name)
        {
            stateNames.Add(name);
        }
        
        public void AddProximityCondition(string name)
        {
            proximityConditionNames.Add(name);
        }
    }

    private EnemyStateMachineData _stateMachineData;

    // Editor window initialization
    [MenuItem("Window/Enemy State Machine Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EnemyStateMachineEditor));
    }
    
    private void OnEnable()
    {
        if (_stateMachineData == null)
        {
            // Load or create the data asset
            _stateMachineData = AssetDatabase.LoadAssetAtPath<EnemyStateMachineData>("Assets/Editor/EnemyStateMachineData.asset");

            if (_stateMachineData == null)
            {
                _stateMachineData = CreateInstance<EnemyStateMachineData>();
                _stateMachineData.AddState("PatrolState");
                _stateMachineData.AddState("AttackState");
                _stateMachineData.AddProximityCondition("HearingCondition");
                AssetDatabase.CreateAsset(_stateMachineData, "Assets/Editor/EnemyStateMachineData.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }
    
    private void OnGUI()
    {
        if (_stateMachineData == null)
        {
            // Load or create the data asset
            _stateMachineData = AssetDatabase.LoadAssetAtPath<EnemyStateMachineData>("Assets/Editor/EnemyStateMachineData.asset");

            if (_stateMachineData == null)
            {
                _stateMachineData = CreateInstance<EnemyStateMachineData>();
                _stateMachineData.AddState("PatrolState");
                _stateMachineData.AddState("AttackState");
                _stateMachineData.AddProximityCondition("HearingCondition");
                AssetDatabase.CreateAsset(_stateMachineData, "Assets/Editor/EnemyStateMachineData.asset");
                AssetDatabase.SaveAssets();
            }
        }
       
        string[] addedStates = _stateMachineData.stateNames.ToArray();
        string[] addedProximityConditions = _stateMachineData.proximityConditionNames.ToArray();
        
        EditorGUILayout.LabelField("Enemy State Machine Editor", EditorStyles.boldLabel);

        // Display the states in the editor
        for (int i = 0; i < _stateMachineData.states.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            
            int selectedIndex = Array.IndexOf(addedStates, _stateMachineData.states[i].name);
            selectedIndex = EditorGUILayout.Popup("State Name", selectedIndex, addedStates);
        
            // Update the state name based on the selected index
            if (selectedIndex >= 0 && selectedIndex < addedStates.Length)
            {
                _stateMachineData.states[i].name = addedStates[selectedIndex];
            }
            
            int index = Array.IndexOf(addedProximityConditions, _stateMachineData.states[i].condition);
            index = EditorGUILayout.Popup("Proximity Condition", index, addedProximityConditions);

            // Update the state name based on the selected index
            if (index >= 0 && index < addedProximityConditions.Length)
            {
                _stateMachineData.states[i].condition = addedProximityConditions[index];
            }
            
            //stateMachineData.states[i].proximityCondition = EditorGUILayout.FloatField("Proximity Condition", stateMachineData.states[i].proximityCondition);
            //stateMachineData.states[i].nextState = EditorGUILayout.TextField("Next State", stateMachineData.states[i].nextState);
            
            List<string> nonRepetedStates = addedStates.ToList();
            if (nonRepetedStates.Contains(_stateMachineData.states[i].name))
            {
                nonRepetedStates.Remove(_stateMachineData.states[i].name);
            }

            int selectedIndex2 = Array.IndexOf(nonRepetedStates.ToArray(), _stateMachineData.states[i].nextState);
            selectedIndex2 = EditorGUILayout.Popup("Next State", selectedIndex2, nonRepetedStates.ToArray());
        
            // Update the state name based on the selected index
            if (selectedIndex2 >= 0 && selectedIndex2 < nonRepetedStates.Count)
            {
                _stateMachineData.states[i].nextState = nonRepetedStates[selectedIndex2];
            }

            // Remove State button
            if (GUILayout.Button("Remove State"))
            {
                EnemyState[] states = _stateMachineData.states.ToArray();
                ArrayUtility.RemoveAt(ref states, i);
                GUIUtility.ExitGUI(); // Exit GUI to prevent index out of range error
                return;
            }
            
            EditorGUILayout.EndVertical();
        }

        // Add a new state button
        if (GUILayout.Button("Add State"))
        {
            EnemyState[] states = _stateMachineData.states.ToArray();
            ArrayUtility.Add(ref states, EnemyState.CreateInstance());
        }

        // Generate Script button
        if (GUILayout.Button("Generate Script"))
        {
            GenerateScript();
        }

        // Save the changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_stateMachineData);
            AssetDatabase.SaveAssets();
        }
    }

    private void GenerateScript()
    {
        
        foreach (var state in _stateMachineData.states)
        {
            // Generate the script content based on the ScriptableObject
            string scriptContent = GenerateScriptContent();
            
            // Specify the path where the script will be saved
            string scriptPath = "Assets/Scripts/EnemyStateMachine.cs";
            
            // Write the script content to the file
            File.WriteAllText(scriptPath, scriptContent);
            
            // Refresh the asset database to make sure Unity recognizes the new script
            AssetDatabase.Refresh();
            
            Debug.Log($"Script generated at: {scriptPath}");
        }
    }
    
    FieldInfo[] GetPublicVariables(ScriptableObject scriptableObject)
    {
        Type type = scriptableObject.GetType();
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    private string GenerateScriptContent()
    {

        string scriptContent = "using UnityEngine;\n\n";
        scriptContent += "public class EnemyStateMachine : MonoBehaviour\n";
        scriptContent += "{\n";

        List<string> states = new List<string>();   
        foreach (var state in _stateMachineData.states)
        {
            states.Add(state.name);
        }
        
        foreach (var stateName in states.Distinct())
        {
            ScriptableObject actionState = CreateInstance(stateName+"State");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(stateName[0]) + stateName.Substring(1) + "State"};\n\n";
            
            scriptContent += "\t[Header(\""+stateName+" Settings\")]\n\n";
            
            FieldInfo[] fields = GetPublicVariables(actionState);

            // Print the names and values of the public variables
            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(actionState);
                scriptContent += $"\tpublic {value.GetType()} {field.Name +" = "+ value};\n";
            }
        }
        
        List<string> contidions = new List<string>();   
        foreach (var state in _stateMachineData.states)
        {
            contidions.Add(state.condition);
        }

        foreach (var condition in contidions.Distinct())
        {
            ScriptableObject actionState = CreateInstance(condition+"Condition");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(condition[0]) + condition.Substring(1) + "Condition"};\n\n";
        }
        
        scriptContent += "\t\n";
        scriptContent += $"\tpublic void Update()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t}\n\n";
        
        List<string> states2 = new List<string>();

        foreach (var state in _stateMachineData.states)
        {
            if (!states2.Contains(state.name))
            {
                scriptContent += $"\tpublic void {state.name}()\n";
                scriptContent += "\t{\n";
                
                // Generate a call to the SpecificAttackMethod
                string nameCorrection = char.ToLowerInvariant(state.name[0]) + state.name.Substring(1);
                string nameCorrection2 = char.ToLowerInvariant(state.condition[0]) + state.condition.Substring(1) + "Condition";
                            
                scriptContent += $"\t\t{nameCorrection}State.Execute();\n";
                scriptContent += $"\t\tif ("+nameCorrection2+".Condition())\n";
                scriptContent += "\t\t{\n";
                scriptContent += $"\t\t\t{state.nextState}();\n";
                scriptContent += "\t\t}\n";
                scriptContent += "\t}\n\n";
                
                states2.Add(state.name);
            }
            
        }

        scriptContent += "}\n";

        return scriptContent;
    }
}

