#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Data.Save;
    using UnityEditor;
    using FSM.Enumerations;
    public static class EnemyStateMachineEditor
    {
        private static List<FSMNodeSaveData> _states;
        private static bool _hasPatrolState = false;

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
            string scriptContent = "#if UNITY_EDITOR\n";
            scriptContent += "namespace EditorWindow.FSMSystem.BehaviorScripts\n";
            scriptContent += "{\n";
            scriptContent += "\tusing UnityEngine;\n";
            scriptContent += "\tusing System;\n";
            scriptContent += "\tusing UnityEngine.AI;\n";
            scriptContent += "\tusing Utilities;\n";
            scriptContent += "\tusing FSM.Enemy;\n";
            scriptContent += "\tusing FSM.Enumerations;\n";
            scriptContent += "\tusing FSM.Nodes.States.StateScripts;\n";
            scriptContent += "\tusing FSM.Utilities;\n\n";

            scriptContent += "\t[Serializable]\n";

            string stringWithSpaces = saveData.FileName;
            string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");

            scriptContent += $"\tpublic class {stringWithoutSpaces} : BehaviorScript\n";
            scriptContent += "\t{\n";

            _states = new List<FSMNodeSaveData>();
            foreach (FSMNodeSaveData state in saveData.Nodes)
            {
                if (state.Name == "Patrol")
                {
                    _hasPatrolState = true;
                }

                if (state.NodeType != FSMNodeType.Initial) _states.Add(state);
            }

            foreach (var node in _states.Distinct())
            {
                if (node.NodeType != FSMNodeType.Extension)
                {
                    scriptContent += $"\t\t[Header(\"" + node.Name + "\")]\n";
                    scriptContent += "\t\t[SerializeField]\n";
                    string variableName = node.NodeType == FSMNodeType.State || node.NodeType == FSMNodeType.CustomState
                        ? node.Name + "State"
                        : node.Name + "Condition";
                    string name = char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1);
                    name = name.Replace(" ", "");
                    variableName = Regex.Replace(variableName, @"[\s\d]", "");
                    scriptContent += $"\t\tpublic {variableName}Script {name};\n";
                    scriptContent += "\n";
                }
            }

            if (saveData.HitData.HitEnable)
            {
                scriptContent += $"\t\tfloat waitHitTime = {saveData.HitData.TimeToWait}f;\n";
                scriptContent += "\t\tfloat hitLastTime = 0f;\n";
            }

            scriptContent += "\t\tprivate void Start()\n";
            scriptContent += "\t\t{\n";
            scriptContent += $"\t\t\tcurrentState = FSMStates.{saveData.InitialState};\n";
            scriptContent +=
                "\t\t\tGetComponent<Animator>().SetFloat(\"Speed\", GetComponent<NavMeshAgent>().speed);\n";
            scriptContent += "\t\t}\n";

            scriptContent += "\t\tvoid Update()\n";
            scriptContent += "\t\t{\n";
            scriptContent += "\t\t\tswitch (currentState)\n";
            scriptContent += "\t\t\t{\n";

            foreach (var node in _states.Distinct())
            {
                if (node.NodeType == FSMNodeType.State || node.NodeType == FSMNodeType.CustomState)
                {
                    scriptContent += $"\t\t\t\tcase FSMStates.{node.Name}:\n";
                    scriptContent += $"\t\t\t\t\tUpdate{node.Name}State();\n";
                    scriptContent += "\t\t\t\t\tbreak;\n";
                }
            }

            if (saveData.HitData.HitEnable)
            {
                scriptContent += "\t\t\t\tcase FSMStates.Hit:\n";
                scriptContent += "\t\t\t\t\tUpdateHitState();\n";
                scriptContent += "\t\t\t\t\tbreak;\n";
                if (saveData.HitData.CanDie)
                {
                    scriptContent += "\t\t\t\tcase FSMStates.Die:\n";
                    scriptContent += "\t\t\t\t\tUpdateDieState();\n";
                    scriptContent += "\t\t\t\t\tbreak;\n";
                }

                scriptContent += "\t\t\t}\n";
                scriptContent += "\t\t\tEnemyHealthSystem healthSystem = GetComponent<EnemyHealthSystem>();\n";
                scriptContent += "\t\t\tif(healthSystem.GetCurrentHealth() < healthSystem.GetPreviousHealth())\n";
                scriptContent += "\t\t\t{\n";
                scriptContent += "\t\t\t\tChangeHitState();\n";
                scriptContent += "\t\t\t\thealthSystem.SetPreviousHealth(healthSystem.GetCurrentHealth());\n";
                scriptContent += "\t\t\t}\n";
                if (saveData.HitData.CanDie)
                {
                    scriptContent += "\t\t\tif(healthSystem.GetCurrentHealth() <= 0)\n";
                    scriptContent += "\t\t\t{\n";
                    scriptContent += "\t\t\t\tChangeDieState();\n";
                    scriptContent += "\t\t\t}\n";
                }
            }
            else scriptContent += "\t\t\t}\n";

            scriptContent += "\t\t}\n";

            foreach (var node in _states.Distinct())
            {
                if (node.NodeType == FSMNodeType.State || node.NodeType == FSMNodeType.CustomState)
                {
                    scriptContent += $"\t\tpublic void Update{node.Name}State()\n";
                    scriptContent += "\t\t{\n";
                    scriptContent +=
                        $"\t\t\t{char.ToLowerInvariant(node.Name[0]) + node.Name.Substring(1)}.Execute();\n";
                    if (node.AnimatorSaveData.TriggerEnable)
                    {
                        switch (node.AnimatorSaveData.ParameterType)
                        {
                            case "Float":
                                scriptContent +=
                                    $"\t\t\tGetComponent<Animator>().SetFloat(\"{node.AnimatorSaveData.ParameterName}\", {node.AnimatorSaveData.Value});\n";
                                break;
                            case "Int":
                                scriptContent +=
                                    $"\t\t\tGetComponent<Animator>().SetInteger(\"{node.AnimatorSaveData.ParameterName}\", {node.AnimatorSaveData.Value});\n";
                                break;
                            case "Bool":
                                scriptContent +=
                                    $"\t\t\tGetComponent<Animator>().SetBool(\"{node.AnimatorSaveData.ParameterName}\", {char.ToLowerInvariant(node.AnimatorSaveData.Value[0]) + node.AnimatorSaveData.Value.Substring(1)});\n";
                                break;
                            case "Trigger":
                                scriptContent +=
                                    $"\t\t\tGetComponent<Animator>().SetTrigger(\"{node.AnimatorSaveData.ParameterName}\");\n";
                                break;
                        }
                    }

                    string conditions = "";

                    FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[0].NodeId));
                    bool check = nodeSaveData.Connections.Count == 2;

                    scriptContent += GenerateConditionsRecursive(nodeSaveData, conditions, true, check, "");
                    scriptContent += "\t\t}\n";
                }
            }

            if (saveData.HitData.HitEnable)
            {
                scriptContent += $"\t\tpublic void UpdateHitState()\n";
                scriptContent += "\t\t{\n";
                scriptContent += "\t\t\tNavMeshAgent agent = GetComponent<NavMeshAgent>();\n";
                scriptContent += "\t\t\tagent.isStopped = true;\n";
                scriptContent += "\t\t\tif(Time.time >= hitLastTime + waitHitTime)\n";
                scriptContent += "\t\t\t{\n";
                scriptContent += $"\t\t\t\tcurrentState = FSMStates.{saveData.InitialState};\n";
                scriptContent += "\t\t\t\tagent.isStopped = false;\n";
                scriptContent += "\t\t\t}\n";
                scriptContent += "\t\t}\n";
                if (saveData.HitData.CanDie)
                {
                    scriptContent += $"\t\tpublic void UpdateDieState()\n";
                    scriptContent += "\t\t{\n";
                    scriptContent += "\t\t\tGetComponent<EnemyHealthSystem>().Die();\n";
                    scriptContent += "\t\t}\n";
                }
            }


            foreach (var node in _states.Distinct())
            {
                if (node.NodeType == FSMNodeType.State || node.NodeType == FSMNodeType.CustomState)
                {
                    scriptContent += $"\t\tprivate void Change{node.Name.Replace(" ", "")}State()\n";
                    scriptContent += "\t\t{\n";
                    scriptContent += $"\t\t\tcurrentState = FSMStates.{node.Name};\n";
                    scriptContent += "\t\t}\n";
                }
            }

            if (saveData.HitData.HitEnable)
            {
                scriptContent += $"\t\tprivate void ChangeHitState()\n";
                scriptContent += "\t\t{\n";
                scriptContent += $"\t\t\tcurrentState = FSMStates.Hit;\n";
                scriptContent += "\t\t}\n";
                if (saveData.HitData.CanDie)
                {
                    scriptContent += $"\t\tprivate void ChangeDieState()\n";
                    scriptContent += "\t\t{\n";
                    scriptContent += $"\t\t\tcurrentState = FSMStates.Die;\n";
                    scriptContent += "\t\t}\n";
                }
            }

            if (_hasPatrolState)
            {
                scriptContent += "\t\tpublic void AddObjectToList()\n";
                scriptContent += "\t\t{\n";
                scriptContent += "\t\t\tpatrol.patrolPoints.Add(\"\");\n";
                scriptContent += "\t\t}\n";

                scriptContent += "\t\tpublic void RemoveObjectFromList(string patrolPoint)\n";
                scriptContent += "\t\t{\n";
                scriptContent += "\t\t\tpatrol.RemovePatrolPoint(patrolPoint);\n";
                scriptContent +=
                    "\t\t\tGameObject patrolPointObject = FSMIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);\n";
                scriptContent += "\t\t\tif(patrolPointObject != null)\n";
                scriptContent += "\t\t\t{\n";
                scriptContent += "\t\t\t\tDestroyImmediate(patrolPointObject);\n";
                scriptContent += "\t\t\t}\n";
                scriptContent += "\t\t}\n";
            }

            scriptContent += "\t\tprivate void OnFootstep() {}\n";

            scriptContent += "\t}\n";
            scriptContent += "}\n";
            scriptContent += "#endif\n";
            return scriptContent;
        }

        private static string GenerateConditionsRecursive(FSMNodeSaveData node, string test, bool isFirst, bool isElse,
            string pastFalse)
        {
            if (node.NodeType == FSMNodeType.Initial || node.NodeType == FSMNodeType.State ||
                node.NodeType == FSMNodeType.CustomState)
            {
                test += ")\n";
                test += "\t\t{\n";
                test += $"\t\t\tChange{node.Name.Replace(" ", "")}State();\n";
                test += "\t\t}\n";
                return test;
            }

            if (node.NodeType == FSMNodeType.Extension)
            {
                FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[0].NodeId));
                bool check = nodeSaveData.Connections.Count == 2;
                return GenerateConditionsRecursive(nodeSaveData, test, isFirst, check, pastFalse);
            }

            string name = node.Name;
            string conditionName = char.ToLowerInvariant(name[0]) + name.Substring(1);
            conditionName = conditionName.Replace(" ", "");

            if (isFirst) test += $"\t\tif({conditionName}.Condition()";
            else test += $" && {conditionName}.Condition()";

            if (isElse)
            {
                FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[1].NodeId));
                bool check1 = nodeSaveData.Connections.Count == 2;

                FSMNodeSaveData nodeSaveData2 = GetNodeData(GetState(node.Connections[0].NodeId));
                bool check2 = nodeSaveData2.Connections.Count == 2;

                string pastTrue = pastFalse;

                if (pastFalse == "")
                {
                    pastFalse = $"\t\telse if(!{conditionName}.Condition()";
                }
                else pastFalse += $" && !{conditionName}.Condition()";

                if (pastTrue == "")
                {
                    pastTrue = $"\t\telse if({conditionName}.Condition()";
                }
                else pastTrue += $" && {conditionName}.Condition()";

                return GenerateConditionsRecursive(nodeSaveData2, test, false, check2, pastTrue) +
                       GenerateConditionsRecursive(nodeSaveData, pastFalse, false, check1, pastFalse);
            }
            else
            {
                pastFalse += $"\t\telse if({conditionName}.Condition()";

                FSMNodeSaveData nodeSaveData = GetNodeData(GetState(node.Connections[0].NodeId));
                bool check = nodeSaveData.Connections.Count == 2;
                return GenerateConditionsRecursive(nodeSaveData, test, false, check, pastFalse);
            }
        }

        private static string GenerateEditorScriptContent(FSMGraphSaveData saveData)
        {
            string scriptContent = "#if UNITY_EDITOR\n";
            scriptContent += "namespace EditorWindow.FSMSystem.Inspectors\n";
            scriptContent += "{\n";
            scriptContent += "\tusing System;\n";
            scriptContent += "\tusing System.Collections.Generic;\n";
            scriptContent += "\tusing UnityEditor;\n";
            scriptContent += "\tusing System.Reflection;\n";
            scriptContent += "\tusing BehaviorScripts;\n";
            scriptContent += "\tusing Utilities;\n";
            scriptContent += "\tusing UnityEngine;\n";
            scriptContent += "\tusing FSM.Nodes.States;\n";
            scriptContent += "\tusing FSM.Utilities;\n\n";
            
            string stringWithSpaces = saveData.FileName;
            string stringWithoutSpaces = stringWithSpaces.Replace(" ", "");

            scriptContent += $"\t[CustomEditor(typeof({stringWithoutSpaces}))]\n";
            scriptContent += $"\tpublic class {stringWithoutSpaces}Editor : Editor\n";
            scriptContent += "\t{\n";

            _states = new List<FSMNodeSaveData>();
            foreach (FSMNodeSaveData state in saveData.Nodes)
            {
                _states.Add(state);
            }

            string nameLowerCapital = char.ToLowerInvariant(stringWithoutSpaces[0]) + stringWithoutSpaces.Substring(1);

            scriptContent += "\t\tprivate SerializedProperty selectedOptionIndexProp;\n";
            scriptContent += "\t\tprivate float lastClickedIndex = -1;\n";
            scriptContent +=
                "\t\tDictionary<string, StateScript> optionToObjectMap = new Dictionary<string, StateScript>();\n";

            scriptContent += "\t\tvoid OnEnable()\n";
            scriptContent += "\t\t{\n";
            scriptContent +=
                "\t\t\tselectedOptionIndexProp = serializedObject.FindProperty(\"selectedOptionIndex\");\n";
            scriptContent += $"\t\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
            scriptContent += $"\t\t\tType type = typeof({stringWithoutSpaces});\n";
            scriptContent +=
                "\t\t\tFieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);\n";
            scriptContent += "\t\t\tint j = 0;\n";
            scriptContent += "\t\t\tforeach (FieldInfo field in fields)\n";
            scriptContent += "\t\t\t{\n";
            scriptContent += "\t\t\t\tstring nameField = FixName(field.Name);\n";
            scriptContent += $"\t\t\t\toptionToObjectMap[nameField] = {nameLowerCapital}.options[j];\n";
            scriptContent += "\t\t\t\tj++;\n";
            scriptContent += "\t\t\t}\n";
            scriptContent += "\t\t}\n";

            scriptContent += "\t\tpublic override void OnInspectorGUI()\n";
            scriptContent += "\t\t{\n";
            scriptContent += "\t\t\tserializedObject.Update();\n";
            scriptContent += $"\t\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
            scriptContent += $"\t\t\tType type = typeof({stringWithoutSpaces});\n";
            scriptContent +=
                "\t\t\tFieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);\n";
            scriptContent += $"\t\t\tstring[] options = new string[fields.Length];\n";
            scriptContent += "\t\t\tint x = 0;\n";
            scriptContent += "\t\t\tforeach (FieldInfo field in fields) \n";
            scriptContent += "\t\t\t{\n";
            scriptContent += "\t\t\t\tstring nameField = FixName(field.Name);\n";
            scriptContent += "\t\t\t\toptions[x] = nameField;\n";
            scriptContent += "\t\t\t\tx++;\n";
            scriptContent += "\t\t\t}\n";
            scriptContent += "\t\t\tstring selectedOptionName = options[selectedOptionIndexProp.intValue];\n";

            scriptContent += "\t\t\tGUIStyle buttonStyle = new GUIStyle(GUI.skin.button);\n";
            scriptContent += "\t\t\tbuttonStyle.normal.textColor = Color.white;\n";
            scriptContent += "\t\t\tbuttonStyle.hover.textColor = Color.white;\n";
            scriptContent += "\t\t\tbuttonStyle.fontSize = 14;\n";
            scriptContent += "\t\t\tbuttonStyle.fixedHeight = 30;\n";
            scriptContent += "\t\t\tbuttonStyle.fixedWidth = 150;\n";
            scriptContent += "\t\t\tbuttonStyle.margin = new RectOffset(5, 5, 5, 5);\n";
            scriptContent += "\t\t\tGUILayout.Space(10);\n";

            scriptContent +=
                "\t\t\tEditorGUILayout.LabelField(\"SELECT THE STATE YOU WANT TO MODIFY\", new GUIStyle(EditorStyles.boldLabel)\n";
            scriptContent += "\t\t\t{\n";
            scriptContent += "\t\t\t\talignment = TextAnchor.MiddleCenter,\n";
            scriptContent += "\t\t\t\tfontSize = 14\n";
            scriptContent += "\t\t\t});\n";
            scriptContent += "\t\t\tint buttonsPerRow = 3;\n";
            scriptContent += "\t\t\tint buttonCount = 0;\n";
            scriptContent += "\t\t\tGUILayout.BeginVertical();\n";
            scriptContent += "\t\t\tGUILayout.Space(10);\n";
            scriptContent += "\t\t\tGUILayout.BeginHorizontal();\n";
            scriptContent += "\t\t\tfor (int i = 0; i < options.Length; i++)\n";
            scriptContent += "\t\t\t{\n";
            scriptContent += "\t\t\t\tif (buttonCount > 0 && buttonCount % buttonsPerRow == 0)\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tGUILayout.EndHorizontal();\n";
            scriptContent += "\t\t\t\t\tGUILayout.BeginHorizontal();\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t\tbool isSelected = selectedOptionIndexProp.intValue == i;\n";
            scriptContent += "\t\t\t\tif (isSelected || lastClickedIndex == i)\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tGUI.backgroundColor = new Color(0.89f, 0.716f, 0.969f);\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t\telse\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tGUI.backgroundColor = new Color(0.575f, 0f, 0.671f);\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t\tGUILayout.FlexibleSpace();\n";
            scriptContent += "\t\t\t\tif (GUILayout.Button(options[i], buttonStyle))\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tselectedOptionIndexProp.intValue = i;\n";
            scriptContent += "\t\t\t\t\tlastClickedIndex = i;\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t\tGUILayout.FlexibleSpace();\n";
            scriptContent += "\t\t\t\tbuttonCount++;\n";
            scriptContent += "\t\t\t}\n";
            scriptContent += "\t\t\tGUI.backgroundColor = Color.white;\n";
            scriptContent += "\t\t\tGUILayout.EndHorizontal();\n";
            scriptContent += "\t\t\tGUILayout.EndVertical();\n";


            scriptContent += "\t\t\tif (optionToObjectMap.ContainsKey(selectedOptionName))\n";
            scriptContent += "\t\t\t{\n";
            scriptContent +=
                "\t\t\t\tEditorGUILayout.LabelField($\"{selectedOptionName} Attributes:\", EditorStyles.boldLabel);\n";
            scriptContent += "\t\t\t\tStateScript selectedObject = optionToObjectMap[selectedOptionName];\n";
            scriptContent +=
                "\t\t\t\tSerializedObject selectedObjectSerialized = new SerializedObject(selectedObject);\n";
            scriptContent += "\t\t\t\tselectedObjectSerialized.Update();\n";
            scriptContent += "\t\t\t\tEditorGUI.BeginChangeCheck();\n";
            scriptContent += "\t\t\t\tSerializedProperty iterator = selectedObjectSerialized.GetIterator();\n";
            scriptContent += "\t\t\t\tbool nextVisible = iterator.NextVisible(true);\n";
            scriptContent += "\t\t\t\twhile (nextVisible)\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tif (iterator.name != \"m_Script\")\n";
            scriptContent += "\t\t\t\t\t{\n";
            if (_hasPatrolState)
            {
                scriptContent += "\t\t\t\t\t\tif (iterator.isArray)\n";
                scriptContent += "\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.Space();\n";
                scriptContent +=
                    "\t\t\t\t\t\t\tEditorGUILayout.LabelField(\"Create a Patrol Waypoint\", EditorStyles.boldLabel);\n";
                scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.Space();\n";
                scriptContent += "\t\t\t\t\t\t\tfor (int i = 0; i < iterator.arraySize; i++)\n";
                scriptContent += "\t\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\t\tEditorGUILayout.BeginHorizontal();\n";
                scriptContent +=
                    "\t\t\t\t\t\t\t\tSerializedProperty gameObjectElementProperty = iterator.GetArrayElementAtIndex(i);\n";
                scriptContent += "\t\t\t\t\t\t\t\tif (gameObjectElementProperty.stringValue != null)\n";
                scriptContent += "\t\t\t\t\t\t\t\t{\n";
                scriptContent +=
                    "\t\t\t\t\t\t\t\t\tGameObject gameObject = FSMIOUtility.FindGameObjectWithId<IDGenerator>(gameObjectElementProperty.stringValue);\n";
                scriptContent += "\t\t\t\t\t\t\t\t\tEditorGUI.BeginChangeCheck();\n";
                scriptContent +=
                    "\t\t\t\t\t\t\t\t\tgameObject = EditorGUILayout.ObjectField(\"Patrol Point\", gameObject, typeof(GameObject), true) as GameObject;\n";
                scriptContent += "\t\t\t\t\t\t\t\t\tif (EditorGUI.EndChangeCheck())\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\tif (gameObject != null)\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t{\n";
                scriptContent +=
                    "\t\t\t\t\t\t\t\t\t\t\tIDGenerator idGenerator = gameObject.GetComponent<IDGenerator>();\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t\tif (idGenerator.GetUniqueID() != null)\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t\t{\n";
                scriptContent +=
                    "\t\t\t\t\t\t\t\t\t\t\t\tgameObjectElementProperty.stringValue = idGenerator.GetUniqueID();\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\telse\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t\tgameObjectElementProperty.stringValue = string.Empty;\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
                scriptContent +=
                    $"\t\t\t\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\t\tif (GUILayout.Button(\"Remove\", GUILayout.Width(70)))\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\tRemovePatrolPoint(gameObjectElementProperty.stringValue);\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
                scriptContent +=
                    $"\t\t\t\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n";
                scriptContent += "\t\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\t\tEditorGUILayout.EndHorizontal();\n";
                scriptContent += "\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t\tif (GUILayout.Button(\"Create and Add a Patrol Point\"))\n";
                scriptContent += "\t\t\t\t\t\t\t{\n";
                scriptContent += $"\t\t\t\t\t\t\t\tCreateAndAddGameObject({nameLowerCapital});\n";
                scriptContent += "\t\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
                scriptContent +=
                    $"\t\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n";
                scriptContent += "\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\telse\n";
                scriptContent += "\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\tEditorGUI.BeginChangeCheck();\n";
                scriptContent += "\t\t\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n";
                scriptContent += "\t\t\t\t\t\t\tif (EditorGUI.EndChangeCheck())\n";
                scriptContent += "\t\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
                scriptContent +=
                    $"\t\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n";
                scriptContent += "\t\t\t\t\t\t\t}\n";
                scriptContent += "\t\t\t\t\t\t}\n";
            }
            else
            {
                scriptContent += "\t\t\t\t\t\tEditorGUI.BeginChangeCheck();\n";
                scriptContent += "\t\t\t\t\t\tEditorGUILayout.PropertyField(iterator, true);\n";
                scriptContent += "\t\t\t\t\t\tif (EditorGUI.EndChangeCheck())\n";
                scriptContent += "\t\t\t\t\t\t{\n";
                scriptContent += "\t\t\t\t\t\t\tselectedObject.SetStateName(selectedOptionName);\n";
                scriptContent += "\t\t\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
                scriptContent += $"\t\t\t\t\t\t\tFSMIOUtility.CreateJson(selectedObject, \"{stringWithoutSpaces}\");\n";
                scriptContent += "\t\t\t\t\t\t}\n";
            }

            scriptContent += "\t\t\t\t\t}\n";
            scriptContent += "\t\t\t\t\tnextVisible = iterator.NextVisible(false);\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t\tif (EditorGUI.EndChangeCheck())\n";
            scriptContent += "\t\t\t\t{\n";
            scriptContent += "\t\t\t\t\tselectedObjectSerialized.ApplyModifiedProperties();\n";
            scriptContent += "\t\t\t\t}\n";
            scriptContent += "\t\t\t}\n";
            scriptContent += "\t\t\tserializedObject.ApplyModifiedProperties();\n";
            scriptContent += "\t\t}\n";

            if (_hasPatrolState)
            {
                scriptContent += $"\t\tprivate void CreateAndAddGameObject({stringWithoutSpaces} {nameLowerCapital})\n";
                scriptContent += "\t\t{\n";
                scriptContent += $"\t\t\t{nameLowerCapital}.AddObjectToList();\n";
                scriptContent += "\t\t}\n";
                scriptContent += "\t\tprivate void RemovePatrolPoint(string patrolPoint)\n";
                scriptContent += "\t\t{\n";
                scriptContent += $"\t\t\t{stringWithoutSpaces} {nameLowerCapital} = ({stringWithoutSpaces})target;\n";
                scriptContent += $"\t\t\t{nameLowerCapital}.patrol.RemovePatrolPoint(patrolPoint);\n";
                scriptContent +=
                    "\t\t\tGameObject patrolPointObject = FSMIOUtility.FindGameObjectWithId<IDGenerator>(patrolPoint);\n";
                scriptContent += "\t\t\tif(patrolPointObject != null)\n";
                scriptContent += "\t\t\t{\n";
                scriptContent += "\t\t\t\tDestroyImmediate(patrolPointObject);\n";
                scriptContent += "\t\t\t}\n";
                scriptContent += "\t\t}\n";
            }

            scriptContent += "\t\tprivate string FixName(string oldName)\n";
            scriptContent += "\t\t{\n";
            scriptContent += "\t\t\treturn char.ToUpperInvariant(oldName[0]) + oldName.Substring(1);\n";
            scriptContent += "\t\t}\n";

            scriptContent += "\t}\n";
            scriptContent += "}\n";
            scriptContent += "#endif\n";
            return scriptContent;
        }

        #region Utilities

        private static string GetState(string id)
        {
            foreach (var state in _states)
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
            foreach (var state in _states)
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
            foreach (var state in _states)
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
}
#endif