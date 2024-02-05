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
        
        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");

        string scriptPath = $"Assets/EditorWindow/FSMSystem/BehaviorScripts/{stringWithoutSpaces}.cs";
        string editorScriptPath = $"Assets/EditorWindow/FSMSystem/Inspectors/{stringWithoutSpaces}Editor.cs";
        
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
        
        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");
        
        scriptContent += $"public class {stringWithoutSpaces} : BehaviorScript\n";
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
        
        scriptContent += "\tprivate void Start()\n";
        scriptContent += "\t{\n";
        scriptContent += $"\t\tcurrentState = FSMStates.{saveData.InitialState};\n";
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
                
                string name = GetState(node.Connections[0].NodeId);
                string connectionName = GetConnection(node.Connections[0].NodeId);
                
                scriptContent += $"\t\tif({char.ToLowerInvariant(name[0]) + name.Substring(1)}.Condition())\n";
                scriptContent += "\t\t{\n";  
                scriptContent += $"\t\t\tChange{connectionName}State();\n";
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
        scriptContent += "\tpublic void AddObjectToList(GameObject obj)\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tpatrol.patrolPoints.Add(obj);\n";
        scriptContent += "\t}\n";

        scriptContent += "}\n";

        return scriptContent;
    }

    private static string GenerateEditorScriptContent(FSMGraphSaveData saveData)
    {
        string scriptContent = "using System.Collections.Generic;\n";
        scriptContent += "using UnityEditor;\n";
        scriptContent += "using System.Collections;\n";
        scriptContent += "using UnityEngine;\n";

        string stringWithSpaces = saveData.FileName;
        string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");
        Debug.Log(stringWithoutSpaces);
        
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
        scriptContent += "\tDictionary<string, State> optionToObjectMap = new Dictionary<string, State>();\n";
        
        scriptContent += "\tvoid OnEnable()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tselectedOptionIndexProp = serializedObject.FindProperty(\"selectedOptionIndex\");\n";
        scriptContent += $"\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
        scriptContent += $"\t\tfor (int i = 0; i < {nameLowerCapital}.options.Count; i++)\n";
        scriptContent += "\t\t{\n";
        scriptContent += $"\t\t\toptionToObjectMap[{nameLowerCapital}.options[i].GetStateName()] = {nameLowerCapital}.options[i];\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t}\n";
        
        scriptContent += "\tpublic override void OnInspectorGUI()\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tserializedObject.Update();\n";
        scriptContent += $"\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
        scriptContent += $"\t\tstring[] options = new string[{nameLowerCapital}.options.Count];\n";
        scriptContent += "\t\tfor (int i = 0; i < options.Length; i++)\n";
        scriptContent += "\t\t{\n";
        scriptContent += $"\t\t\toptions[i] = {nameLowerCapital}.options[i].GetStateName();\n";
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
        scriptContent += "\t\t\t\t\t\t\t\tVector3 position = gameObject.transform.position;\n";
        scriptContent += "\t\t\t\t\t\t\t\tEditorGUILayout.LabelField(\"Position \" + i, \"X: \" + position.x + \"\\tY: \" + position.y + \"\\tZ: \" + position.z);\n";
        scriptContent += "\t\t\t\t\t\t\t\tif (GUILayout.Button(\"Remove\", GUILayout.Width(70)))\n";
        scriptContent += "\t\t\t\t\t\t\t\t{\n";
        scriptContent += "\t\t\t\t\t\t\t\t\tRemovePatrolPoint(gameObject);\n";
        scriptContent += "\t\t\t\t\t\t\t\t}\n";
        scriptContent += "\t\t\t\t\t\t\t}\n";
        scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.EndHorizontal();\n";
        scriptContent += "\t\t\t\t\t\t}\n";
        scriptContent += "\t\t\t\t\t\tif (GUILayout.Button(\"Create and Add a Patrol Point\"))\n";
        scriptContent += "\t\t\t\t\t\t{\n";
        scriptContent += $"\t\t\t\t\t\t\tCreateAndAddGameObject({nameLowerCapital}, iterator.arraySize);\n";
        scriptContent += "\t\t\t\t\t\t}\n";
        scriptContent += "\t\t\t\t\t}\n";
        scriptContent += "\t\t\t\t\telse\n";
        scriptContent += "\t\t\t\t\t{\n";
        scriptContent += "\t\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n";
        scriptContent += "\t\t\t\t\t}\n";
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
        scriptContent += $"\tprivate void CreateAndAddGameObject({stringWithoutSpaces} {nameLowerCapital}, int count)\n";
        scriptContent += "\t{\n";
        scriptContent += "\t\tGameObject newGameObject = new GameObject(\"Patrol Point \" + count);\n";
        scriptContent += "\t\tstring prefabPath = \"Assets/Prefabs/PatrolPoint\" + count + \".prefab\";\n";
        scriptContent += "\t\tPrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);\n";
        scriptContent += "\t\tGameObject prefabToList = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);\n";
        scriptContent += "\t\tGameObject prefab = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath)) as GameObject;\n";
        scriptContent += "\t\tDestroyImmediate(newGameObject);\n";
        scriptContent += "\t\tif (prefabToList != null)\n";
        scriptContent += "\t\t{\n";
        scriptContent += $"\t\t\t{nameLowerCapital}.AddObjectToList(prefabToList);\n";
        scriptContent += "\t\t}\n";
        scriptContent += "\t\telse\n";
        scriptContent += "\t\t{\n";
        scriptContent += "\t\t\tDebug.LogError(\"Failed to create a new GameObject\");\n";
        scriptContent += "\t\t}\n";
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


