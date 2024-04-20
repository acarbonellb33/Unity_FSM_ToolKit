using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EnemyStateMachineEditor
{
    private static List<FSMNodeSaveData> states;
    private static bool hasPatrolState = false;
    public static void GenerateScript(FSMGraphSaveData saveData)
    {
        string scriptContent = GenerateScriptContent(saveData);
        string editorScriptContent = GenerateEditorScriptContent(saveData);
        
        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");

        string scriptPath = $"Assets/EditorWindow/FSMSystem/BehaviorScripts/{stringWithoutSpaces}.cs";
        string editorScriptPath = $"Assets/EditorWindow/FSMSystem/Inspectors/{stringWithoutSpaces}Editor.cs";
        
        File.WriteAllText(scriptPath, scriptContent);
        File.WriteAllText(editorScriptPath, editorScriptContent);
        
        AssetDatabase.Refresh();
    }

    private static string GenerateScriptContent(FSMGraphSaveData saveData)
    {

        string scriptContent = "using UnityEngine;\n";
        scriptContent += "using System;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using System.Collections.Generic;\n";
        scriptContent += "using UnityEngine.AI;\n";
        scriptContent += "using System.Reflection;\n\n";

        scriptContent += "[Serializable]\n";
        
        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");
        
        scriptContent += $"public class {stringWithoutSpaces} : BehaviorScript\n";
        scriptContent += "{\n";

        states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            if (state.Name == "Patrol")
            {
                hasPatrolState = true;
            }
            if(state.NodeType != FSMNodeType.Initial)states.Add(state);
        }
        
        foreach (var node in states.Distinct())
        {
            scriptContent += $"\t[Header(\"" + node.Name + "\")]\n";
            scriptContent += "\t[SerializeField]\n";
            string variableName = node.NodeType == FSMNodeType.State ? node.Name+"State" : node.Name+"Condition";
            string name = char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1);
            name = name.Replace(" ", "");
            variableName = Regex.Replace(variableName, @"[\s\d]", "");
            scriptContent += $"\tpublic {variableName}Script {name};\n";
            scriptContent += "\n";
        }
        
        scriptContent += "\tprivate void Start()\n";
        scriptContent += "\t{\n";
        scriptContent += $"\t\tcurrentState = FSMStates.{saveData.InitialState};\n";
        scriptContent += "\t\tGetComponent<Animator>().SetFloat(\"Speed\", GetComponent<NavMeshAgent>().speed);\n";
        scriptContent += "\t}\n";

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
                
                string conditions = "";
                
                FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[0].NodeId));
                bool check = nodeSaveData.Connections.Count == 2;
                
                scriptContent += GenerateConditionsRecursive(nodeSaveData, conditions, true, check, "");
                scriptContent += "\t}\n";
            }
        }
        
        foreach (var node in states.Distinct())
        {
            if (node.NodeType == FSMNodeType.State)
            {
                scriptContent += $"\tprivate void Change{node.Name.Replace(" ","")}State()\n";
                scriptContent += "\t{\n";
                scriptContent += $"\t\tcurrentState = FSMStates.{node.Name};\n";
                scriptContent += "\t}\n";
            }
        }

        if (hasPatrolState)
        {
            scriptContent += "\tpublic GameObject AddObjectToList()\n";
            scriptContent += "\t{\n";
            scriptContent += "\t\tGameObject newGameObject = new GameObject(\"Patrol Point \" + patrol.patrolPoints.Count);\n";
            scriptContent += "\t\tpatrol.patrolPoints.Add(newGameObject);\n";
            scriptContent += "\t\treturn newGameObject;\n";
            scriptContent += "\t}\n";
 
            scriptContent += "\tpublic void RemoveObjectFromList(GameObject patrolPoint)\n";
            scriptContent += "\t{\n";
            scriptContent += "\t\tpatrol.RemovePatrolPoint(patrolPoint);\n";
            scriptContent += "\t\tif(GameObject.Find(patrolPoint.name) != null)\n";
            scriptContent += "\t\t{\n";
            scriptContent += "\t\t\tDestroyImmediate(patrolPoint);\n";
            scriptContent += "\t\t}\n";
            scriptContent += "\t}\n";
        }
        
        scriptContent += "\tprivate void OnFootstep() {}\n";

        scriptContent += "}\n";
        return scriptContent;
    }

    private static string GenerateConditionsRecursive(FSMNodeSaveData node, string test, bool isFirst, bool isElse, string pastFalse)
    {
        if(node.NodeType == FSMNodeType.Initial || node.NodeType == FSMNodeType.State)
        {
            test += ")\n";
            test += "\t\t{\n";
            test += $"\t\t\tChange{node.Name.Replace(" ", "")}State();\n";
            test += "\t\t}\n";
            return test;
        }

        string name = node.Name;
        string conditionName = char.ToLowerInvariant(name[0]) + name.Substring(1);
        conditionName = conditionName.Replace(" ", "");
        
        if(isFirst)test += $"\t\tif({conditionName}.Condition()";
        else test += $" && {conditionName}.Condition()";
        
        if (isElse)
        {
            FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[1].NodeId));
            bool check1 = nodeSaveData.Connections.Count == 2;

            FSMNodeSaveData nodeSaveData2 = GetNodeData(GetState(node.Connections[0].NodeId));
            bool check2 = nodeSaveData2.Connections.Count == 2;
            
            if(pastFalse == "")pastFalse = $"\t\telse if(!{conditionName}.Condition()";
            else pastFalse += $" && !{conditionName}.Condition()";

            return GenerateConditionsRecursive(nodeSaveData2, test, false, check2, null) +
                   GenerateConditionsRecursive(nodeSaveData, pastFalse, false, check1, pastFalse);
        }
        else
        {
            FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[0].NodeId));
            bool check = nodeSaveData.Connections.Count == 2;
            return GenerateConditionsRecursive(nodeSaveData, test, false, check, null);
        }
    }

    private static string GenerateEditorScriptContent(FSMGraphSaveData saveData)
    {
        string scriptContent = "using System;\n";
        scriptContent += "using System.Collections.Generic;\n";
        scriptContent += "using UnityEditor;\n";
        scriptContent += "using System.Reflection;\n";
        scriptContent += "using UnityEngine;\n\n";

        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");
    
        scriptContent += $"[CustomEditor(typeof({stringWithoutSpaces}))]\n";
        scriptContent += $"public class {stringWithoutSpaces}Editor : Editor\n";
        scriptContent += "{\n";
        
        states = new List<FSMNodeSaveData>();   
        foreach (FSMNodeSaveData state in saveData.Nodes)
        {
            states.Add(state);
        }
        
        string nameLowerCapital = char.ToLowerInvariant(stringWithoutSpaces[0]) + stringWithoutSpaces.Substring(1);
        
        scriptContent += "\tprivate SerializedProperty selectedOptionIndexProp;\n";
        scriptContent += "\tDictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();\n";
        
        scriptContent += "\tvoid OnEnable()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tselectedOptionIndexProp = serializedObject.FindProperty(\"selectedOptionIndex\");\n";
        scriptContent += $"\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
        scriptContent += $"\t\tType type = typeof({nameLowerCapital});\n";
        scriptContent += "\t\tFieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);\n";
        scriptContent += "\t\tint j = 0;\n";
        scriptContent += "\t\tforeach (FieldInfo field in fields)\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tstring nameField = FixName(field.Name);\n";
        scriptContent += $"\t\t\toptionToObjectMap[nameField] = {nameLowerCapital}.options[j];\n";
        scriptContent += "\t\t\tj++;\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        scriptContent += "\tpublic override void OnInspectorGUI()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tserializedObject.Update();\n";
        scriptContent += $"\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
        scriptContent += $"\t\tstring[] options = new string[{nameLowerCapital}.options.Count];\n";
        scriptContent += $"\t\tType type = typeof({nameLowerCapital});\n";
        scriptContent += "\t\tFieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);\n";
        scriptContent += "\t\tint x = 0;\n";
        scriptContent += "\t\tforeach (FieldInfo field in fields) \n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tstring nameField = FixName(field.Name);\n";
        scriptContent += "\t\t\toptions[x] = nameField;\n";
        scriptContent += "\t\t\tx++;\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\tselectedOptionIndexProp.intValue = EditorGUILayout.Popup(\"Selected Option\", selectedOptionIndexProp.intValue, options);\n";
        scriptContent += "\t\tstring selectedOptionName = options[selectedOptionIndexProp.intValue];\n";
        scriptContent += "\t\tif (optionToObjectMap.ContainsKey(selectedOptionName))\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tEditorGUILayout.LabelField($\"{selectedOptionName} Attributes:\");\n";
        scriptContent += "\t\t\tStateScript selectedObject = optionToObjectMap[selectedOptionName];\n";
        scriptContent += "\t\t\tSerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);\n";
        scriptContent += "\t\t\tselectedObjectSerialized.Update();\n";
        scriptContent += "\t\t\tEditorGUI.BeginChangeCheck();\n";
        scriptContent += "\t\t\tSerializedProperty iterator = selectedObjectSerialized.GetIterator();\n";
        scriptContent += "\t\t\tbool nextVisible = iterator.NextVisible(true);\n";
        scriptContent += "\t\t\twhile (nextVisible)\n";
        scriptContent += "\t\t\t{\n";
        scriptContent += "\t\t\t\tif (iterator.name != \"m_Script\")\n";
        scriptContent += "\t\t\t\t{\n";
        if (hasPatrolState)
        {
            scriptContent += "\t\t\t\t\tif (iterator.isArray)\n";
            scriptContent += "\t\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\t\tEditorGUILayout.Space();\n";
            scriptContent += "\t\t\t\t\t\tEditorGUILayout.LabelField(\"Create a Patrol Waypoint\", EditorStyles.boldLabel);\n";
            scriptContent += "\t\t\t\t\t\tEditorGUILayout.Space();\n";
            scriptContent += "\t\t\t\t\t\tfor (int i = 0; i < iterator.arraySize; i++)\n";
            scriptContent += "\t\t\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.BeginHorizontal();\n";
            scriptContent += "\t\t\t\t\t\t\tSerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);\n";
            scriptContent += "\t\t\t\t\t\t\tif (gameObjectElementProperty.objectReferenceValue != null)\n";
            scriptContent += "\t\t\t\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\t\t\t\tGameObject gameObject = (GameObject)gameObjectElementProperty.objectReferenceValue;\n";
            scriptContent += "\t\t\t\t\t\t\t\tEditorGUILayout.PropertyField(gameObjectElementProperty, GUIContent.none);\n"; 
            scriptContent += "\t\t\t\t\t\t\t\tif (GUILayout.Button(\"Remove\", GUILayout.Width(70)))\n"; 
            scriptContent += "\t\t\t\t\t\t\t\t{\n"; 
            scriptContent += "\t\t\t\t\t\t\t\t\tRemovePatrolPoint(gameObject);\n"; 
            scriptContent += "\t\t\t\t\t\t\t\t}\n"; 
            scriptContent += $"\t\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n"; 
            scriptContent += "\t\t\t\t\t\t\t}\n"; 
            scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.EndHorizontal();\n"; 
            scriptContent += "\t\t\t\t\t\t}\n"; 
            scriptContent += "\t\t\t\t\t\tif (GUILayout.Button(\"Create and Add a Patrol Point\"))\n"; 
            scriptContent += "\t\t\t\t\t\t{\n"; 
            scriptContent += $"\t\t\t\t\t\t\tCreateAndAddGameObject({nameLowerCapital});\n"; 
            scriptContent += "\t\t\t\t\t\t}\n"; 
            scriptContent += "\t\t\t\t\t}\n"; 
            scriptContent += "\t\t\t\t\telse\n"; 
            scriptContent += "\t\t\t\t\t{\n"; 
            scriptContent += "\t\t\t\t\t\tEditorGUI.BeginChangeCheck();\n"; 
            scriptContent += "\t\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n"; 
            scriptContent += "\t\t\t\t\t\tif (EditorGUI.EndChangeCheck())\n"; 
            scriptContent += "\t\t\t\t\t\t{\n"; 
            scriptContent += "\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n"; 
            scriptContent += $"\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n"; 
            scriptContent += "\t\t\t\t\t\t}\n"; 
            scriptContent += "\t\t\t\t\t}\n";
        }
        else
        {
            scriptContent += "\t\t\t\t\tEditorGUI.BeginChangeCheck();\n"; 
            scriptContent += "\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n"; 
            scriptContent += "\t\t\t\t\tif (EditorGUI.EndChangeCheck())\n"; 
            scriptContent += "\t\t\t\t\t{\n"; 
            scriptContent += "\t\t\t\t\t\tselectedObject.SetStateName(selectedOptionName);\n";
            scriptContent += "\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n"; 
            scriptContent += $"\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n"; 
            scriptContent += "\t\t\t\t\t}\n"; 
        }
        
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
       
        if (hasPatrolState)
        {
            scriptContent += $"\tprivate void CreateAndAddGameObject({stringWithoutSpaces} {nameLowerCapital})\n";
            scriptContent += "\t{\n";
            scriptContent += $"\t\t{nameLowerCapital}.AddObjectToList();\n";
            scriptContent += "\t}\n";
            scriptContent += "\tprivate void RemovePatrolPoint(GameObject patrolPoint)\n";
            scriptContent += "\t{\n";
            scriptContent += $"\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
            scriptContent += $"\t\t{nameLowerCapital}.patrol.RemovePatrolPoint(patrolPoint);\n";
            scriptContent += "\t\tif(GameObject.Find(patrolPoint.name) != null)\n";
            scriptContent += "\t\t{\n";
            scriptContent += "\t\t\tDestroyImmediate(patrolPoint);\n";
            scriptContent += "\t\t}\n";
            scriptContent += "\t}\n";
        }
        
        scriptContent += "\tprivate string FixName(string oldName)\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\treturn char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);\n";
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

    private static FSMNodeSaveData GetNodeData(string name)
    {
        foreach (var state in states)
        {
            if (state.Name == name)
            {
                return state;
            }
        }
        return null;
    }
    
    #endregion
}


