using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class FSMIOUtility
{
    private static FSMGraphView _graphView;
    private static string _graphName;
    private static string _containerFolderPath;
    
    private static List<FSMGroup> _groups;
    private static List<FSMNode> _nodes;

    private static Dictionary<string, FSMNodeGroupSO> _createdNodeGroups;
    private static Dictionary<string, FSMNodeSO> _createdNodes;
    
    private static Dictionary<string, FSMGroup> _loadedGroups;
    private static Dictionary<string, FSMNode> _loadedNodes;

    public static void Initialize(string graphName, FSMGraphView fsmGraphView)
    {
        _graphView = fsmGraphView;
        _graphName = graphName;
        _containerFolderPath = $"Assets/FSMSystem/FSMs/{graphName}";
        
        _groups = new List<FSMGroup>();
        _nodes = new List<FSMNode>();
        
        _createdNodeGroups = new Dictionary<string, FSMNodeGroupSO>();
        _createdNodes = new Dictionary<string, FSMNodeSO>();
        _loadedGroups = new Dictionary<string, FSMGroup>();
        _loadedNodes = new Dictionary<string, FSMNode>();
    }

    #region SaveMethods
    public static void Save()
    {
        CreateStaticFolders();
        GetElementsFromGraphView();
        FSMGraphSaveData graphSaveData = CreateAsset<FSMGraphSaveData>("Assets/EditorWindow/FSMSystem/Graphs", _graphName);
        graphSaveData.Initialize(_graphName);
        FSMNodesContainerSO nodesContainer = CreateAsset<FSMNodesContainerSO>(_containerFolderPath, _graphName);
        nodesContainer.Initialize(_graphName);
        
        SaveGroups(graphSaveData, nodesContainer);
        SaveNodes(graphSaveData, nodesContainer);

        SaveAsset(graphSaveData);
        SaveAsset(nodesContainer);
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
            SaveNodeToGraph(node, graphSaveData);
            SaveNodeToScriptableObject(node, nodesContainer);
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
    private static void SaveNodeToGraph(FSMNode node, FSMGraphSaveData graphSaveData)
    {
        List<FSMConnectionSaveData> connections = CloneNodeConnections(node.Choices);

        FSMNodeSaveData nodeSaveData = new FSMNodeSaveData()
        {
            Id = node.Id,
            Name = node.StateName,
            Connections = connections,
            GroupId = node.Group?.Id,
            DialogueType = node.DialogueType,
            Position = node.GetPosition().position,
            ScriptableObject = node.StateScriptableObject
        };
        graphSaveData.Nodes.Add(nodeSaveData);
    }
    private static void SaveNodeToScriptableObject(FSMNode node, FSMNodesContainerSO nodesContainer)
    {
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
            node.DialogueType,
            node.StateScriptableObject
        );
        _createdNodes.Add(node.Id, nodeSo);
        SaveAsset(nodeSo);
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
        FSMEditorWindow.UpdateFileName(graphSaveData.FileName);
        LoadGroups(graphSaveData.Groups);
        LoadNodes(graphSaveData.Nodes);
        LoadConnections();
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

            FSMNode node = _graphView.CreateNode(nodeData.Name, nodeData.Position, nodeData.DialogueType, false);
            node.Id = nodeData.Id;
            node.Choices = connections;
            node.StateScriptableObject = nodeData.ScriptableObject;

            for (int i = 0; i < nodeData.ScriptableObject.GetVariablesValues().Count; i++)
            {
                //Debug.Log(node.StateScriptableObject.GetVariablesValues()[i]);
            }

            //node.StateName = nodeData.Name;
            //node.DialogueType = nodeData.DialogueType;
            //node.Group = _loadedGroups[nodeData.GroupId];
            
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