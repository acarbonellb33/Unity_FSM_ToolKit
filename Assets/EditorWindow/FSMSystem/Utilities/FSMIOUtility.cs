using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class FSMIOUtility
{
    private static FSMGraphView _graphView;
    private static string _initialState;
    private static string _graphName;
    private static string _containerFolderPath;
    private static FSMHitSaveData _hitData;
    
    private static List<FSMGroup> _groups;
    private static List<FSMNode> _nodes;

    private static Dictionary<string, FSMNodeGroupSO> _createdNodeGroups;
    private static Dictionary<string, FSMNodeSO> _createdNodes;
    
    private static Dictionary<string, FSMGroup> _loadedGroups;
    private static Dictionary<string, FSMNode> _loadedNodes;
    
    private static string _stateDataObject;

    public static void Initialize(string graphName, FSMGraphView fsmGraphView, string initialState, FSMHitSaveData hitData)
    {
        _graphView = fsmGraphView;
        _initialState = initialState;
        _graphName = graphName;
        _hitData = hitData;
        _containerFolderPath = $"Assets/FSMSystem/FSMs/{graphName}";
        
        _groups = new List<FSMGroup>();
        _nodes = new List<FSMNode>();
        
        _createdNodeGroups = new Dictionary<string, FSMNodeGroupSO>();
        _createdNodes = new Dictionary<string, FSMNodeSO>();
        _loadedGroups = new Dictionary<string, FSMGroup>();
        _loadedNodes = new Dictionary<string, FSMNode>();
    }

    #region SaveMethods
    public static bool Save()
    {
        GetElementsFromGraphView();
        
        foreach(FSMNode node in _nodes)
        {
            foreach (FSMConnectionSaveData choice in node.Choices)
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
        
        FSMGraphSaveData graphSaveData = CreateAsset<FSMGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
        graphSaveData.Initialize(_graphName, _initialState, _hitData);
        FSMNodesContainerSO nodesContainer = CreateAsset<FSMNodesContainerSO>(_containerFolderPath, _graphName);
        nodesContainer.Initialize(_graphName);
        
        SaveGroups(graphSaveData, nodesContainer);
        SaveNodes(graphSaveData, nodesContainer);

        SaveAsset(graphSaveData);
        SaveAsset(nodesContainer);
        
        return true;
    }

    #region Groups
    private static void SaveGroups(FSMGraphSaveData graphSaveData, FSMNodesContainerSO nodesContainerSo)
    {
        List<string> groupNames = new List<string>();
        foreach(FSMGroup group in _groups)
        {
            SaveGroupToGraph(group, graphSaveData);
            SaveGroupToScriptableObject(group, nodesContainerSo);
            groupNames.Add(group.title);
        }

        UpdateOldGroups(groupNames, graphSaveData);
    }
    private static void SaveGroupToGraph(FSMGroup group, FSMGraphSaveData graphSaveData)
    {
        FSMGroupSaveData groupSaveData = new FSMGroupSaveData()
        {
            Id = group.Id,
            Name = group.title,
            Position = group.GetPosition().position
        };
        graphSaveData.Groups.Add(groupSaveData);
    }
    
    private static void SaveGroupToScriptableObject(FSMGroup group, FSMNodesContainerSO nodesContainerSo)
    {
        string groupName = group.title;
        CreateFolder($"{_containerFolderPath}/Groups", groupName);
        CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Nodes");
        
        FSMNodeGroupSO nodesGroup = CreateAsset<FSMNodeGroupSO>($"{_containerFolderPath}/Groups/{groupName}", groupName);
        nodesGroup.Initialize(groupName);
        
        _createdNodeGroups.Add(group.Id, nodesGroup);
        
        nodesContainerSo.GroupedNodes.Add(nodesGroup, new List<FSMNodeSO>());

        SaveAsset(nodesGroup);
    }
    
    private static void UpdateOldGroups(List<string> currentGroupNames, FSMGraphSaveData graphSaveData)
    {
        if (graphSaveData.OldGroupedNames != null && graphSaveData.OldGroupedNames.Count != 0)
        {
            List<string> groupsToRemove = graphSaveData.OldGroupedNames.Except(currentGroupNames).ToList();

            foreach (string groupToRemove in groupsToRemove)
            {
                RemoveFolder($"{_containerFolderPath}/Groups/{groupToRemove}");
            }
        }

        graphSaveData.OldGroupedNames = new List<string>(currentGroupNames);
    }
    #endregion

    #region Nodes
    private static void SaveNodes(FSMGraphSaveData graphSaveData, FSMNodesContainerSO nodesContainer)
    {
        SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
        List<string> ungroupedNodeNames = new List<string>();
        
        foreach(FSMNode node in _nodes)
        {
            SaveNodeToGraph(node, graphSaveData, nodesContainer);
            if(node.Group != null)
            {
                groupedNodeNames.AddItem(node.Group.title, node.StateName);
                continue;
            }
            ungroupedNodeNames.Add(node.StateName);
        }

        UpdateNodeConnections();
        UpdateOldGroupedNodes(groupedNodeNames, graphSaveData);
        UpdateOldUngroupedNodes(ungroupedNodeNames, graphSaveData);
    }
    private static void SaveNodeToGraph(FSMNode node, FSMGraphSaveData graphSaveData, FSMNodesContainerSO nodesContainer)
    {
        List<FSMConnectionSaveData> connections = CloneNodeConnections(node.Choices);
        _stateDataObject = CreateJsonDataObject(node, nodesContainer);

        FSMNodeSaveData nodeSaveData = new FSMNodeSaveData()
        {
            Id = node.Id,
            Name = node.StateName,
            Connections = connections,
            GroupId = node.Group?.Id,
            NodeType = node.NodeType,
            Position = node.GetPosition().position,
            DataObject = _stateDataObject,
        };
        graphSaveData.Nodes.Add(nodeSaveData);
    }
    private static List<FSMNodeConnectionData> ConvertNodeConnection(List<FSMConnectionSaveData> connections)
    {
        List<FSMNodeConnectionData> convertedConnections = new List<FSMNodeConnectionData>();
        foreach(FSMConnectionSaveData connection in connections)
        {
            FSMNodeConnectionData connectionSo = new FSMNodeConnectionData()
            {
                Text = connection.Text,
            };
            convertedConnections.Add(connectionSo);
        }
        return convertedConnections;
    }
    private static void UpdateNodeConnections()
    {
        foreach(FSMNode node in _nodes)
        {
            FSMNodeSO nodeSelected = _createdNodes[node.Id];
            for(int index = 0; index < node.Choices.Count; index++)
            {
                FSMConnectionSaveData connection = node.Choices[index];
                if(string.IsNullOrEmpty(connection.NodeId))
                {
                    continue;
                }
                nodeSelected.Connections[index].NextNode = _createdNodes[connection.NodeId];
                SaveAsset(nodeSelected);
            }
        }
    }
    private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, FSMGraphSaveData graphSaveData)
    {
        if (graphSaveData.OldGroupedNodeNames != null && graphSaveData.OldGroupedNodeNames.Count != 0)
        {
            foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphSaveData.OldGroupedNodeNames)
            {
                List<string> nodesToRemove = new List<string>();

                if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                {
                    nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                }

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{_containerFolderPath}/Groups/{oldGroupedNode.Key}/Nodes", nodeToRemove);
                }
            }
        }
    }
    private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, FSMGraphSaveData graphSaveData)
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
        FSMGraphSaveData graphSaveData = LoadAsset<FSMGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
        if (graphSaveData == null)
        {
            EditorUtility.DisplayDialog(
                "Error loading the graph", 
                "The file at the following path could not be found:\n\n" +
                $"Assets/EditorWindow/FSMSystem/Graphs/{_graphName}\n\n",
                $"Ok");
            return;
        }
        
        List<string> stateNames = new List<string>();
        foreach(FSMNodeSaveData node in graphSaveData.Nodes)
        {
            if(node.NodeType == FSMNodeType.State)
            {
                stateNames.Add(node.Name);
            }
        }
        
        LoadGroups(graphSaveData.Groups);
        LoadNodes(graphSaveData.Nodes);
        LoadConnections();
        LoadHitData(graphSaveData.HitData);
    }
    private static void LoadGroups(List<FSMGroupSaveData> groups)
    {
        foreach(FSMGroupSaveData groupData in groups)
        {
            FSMGroup group = _graphView.CreateGroup(groupData.Name, groupData.Position);
            group.Id = groupData.Id;
            _loadedGroups.Add(group.Id, group);
        }
    }
    private static void LoadNodes(List<FSMNodeSaveData> nodes)
    {
        foreach(FSMNodeSaveData nodeData in nodes)
        {
            List<FSMConnectionSaveData> connections = CloneNodeConnections(nodeData.Connections);

            FSMNode node = _graphView.CreateNode(nodeData.Name, nodeData.Position, nodeData.NodeType, false);
            node.Id = nodeData.Id;
            node.StateName = nodeData.Name;
            node.Choices = connections;
            node.StateScript = LoadFromJson(node);
            node.Draw();

            _graphView.AddElement(node);
            _loadedNodes.Add(node.Id, node);
            if(string.IsNullOrEmpty(nodeData.GroupId))
            {
                continue;
            }
            FSMGroup group = _loadedGroups[nodeData.GroupId];
            node.Group = group;
            group.AddElement(node);
        }
    }
    public static FSMNode LoadNode(FSMNodeSaveData nodeData, string fileName)
    {
        List<FSMConnectionSaveData> connections = CloneNodeConnections(nodeData.Connections);
        FSMNode node = new FSMNode();
        node.Id = nodeData.Id; 
        node.StateName = nodeData.Name;
        node.Choices = connections;
        node.NodeType = nodeData.NodeType;
        node.StateScript = LoadFromJson(node, nodeData.Name,$"Assets/FSMSystem/FSMs/{fileName}/Global/Nodes/{nodeData.Name.Replace(" ","")}DataFile.json");
        return node;
    }
    private static StateScript LoadFromJson(FSMNode node, string nodeName, string path)
    {
        if(node.NodeType == FSMNodeType.Initial)
        {
            return null;
        }
        string json = File.ReadAllText(path);
        string newName = nodeName.Split(' ')[0];
        
        switch (newName)
        {
            case "Patrol":
                PatrolData patrolData = JsonUtility.FromJson<PatrolData>(json);
                node.StateScript = new PatrolStateScript();
                ((PatrolStateScript)node.StateScript).patrolPoints = patrolData.patrolPoints;
                ((PatrolStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Attack":
                AttackData attackData = JsonUtility.FromJson<AttackData>(json);
                node.StateScript = new AttackStateScript();
                ((AttackStateScript)node.StateScript).attackDamage = attackData.attackDamage;
                ((AttackStateScript)node.StateScript).attackFrequency = attackData.attackFrequency;
                ((AttackStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Chase":
                ChaseData chaseData = JsonUtility.FromJson<ChaseData>(json);
                node.StateScript = new ChaseStateScript();
                ((ChaseStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Search":
                SearchData searchData = JsonUtility.FromJson<SearchData>(json);
                node.StateScript = new SearchStateScript();
                ((SearchStateScript)node.StateScript).exploreRadius = searchData.exploreRadius;
                ((SearchStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Hearing":
                HearingData hearingData = JsonUtility.FromJson<HearingData>(json);
                node.StateScript = new HearingConditionScript();
                ((HearingConditionScript)node.StateScript).hearingRange = hearingData.hearingRange;
                ((HearingConditionScript)node.StateScript).operand = hearingData.operand;
                ((HearingConditionScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Seeing":
                SeeingData seeingData = JsonUtility.FromJson<SeeingData>(json);
                node.StateScript = new SeeingConditionScript();
                ((SeeingConditionScript)node.StateScript).distance = seeingData.distance;
                ((SeeingConditionScript)node.StateScript).SetStateName(node.StateName);
                break;
        }
        return node.StateScript;
    }
    private static StateScript LoadFromJson(FSMNode node)
    {
        if (node.NodeType == FSMNodeType.Initial) return null;
        string json = File.ReadAllText($"{_containerFolderPath}/Global/Nodes/{node.StateName.Replace(" ","")}DataFile.json");
        string newName = node.StateName.Split(' ')[0];
        switch (newName)
        {
            case "Patrol":
                PatrolData patrolData = JsonUtility.FromJson<PatrolData>(json);
                node.StateScript = new PatrolStateScript();
                ((PatrolStateScript)node.StateScript).patrolPoints = patrolData.patrolPoints;
                ((PatrolStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Attack":
                AttackData attackData = JsonUtility.FromJson<AttackData>(json);
                node.StateScript = new AttackStateScript();
                ((AttackStateScript)node.StateScript).attackDamage = attackData.attackDamage;
                ((AttackStateScript)node.StateScript).attackFrequency = attackData.attackFrequency;
                ((AttackStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Chase":
                ChaseData chaseData = JsonUtility.FromJson<ChaseData>(json);
                node.StateScript = new ChaseStateScript();
                ((ChaseStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Search":
                SearchData searchData = JsonUtility.FromJson<SearchData>(json);
                node.StateScript = new SearchStateScript();
                ((SearchStateScript)node.StateScript).exploreRadius = searchData.exploreRadius;
                ((SearchStateScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Hearing":
                HearingData hearingData = JsonUtility.FromJson<HearingData>(json);
                node.StateScript = new HearingConditionScript();
                ((HearingConditionScript)node.StateScript).hearingRange = hearingData.hearingRange;
                ((HearingConditionScript)node.StateScript).operand = hearingData.operand;
                ((HearingConditionScript)node.StateScript).SetStateName(node.StateName);
                break;
            case "Seeing":
                SeeingData seeingData = JsonUtility.FromJson<SeeingData>(json);
                node.StateScript = new SeeingConditionScript();
                ((SeeingConditionScript)node.StateScript).distance = seeingData.distance;
                ((SeeingConditionScript)node.StateScript).SetStateName(node.StateName);
                break;
        }
        return node.StateScript;
    }
    private static void LoadConnections()
    {
        foreach (KeyValuePair<string, FSMNode> loadedNode in _loadedNodes)
        {
            foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
            {
                FSMConnectionSaveData choiceData = (FSMConnectionSaveData) choicePort.userData;
                if (string.IsNullOrEmpty(choiceData.NodeId))
                {
                    continue;
                }

                FSMNode nextNode = _loadedNodes[choiceData.NodeId];

                Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                _graphView.AddElement(edge);

                loadedNode.Value.RefreshPorts();
            }
        }
    }
    private static void LoadHitData(FSMHitSaveData hitData)
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
        CreateFolder(_containerFolderPath, "Groups");
        CreateFolder($"{_containerFolderPath}/Global", "Nodes");
    }
    private static string CreateJsonDataObject(FSMNode node, FSMNodesContainerSO nodesContainer)
    {
        string json = null;
        if(node.NodeType != FSMNodeType.Initial)
        {
            json = JsonUtility.ToJson(node.StateScript, true);
            File.WriteAllText($"{_containerFolderPath}/Global/Nodes/{node.StateName.Replace(" ","")}DataFile.json", json);
        }
        FSMNodeSO nodeSo;
        if (node.Group != null)
        {
            nodeSo = CreateAsset<FSMNodeSO>($"{_containerFolderPath}/Groups/{node.Group.title}/Nodes", node.StateName);
            nodesContainer.GroupedNodes.AddItem(_createdNodeGroups[node.Group.Id], nodeSo);
        }
        else
        {
            nodeSo = CreateAsset<FSMNodeSO>($"{_containerFolderPath}/Global/Nodes", node.StateName);
            nodesContainer.UngroupedNodes.Add(nodeSo);

        }
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
        File.WriteAllText(Application.dataPath+$"/FSMSystem/FSMs/{className}/Global/Nodes/{stateScript.GetStateName().Replace(" ","")}DataFile.json", json);
        
        string[] parts = Regex.Split(stateScript.GetStateName(), @"(\d)", RegexOptions.None);
        string result = "";
        for (int i=0; i< parts.Length; i++)
        {
            if(i+2 < parts.Length)result += parts[i] + " ";
            else result += parts[i];
        }
        
        FSMNodeSO node = LoadAsset<FSMNodeSO>($"Assets/FSMSystem/FSMs/{className}/Global/Nodes", result);
        node.DataObject = json;

        FSMGraphSaveData graphSaveData = LoadAsset<FSMGraphSaveData>($"Assets/EditorWindow/FSMSystem/Graphs", className);
        foreach(FSMNodeSaveData nodeData in graphSaveData.Nodes)
        {
            if (nodeData.Name == node.NodeName)
            {
                nodeData.DataObject = json;
            }
        }
    }
    
    public static void UpdateJson(string className, string fileName, string variableName, object newValue)
    {
        string jsonFilePath = Path.Combine(Application.dataPath+$"/FSMSystem/FSMs/{className}/Global/Nodes", $"{fileName.Replace(" ","")}DataFile.json");
        string jsonString = File.ReadAllText(jsonFilePath);
        
        int startIndex = jsonString.IndexOf($"\"{variableName}\"") + variableName.Length + 4;
        
        if (startIndex > variableName.Length + 4)
        {
            int endIndex = jsonString.IndexOf(',', startIndex);
            if (endIndex == -1)
            {
                endIndex = jsonString.IndexOf('}', startIndex);
            }
            string updatedJsonString = jsonString.Substring(0, startIndex) + newValue.ToString() + jsonString.Substring(endIndex);
            
            File.WriteAllText(jsonFilePath, updatedJsonString);
        }
        else
        {
            Debug.LogError($"Variable \"{variableName}\" not found in the JSON file.");
        }
    }

    #endregion

    #region UtilityMethods
    public static void CreateFolder(string path, string folderName)
    {
        if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
        {
            return;
        }
        AssetDatabase.CreateFolder(path, folderName);
    }
    public static void RemoveFolder(string path)
    {
        FileUtil.DeleteFileOrDirectory($"{path}.meta");
        FileUtil.DeleteFileOrDirectory($"{path}/");
    }
    public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
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
    public static void RemoveAsset(string path, string assetName)
    {
        AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
    }
    public static void SaveAsset(UnityEngine.Object asset)
    {
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
    {
        string assetPathAndName = $"{path}/{assetName}.asset";
        return AssetDatabase.LoadAssetAtPath<T>(assetPathAndName);
    }
    private static List<FSMConnectionSaveData> CloneNodeConnections(List<FSMConnectionSaveData> connections)
    {
        List<FSMConnectionSaveData> clonedConnections = new List<FSMConnectionSaveData>();
        foreach(FSMConnectionSaveData connection in connections)
        {
            FSMConnectionSaveData clonedConnection = new FSMConnectionSaveData()
            {
                Text = connection.Text,
                NodeId = connection.NodeId
            };
            clonedConnections.Add(clonedConnection);
        }

        return clonedConnections;
    }
    #endregion
    
    #region GetMethods
    private static void GetElementsFromGraphView()
    {
        Type groupType = typeof(FSMGroup);
        
        _graphView.graphElements.ForEach(graphElement =>
        {
            if (graphElement is FSMNode node)
            {
                _nodes.Add(node);
                return;
            }

            if (graphElement.GetType() == groupType)
            {
                FSMGroup group = (FSMGroup) graphElement;
                _groups.Add(group);
                return;
            }
        });
    }
    #endregion
}
