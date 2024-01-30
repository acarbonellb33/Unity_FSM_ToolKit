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
        scriptContent += $"public class {saveData.FileName} : BehaviorScript\n";
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

        scriptContent += "\tprivate FSMStates currentState;\n";
        scriptContent += "\n";
        scriptContent += "\tvoid Update()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tswitch (currentState)\n";
        scriptContent += "\t\t{\n";

        foreach (var node in states.Distinct())
        {
            if (node.NodeType == FSMNodeType.State)
            {
                scriptContent += $"\t\t\tcase FSMStates.{node.Name}:\n";
                scriptContent += $"\t\t\t\tUpdate{node.Name}State();\n";
                scriptContent += "\t\t\t\tbreak;\n";
            }
        }
        
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        foreach (var node in states.Distinct())
        {
            if (node.NodeType == FSMNodeType.State)
            {
                scriptContent += $"\tpublic void Update{node.Name}State()\n";
                scriptContent += "\t{\n";
                scriptContent += $"\t\t{char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}.Execute();\n";
                
                string name = GetState(node.Connections[0].NodeId);
                string connectionName = GetConnection(node.Connections[0].NodeId);
                
                scriptContent += $"\t\tif({char.ToLowerInvariant(name[0]) + name.Substring(1)}.Condition())\n";
                scriptContent += "\t\t{\n";  
                scriptContent += $"\t\t\tChange{node.Name}State();\n";
                scriptContent += "\t\t}\n";
                scriptContent += "\t}\n";
            }
        }
        
        foreach (var node in states.Distinct())
        {
            if (node.NodeType == FSMNodeType.State)
            {
                scriptContent += $"\tprivate void Change{node.Name}State()\n";
                scriptContent += "\t{\n";
                scriptContent += $"\t\tcurrentState = FSMStates.{node.Name};\n";
                scriptContent += "\t}\n";
            }
        }

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

