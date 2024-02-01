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
        string editorScriptContent = GenerateEditorScriptContent(saveData);

        string scriptPath = $"Assets/EditorWindow/FSMSystem/BehaviorScripts/{saveData.FileName}.cs";
        string editorScriptPath = $"Assets/EditorWindow/FSMSystem/Inspectors/{saveData.FileName}Editor.cs";
        
        File.WriteAllText(scriptPath, scriptContent);
        File.WriteAllText(editorScriptPath, editorScriptContent);
        
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

    private static string GenerateEditorScriptContent(FSMGraphSaveData saveData)
    {
        string scriptContent = "using System.Collections.Generic;\n";
        scriptContent += "using UnityEditor;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using UnityEngine;\n";

        scriptContent += $"[CustomEditor(typeof({saveData.FileName}))]\n";
        scriptContent += $"public class {saveData.FileName}Editor : Editor\n";
        scriptContent += "{\n";
        
        states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            states.Add(state);
        }
        
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\tprivate SerializedProperty {char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}Property;\n";
        }
        
        scriptContent += "\tprivate SerializedProperty optionsProp;\n";
        scriptContent += "\tprivate SerializedProperty selectedOptionIndexProp;\n";
        scriptContent += "\tprivate SerializedProperty currentStateProperty;\n";
        scriptContent += "\tDictionary<string, State> optionToObjectMap = new Dictionary<string, State>();\n";
        
        scriptContent += "\tvoid OnEnable()\n";
        scriptContent += "\t{\n";
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\t\t{char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}Property = serializedObject.FindProperty(\"{char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}\");\n";
        }
        scriptContent += "\t\toptionsProp = serializedObject.FindProperty(\"options\");\n";
        scriptContent += "\t\tselectedOptionIndexProp = serializedObject.FindProperty(\"selectedOptionIndex\");\n";
        scriptContent += "\t\tcurrentStateProperty = serializedObject.FindProperty(\"currentState\");\n";
        
        scriptContent += $"\t\t{saveData.FileName} {char.ToLowerInvariant(saveData.FileName[0]) + saveData.FileName.Substring(1)} = ({saveData.FileName})target;\n";
        scriptContent += $"\t\tfor (int i = 0; i < {saveData.FileName}.options.Count; i++)\n";
        scriptContent += "\t\t{\n";
        scriptContent += $"\t\t\toptionToObjectMap[{saveData.FileName}.options[i].GetStateName()] = {saveData.FileName}.options[i];\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        scriptContent += "\tpublic override void OnInspectorGUI()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tserializedObject.Update();\n";
        scriptContent += $"\t\t{saveData.FileName} {char.ToLowerInvariant(saveData.FileName[0]) + saveData.FileName.Substring(1)} = ({saveData.FileName})target;\n";
        scriptContent += $"\t\tstring[] options = new string[{saveData.FileName}.options.Count];\n";
        scriptContent += "\t\tfor (int i = 0; i < options.Length; i++)\n";
        scriptContent += "\t\t{\n";
        scriptContent += $"\t\t\toptions[i] = {saveData.FileName}.options[i].GetStateName();\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\tselectedOptionIndexProp.intValue = EditorGUILayout.Popup(\"Selected Option\", selectedOptionIndexProp.intValue, options);\n";
        scriptContent += "\t\tstring selectedOptionName = options[selectedOptionIndexProp.intValue];\n";
        scriptContent += "\t\tif (optionToObjectMap.ContainsKey(selectedOptionName))\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tEditorGUILayout.LabelField($\"{selectedOptionName} Attributes:\");\n";
        scriptContent += "\t\t\tScriptableObject selectedObject = optionToObjectMap[selectedOptionName];\n";
        scriptContent += "\t\t\tSerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);\n";
        scriptContent += "\t\t\tselectedObjectSerialized.Update();\n";
        scriptContent += "\t\t\tEditorGUI.BeginChangeCheck();\n";
        scriptContent += "\t\t\tSerializedProperty iterator = selectedObjectSerialized.GetIterator();\n";
        scriptContent += "\t\t\tbool nextVisible = iterator.NextVisible(true);\n";
        scriptContent += "\t\t\twhile (nextVisible)\n";
        scriptContent += "\t\t\t{\n";
        scriptContent += "\t\t\t\tif (iterator.name != \"m_Script\")\n";
        scriptContent += "\t\t\t\t{\n";
        scriptContent += "\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n";
        scriptContent += "\t\t\t\t}\n";
        scriptContent += "\t\t\t\tnextVisible = iterator.NextVisible(false);\n";
        scriptContent += "\t\t\t}\n";
        scriptContent += "\t\t\tif (EditorGUI.EndChangeCheck())\n";
        scriptContent += "\t\t\t{\n";
        scriptContent += "\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
        scriptContent += "\t\t\t}\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\tserializedObject.ApplyModifiedProperties();\n";
        scriptContent += "\t}\n";

        scriptContent += "}\n";
        return scriptContent;
    }
    
    #region Utilities
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
    #endregion
}


