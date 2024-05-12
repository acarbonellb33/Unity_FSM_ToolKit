#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Data.Save;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Elements;
    using Windows;
    using FSM.Enumerations;
    using FSM.Nodes.Data;
    using FSM.Nodes.ScriptableObjects;
    using FSM.Nodes.States;
    using FSM.Nodes.States.StatesData;
    using FSM.Utilities;
    public static class FsmIOUtility
    {
        private static FsmGraphView _graphView;
        private static string _initialState;
        private static string _graphName;
        private static string _containerFolderPath;
        private static FsmHitSaveData _hitData;
        
        private static List<FsmNode> _nodes;
        private static Dictionary<string, FsmNodeSo> _createdNodes;
        private static Dictionary<string, FsmNode> _loadedNodes;

        private static string _stateDataObject;

        public static void Initialize(string graphName, FsmGraphView fsmGraphView, string initialState,
            FsmHitSaveData hitData)
        {
            _graphView = fsmGraphView;
            _initialState = initialState;
            _graphName = graphName;
            _hitData = hitData;
            _containerFolderPath = $"Assets/FSMSystem/FSMs/{graphName}";
            
            _nodes = new List<FsmNode>();
            _createdNodes = new Dictionary<string, FsmNodeSo>();
            _loadedNodes = new Dictionary<string, FsmNode>();
        }

        #region SaveMethods

        public static bool Save()
        {
            GetElementsFromGraphView();

            foreach (FsmNode node in _nodes)
            {
                foreach (FsmConnectionSaveData choice in node.Choices)
                {
                    if (String.IsNullOrEmpty(choice.NodeId))
                    {
                        EditorUtility.DisplayDialog(
                            $"Node {choice.Text} is not connected!",
                            "Pleas connect all ports before saving.",
                            "OK"
                        );
                        return false;
                    }
                }
            }

            CreateStaticFolders();

            FsmGraphSaveData graphSaveData =
                CreateAsset<FsmGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
            graphSaveData.Initialize(_graphName, _initialState, _hitData);
            FsmNodesContainerSo nodesContainer = CreateAsset<FsmNodesContainerSo>(_containerFolderPath, _graphName);
            nodesContainer.Initialize(_graphName);

            SaveNodes(graphSaveData, nodesContainer);

            SaveAsset(graphSaveData);
            SaveAsset(nodesContainer);

            return true;
        }
        

        #region Nodes

        private static void SaveNodes(FsmGraphSaveData graphSaveData, FsmNodesContainerSo nodesContainer)
        {
            List<string> ungroupedNodeNames = new List<string>();

            foreach (FsmNode node in _nodes)
            {
                SaveNodeToGraph(node, graphSaveData, nodesContainer);
                ungroupedNodeNames.Add(node.StateName);
            }

            UpdateNodeConnections();
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphSaveData);
        }

        private static void SaveNodeToGraph(FsmNode node, FsmGraphSaveData graphSaveData,
            FsmNodesContainerSo nodesContainer)
        {
            List<FsmConnectionSaveData> connections = CloneNodeConnections(node.Choices);
            _stateDataObject = CreateJsonDataObject(node, nodesContainer);

            FsmNodeSaveData nodeSaveData = new FsmNodeSaveData()
            {
                Id = node.Id,
                Name = node.StateName,
                Connections = connections,
                NodeType = node.NodeType,
                Position = node.GetPosition().position,
                AnimatorSaveData = node.GetAnimatorSaveData(),
                DataObject = _stateDataObject,
            };
            graphSaveData.Nodes.Add(nodeSaveData);
        }

        private static List<FsmNodeConnectionData> ConvertNodeConnection(List<FsmConnectionSaveData> connections)
        {
            List<FsmNodeConnectionData> convertedConnections = new List<FsmNodeConnectionData>();
            foreach (FsmConnectionSaveData connection in connections)
            {
                FsmNodeConnectionData connectionSo = new FsmNodeConnectionData()
                {
                    Text = connection.Text,
                };
                convertedConnections.Add(connectionSo);
            }

            return convertedConnections;
        }

        private static void UpdateNodeConnections()
        {
            foreach (FsmNode node in _nodes)
            {
                FsmNodeSo nodeSelected = _createdNodes[node.Id];
                for (int index = 0; index < node.Choices.Count; index++)
                {
                    FsmConnectionSaveData connection = node.Choices[index];
                    if (string.IsNullOrEmpty(connection.NodeId))
                    {
                        continue;
                    }

                    nodeSelected.Connections[index].NextNode = _createdNodes[connection.NodeId];
                    SaveAsset(nodeSelected);
                }
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames,
            FsmGraphSaveData graphSaveData)
        {
            if (graphSaveData.OldUngroupedNames != null && graphSaveData.OldUngroupedNames.Count != 0)
            {
                List<string> nodesToRemove = graphSaveData.OldUngroupedNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{_containerFolderPath}/Global/Nodes/", nodeToRemove);
                }
            }

            graphSaveData.OldUngroupedNames = new List<string>(currentUngroupedNodeNames);
        }

        #endregion

        #endregion

        #region LoadMethods

        public static void Load()
        {
            FsmGraphSaveData graphSaveData =
                LoadAsset<FsmGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
            if (graphSaveData == null)
            {
                EditorUtility.DisplayDialog(
                    "Error loading the graph",
                    "The file at the following path could not be found:\n\n" +
                    $"Assets/EditorWindow/FSMSystem/Graphs/{_graphName}\n\n",
                    $"Ok");
                return;
            }
            
            LoadNodes(graphSaveData.Nodes);
            LoadConnections();
            LoadHitData(graphSaveData.HitData);
        }

        private static void LoadNodes(List<FsmNodeSaveData> nodes)
        {
            foreach (FsmNodeSaveData nodeData in nodes)
            {
                List<FsmConnectionSaveData> connections = CloneNodeConnections(nodeData.Connections);

                FsmNode node = _graphView.CreateNode(nodeData.Name, nodeData.Position, nodeData.NodeType, false, false);
                node.Id = nodeData.Id;
                node.StateName = nodeData.Name;
                node.Choices = connections;
                node.StateScript = LoadFromJson(node);
                if (node.NodeType == FsmNodeType.State || node.NodeType == FsmNodeType.CustomState)
                {
                    node.SetAnimatorSaveData(nodeData.AnimatorSaveData);
                }

                node.Draw();

                _graphView.AddElement(node);
                _loadedNodes.Add(node.Id, node);
            }
        }

        public static FsmNode LoadNode(FsmNodeSaveData nodeData, string fileName)
        {
            var connections = CloneNodeConnections(nodeData.Connections);
            var node = new FsmNode
            {
                Id = nodeData.Id,
                StateName = nodeData.Name,
                Choices = connections,
                NodeType = nodeData.NodeType
            };
            if (node.NodeType == FsmNodeType.State)
            {
                node.SetAnimatorSaveData(nodeData.AnimatorSaveData);
            }

            node.StateScript = LoadFromJson(node, nodeData.Name,
                $"Assets/FSMSystem/FSMs/{fileName}/Global/Nodes/{nodeData.Name.Replace(" ", "")}DataFile.json");
            return node;
        }

        private static StateScriptData LoadFromJson(FsmNode node, string nodeName, string path)
        {
            if (node.NodeType == FsmNodeType.Initial)
            {
                return null;
            }

            string json = File.ReadAllText(path);
            string newName = nodeName.Split(' ')[0];

            switch (newName)
            {
                case "Patrol":
                    PatrolData patrolData = JsonUtility.FromJson<PatrolData>(json);
                    node.StateScript = patrolData;
                    break;
                case "Attack":
                    AttackData attackData = JsonUtility.FromJson<AttackData>(json);
                    node.StateScript = attackData;
                    break;
                case "Chase":
                    ChaseData chaseData = JsonUtility.FromJson<ChaseData>(json);
                    node.StateScript = chaseData;
                    break;
                case "Search":
                    SearchData searchData = JsonUtility.FromJson<SearchData>(json);
                    node.StateScript = searchData;
                    break;
                case "Hearing":
                    HearingData hearingData = JsonUtility.FromJson<HearingData>(json);
                    node.StateScript = hearingData;
                    break;
                case "Seeing":
                    SeeingData seeingData = JsonUtility.FromJson<SeeingData>(json);
                    node.StateScript = seeingData;
                    break;
                case "Distance":
                    DistanceData distanceData = JsonUtility.FromJson<DistanceData>(json);
                    node.StateScript = distanceData;
                    break;
                case "Health":
                    HealthData healthData = JsonUtility.FromJson<HealthData>(json);
                    node.StateScript = healthData;
                    break;
                case "NextState":
                    NextStateData nextStateData = JsonUtility.FromJson<NextStateData>(json);
                    node.StateScript = nextStateData;
                    break;
                case "Custom":
                    CustomData customData = JsonUtility.FromJson<CustomData>(json);
                    node.StateScript = customData;
                    break;
            }

            return node.StateScript;
        }

        private static StateScriptData LoadFromJson(FsmNode node)
        {
            if (node.NodeType == FsmNodeType.Initial || node.NodeType == FsmNodeType.Extension) return null;
            string json =
                File.ReadAllText($"{_containerFolderPath}/Global/Nodes/{node.StateName.Replace(" ", "")}DataFile.json");
            string newName = node.StateName.Split(' ')[0];
            switch (newName)
            {
                case "Patrol":
                    PatrolData patrolData = JsonUtility.FromJson<PatrolData>(json);
                    node.StateScript = patrolData;
                    break;
                case "Attack":
                    AttackData attackData = JsonUtility.FromJson<AttackData>(json);
                    node.StateScript = attackData;
                    break;
                case "Chase":
                    ChaseData chaseData = JsonUtility.FromJson<ChaseData>(json);
                    node.StateScript = chaseData;
                    break;
                case "Search":
                    SearchData searchData = JsonUtility.FromJson<SearchData>(json);
                    node.StateScript = searchData;
                    break;
                case "Hearing":
                    HearingData hearingData = JsonUtility.FromJson<HearingData>(json);
                    node.StateScript = hearingData;
                    break;
                case "Seeing":
                    SeeingData seeingData = JsonUtility.FromJson<SeeingData>(json);
                    node.StateScript = seeingData;
                    break;
                case "Distance":
                    DistanceData distanceData = JsonUtility.FromJson<DistanceData>(json);
                    node.StateScript = distanceData;
                    break;
                case "Health":
                    HealthData healthData = JsonUtility.FromJson<HealthData>(json);
                    node.StateScript = healthData;
                    break;
                case "NextState":
                    NextStateData nextStateData = JsonUtility.FromJson<NextStateData>(json);
                    node.StateScript = nextStateData;
                    break;
                case "Custom":
                    CustomData customData = JsonUtility.FromJson<CustomData>(json);
                    node.StateScript = customData;
                    break;
            }

            return node.StateScript;
        }

        private static void LoadConnections()
        {
            foreach (KeyValuePair<string, FsmNode> loadedNode in _loadedNodes)
            {
                foreach (var outputElement in loadedNode.Value.outputContainer.Children())
                {
                    if (outputElement is Port choicePort)
                    {
                        FsmConnectionSaveData choiceData = (FsmConnectionSaveData)choicePort.userData;
                        if (string.IsNullOrEmpty(choiceData.NodeId))
                        {
                            choicePort.portColor = Color.red;
                            continue;
                        }

                        choicePort.portColor = Color.white;

                        if (_loadedNodes.TryGetValue(choiceData.NodeId, out FsmNode nextNode))
                        {
                            foreach (var inputElement in nextNode.inputContainer.Children())
                            {
                                if (inputElement is Port nextNodeInputPort)
                                {
                                    nextNodeInputPort.portColor = Color.white;

                                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);
                                    _graphView.AddElement(edge);
                                    loadedNode.Value.RefreshPorts();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void LoadHitData(FsmHitSaveData hitData)
        {
            _graphView.GetWindow().GetHitStatePopup().Initialize(hitData);
        }

        #endregion

        #region CreationMethods

        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/EditorWindow/FSMSystem", "Graphs");
            CreateFolder("Assets", "FSMSystem");
            CreateFolder("Assets/FSMSystem", "FSMs");
            CreateFolder("Assets/FSMSystem/FSMs", _graphName);
            CreateFolder(_containerFolderPath, "Global");
            CleanFolder($"{_containerFolderPath}/Global/Nodes");
            CreateFolder($"{_containerFolderPath}/Global", "Nodes");
        }

        private static string CreateJsonDataObject(FsmNode node, FsmNodesContainerSo nodesContainer)
        {
            string json = null;
            if (node.NodeType != FsmNodeType.Initial && node.NodeType != FsmNodeType.Extension)
            {
                json = JsonUtility.ToJson(node.StateScript, true);
                File.WriteAllText($"{_containerFolderPath}/Global/Nodes/{node.StateName.Replace(" ", "")}DataFile.json",
                    json);
            }
            var nodeSo = CreateAsset<FsmNodeSo>($"{_containerFolderPath}/Global/Nodes", node.StateName);
            nodesContainer.UngroupedNodes.Add(nodeSo);
            
            nodeSo.Initialize(
                node.StateName,
                "Text",
                ConvertNodeConnection(node.Choices),
                node.NodeType,
                json
            );
            _createdNodes.Add(node.Id, nodeSo);
            SaveAsset(nodeSo);
            return json;
        }

        public static void CreateJson(StateScript stateScript, string className)
        {
            string json = JsonUtility.ToJson(stateScript, true);
            File.WriteAllText(
                Application.dataPath +
                $"/FSMSystem/FSMs/{className}/Global/Nodes/{stateScript.GetStateName().Replace(" ", "")}DataFile.json",
                json);

            string[] parts = Regex.Split(stateScript.GetStateName(), @"(\d)", RegexOptions.None);
            string result = "";
            for (int i = 0; i < parts.Length; i++)
            {
                if (i + 2 < parts.Length) result += parts[i] + " ";
                else result += parts[i];
            }

            FsmNodeSo node = LoadAsset<FsmNodeSo>($"Assets/FSMSystem/FSMs/{className}/Global/Nodes", result);
            node.DataObject = json;

            FsmGraphSaveData graphSaveData =
                LoadAsset<FsmGraphSaveData>($"Assets/EditorWindow/FSMSystem/Graphs", className);
            foreach (FsmNodeSaveData nodeData in graphSaveData.Nodes)
            {
                if (nodeData.Name == node.NodeName)
                {
                    nodeData.DataObject = json;
                }
            }
        }

        public static void UpdateJson(string className, string fileName, string variableName, object newValue)
        {
            string jsonFilePath = Path.Combine(Application.dataPath + $"/FSMSystem/FSMs/{className}/Global/Nodes",
                $"{fileName.Replace(" ", "")}DataFile.json");
            string jsonString = File.ReadAllText(jsonFilePath);

            int startIndex = jsonString.IndexOf($"\"{variableName}\"", StringComparison.Ordinal) + variableName.Length + 4;

            if (startIndex > variableName.Length + 4)
            {
                int endIndex = jsonString.IndexOf(',', startIndex);
                if (endIndex == -1)
                {
                    endIndex = jsonString.IndexOf('}', startIndex);
                }

                string updatedJsonString = jsonString.Substring(0, startIndex) + newValue +
                                           jsonString.Substring(endIndex);

                File.WriteAllText(jsonFilePath, updatedJsonString);
            }
            else
            {
                Debug.LogError($"Variable \"{variableName}\" not found in the JSON file.");
            }
        }

        #endregion

        #region UtilityMethods

        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }

        private static void CleanFolder(string folderPath)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(folderPath))
                {
                    // Delete all ScriptableObject files (files with .asset extension)
                    string[] scriptableObjectFiles = Directory.GetFiles(folderPath, "*.asset");
                    foreach (string file in scriptableObjectFiles)
                    {
                        File.Delete(file);
                        File.Delete($"{file}.meta");
                        AssetDatabase.Refresh();
                    }

                    // Delete all JSON files
                    string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
                    foreach (string file in jsonFiles)
                    {
                        File.Delete(file);
                        File.Delete($"{file}.meta");
                        AssetDatabase.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error cleaning folder: " + ex.Message);
            }
        }


        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string assetPathAndName = $"{path}/{assetName}.asset";
            T asset = LoadAsset<T>(path, assetName);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPathAndName);
            }

            return asset;
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string assetPathAndName = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(assetPathAndName);
        }

        private static List<FsmConnectionSaveData> CloneNodeConnections(List<FsmConnectionSaveData> connections)
        {
            List<FsmConnectionSaveData> clonedConnections = new List<FsmConnectionSaveData>();
            foreach (FsmConnectionSaveData connection in connections)
            {
                FsmConnectionSaveData clonedConnection = new FsmConnectionSaveData()
                {
                    Text = connection.Text,
                    NodeId = connection.NodeId
                };
                clonedConnections.Add(clonedConnection);
            }

            return clonedConnections;
        }

        public static GameObject FindGameObjectWithId<T>(string id) where T : MonoBehaviour
        {
            // Find all GameObjects with the component of type T
            T[] components = GameObject.FindObjectsOfType<T>();

            // Iterate through each GameObject and check if it has the specified ID
            foreach (T component in components)
            {
                // Check if the component has an IDGenerator attached
                IDGenerator idGenerator = component.GetComponent<IDGenerator>();
                if (idGenerator != null && idGenerator.GetUniqueID() == id)
                {
                    // Return the GameObject if the ID matches
                    return component.gameObject;
                }
            }

            // Return null if no matching GameObject is found
            return null;
        }

        #endregion

        #region GetMethods

        private static void GetElementsFromGraphView()
        {
            _graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is FsmNode node)
                {
                    _nodes.Add(node);
                }
            });
        }

        #endregion
    }
}
#endif