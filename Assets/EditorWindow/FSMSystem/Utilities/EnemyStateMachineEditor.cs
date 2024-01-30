using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class EnemyStateMachineEditor
{
    private static List<FSMNodeSaveData> states;
    public static void GenerateScript(FSMGraphSaveData saveData)
    {
        string scriptContent = GenerateScriptContent(saveData);

        string scriptPath = $"Assets/EditorWindow/FSMSystem/BehaviorScripts/{saveData.FileName}.cs";
        
        File.WriteAllText(scriptPath, scriptContent);
        
        AssetDatabase.Refresh();
        
        CreateWindow window = ScriptableObject.CreateInstance<CreateWindow>();
        window.Initialize(saveData.FileName, saveData);
    }

    private static string GenerateScriptContent(FSMGraphSaveData saveData)
    {

        string scriptContent = "using UnityEngine;\n";
        scriptContent += "using System;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using System.Collections.Generic;\n";
        scriptContent += "using System.Reflection;\n\n";
        
        scriptContent += "[Serializable]\n";
        scriptContent += $"public class {saveData.FileName} : MonoBehaviour\n";
        scriptContent += "{\n";

        states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            states.Add(state);
        }
        
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\t[Header(\"" + node.Name + "\")]\n";
            scriptContent += "\t[SerializeField]\n";
            string variableName = node.NodeType == FSMNodeType.State ? node.Name+"State" : node.Name+"Condition";
            scriptContent += $"\tpublic {variableName} {char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)};\n";
            scriptContent += "\n";
        }
        
        foreach (var node in states.Distinct())
        {
            if (node.NodeType == FSMNodeType.State)
            {
                scriptContent += $"\tpublic void {node.Name}()\n";
                scriptContent += "\t{\n";
                scriptContent += $"\t\t{char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}.Execute();\n";
                
                string name = GetState(node.Connections[0].NodeId);
                string connectionName = GetConnection(node.Connections[0].NodeId);
                
                scriptContent += $"\t\tif({char.ToLowerInvariant(name[0]) + name.Substring(1)}.Condition())\n";
                scriptContent += "\t\t{\n";  
                scriptContent += $"\t\t\t{char.ToLowerInvariant(connectionName[0]) + connectionName.Substring(1)}.Execute();\n";
                scriptContent += "\t\t}\n";
                scriptContent += "\t}\n";
            }
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
    
    private static string GetState(string id)
    {
        foreach (var state in states)
        {
            if (state.Id == id)
            {
                return state.Name;
            }
        }
        return null;
    }

    private static string GetConnection(string id)
    {
        foreach (var state in states)
        {
            if (state.Id == id)
            {
                return GetState(state.Connections[0].NodeId);
            }
        }
        return null;
    }
}

