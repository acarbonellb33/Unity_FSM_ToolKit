using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public static class EnemyStateMachineEditor
{
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

    private static EnemyStateMachineData _stateMachineData;

    public static void Initialize()
    {
        if (_stateMachineData == null)
        {
            // Load or create the data asset
            _stateMachineData = AssetDatabase.LoadAssetAtPath<EnemyStateMachineData>("Assets/Editor/EnemyStateMachineData.asset");

            if (_stateMachineData == null)
            {
                //_stateMachineData = CreateInstance<EnemyStateMachineData>();
                _stateMachineData.AddState("PatrolState");
                _stateMachineData.AddState("AttackState");
                _stateMachineData.AddProximityCondition("HearingCondition");
                AssetDatabase.CreateAsset(_stateMachineData, "Assets/Editor/EnemyStateMachineData.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }

    public static void GenerateScript(FSMGraphSaveData saveData, GameObject gameObject)
    {
        string scriptContent = GenerateScriptContent(saveData);

        string scriptPath = "Assets/Scripts/EnemyStateMachine.cs";
        
        File.WriteAllText(scriptPath, scriptContent);
        
        AssetDatabase.Refresh();
        
        AssignScriptableObjectReferences(gameObject, saveData);
            
        Debug.Log($"Script generated at: {scriptPath}");
    }
    
    static FieldInfo[] GetPublicVariables(ScriptableObject scriptableObject)
    {
        Type type = scriptableObject.GetType();
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    private static void AssignScriptableObjectReferences(GameObject gameObject, FSMGraphSaveData saveData)
    {
        EnemyStateMachine enemyStateMachine = gameObject.GetComponent<EnemyStateMachine>();
        for (int i = 0; i < saveData.Nodes.Count; i++)
        {
            enemyStateMachine.SetVariableValue(char.ToLowerInvariant(saveData.Nodes[i].Name[0]) + saveData.Nodes[i].Name.Substring(1), saveData.Nodes[i].ScriptableObject);
        }
    }

    private static string GenerateScriptContent(FSMGraphSaveData saveData)
    {

        string scriptContent = "using UnityEngine;\n";
        scriptContent += "using System;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using System.Collections.Generic;\n";
        scriptContent += "using System.Reflection;\n\n";
        

        scriptContent += "public class EnemyStateMachine : MonoBehaviour\n";
        scriptContent += "{\n";

        List<FSMNodeSaveData> states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            states.Add(state);
        }
        
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\t[Header(\"" + node.Name + "\")]\n";
            foreach (string fullAttribute in node.ScriptableObject.InspectVariables())
            {
                /*string[] result = fullAttribute.Split(',');
                scriptContent += "\t[SerializeField]\n";
                scriptContent += $"\tprivate {result[1]} {result[0]} = {char.ToLowerInvariant(result[2][0]) + result[2].Substring(1)};\n";*/
            }
            
            scriptContent += "\t[SerializeField]\n";
            string variableName = node.DialogueType == FSMDialogueType.State ? node.Name+"State" : node.Name+"Condition";
            scriptContent += $"\tpublic {variableName} {char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)};\n";

            scriptContent += "\n";


            /*ScriptableObject actionState = CreateInstance(node.Name+"State");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(stateName[0]) + stateName.Substring(1) + "State"};\n\n";
            
            scriptContent += "\t[Header(\""+stateName+" Settings\")]\n\n";
            
            FieldInfo[] fields = GetPublicVariables(actionState);

            // Print the names and values of the public variables
            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(actionState);
                scriptContent += $"\tpublic {value.GetType()} {field.Name +" = "+ value};\n";
            }*/
        }
        
        scriptContent += $"\tpublic Dictionary<string, object> GetVariables()";
        scriptContent += "\n\t{\n";
        scriptContent += "\t\tDictionary<string, object> variables = new Dictionary<string, object>();\n";
        scriptContent += $"\t\tType type = GetType();\n";
        scriptContent += $"\t\tFieldInfo[] fields = type.GetFields();\n";
        scriptContent += "\n";
        scriptContent += "\t\tforeach (FieldInfo field in fields)\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tobject value = field.GetValue(this);\n";
        scriptContent += "\t\t\tvariables.Add(field.Name, value);\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\treturn variables;\n";
        scriptContent += "\t}\n";
            
        scriptContent += "\n";
            
        scriptContent += $"\tpublic void SetVariableValue(string variableName, object newValue)\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\t// Use reflection to set the value of the variable with the given name\n";
        scriptContent += "\t\tSystem.Type type = GetType();\n";
        scriptContent += "\t\tSystem.Reflection.FieldInfo field = type.GetField(variableName);\n";
        scriptContent += "\n";
        scriptContent += "\t\tif (field != null)\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tfield.SetValue(this, newValue);\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\telse\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tDebug.LogError($\"{variableName} does not exist in the ScriptableObject.\");\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        /*List<string> contidions = new List<string>();   
        foreach (var state in _stateMachineData.states)
        {
            contidions.Add(state.condition);
        }

        foreach (var condition in contidions.Distinct())
        {
            ScriptableObject actionState = CreateInstance(condition+"Condition");
            scriptContent += $"\n\tprivate {actionState.GetType()} {char.ToLowerInvariant(condition[0]) + condition.Substring(1) + "Condition"};\n\n";
        }*/
        
        /*scriptContent += "\t\n";
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
            
        }*/

        scriptContent += "}\n";

        return scriptContent;
    }
}

