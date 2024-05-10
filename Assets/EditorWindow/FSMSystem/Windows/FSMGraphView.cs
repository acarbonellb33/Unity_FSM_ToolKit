using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMGraphView : GraphView
{
    private FSMSearchWindow _searchWindow;
    private FSMEditorWindow _window;

    private MiniMap _miniMap;

    public SerializableDictionary<string, FSMNodeErrorData> _ungroupedNodes;
    private SerializableDictionary<string, FSMGroupErrorData> _groups;
    public SerializableDictionary<Group, SerializableDictionary<string, FSMNodeErrorData>> _groupedNodes;

    private int repeatedNameCount;

    public int RepeatedNameCount
    {
        get { return repeatedNameCount; }
        set
        {
            repeatedNameCount = value;
            if (repeatedNameCount == 0)
            {
                _window.EnableSaving();
            }
            else
            {
                _window.DisableSaving();
            }
        }
    }

    public FSMGraphView(FSMEditorWindow window)
    {
        _window = window;
        _ungroupedNodes = new SerializableDictionary<string, FSMNodeErrorData>();
        _groups = new SerializableDictionary<string, FSMGroupErrorData>();
        _groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, FSMNodeErrorData>>();

        AddManipulators();
        AddSearchWindow();
        AddMiniMap();
        AddGridBackground();
        OnElementsDeleted();
        OnGroupElementsAdded();
        OnGroupElementsRemoved();
        OnGroupRenamed();
        OnGraphViewChanged();
        AddStyles();
        AddMiniMapStyles();
        
        RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        
        //AddElement(CreateNode("Initial State", new Vector2(100, 45), FSMNodeType.Initial));
    }
    
    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.MiddleMouse && evt.target is Edge edge)
        {
            // Get the ports connected by the edge
            Port inputPort = edge.input;
            Port outputPort = edge.output;

            // Calculate position for the new node between the ports
            Vector2 newNodePosition = (inputPort.GetGlobalCenter() + outputPort.GetGlobalCenter()) / 2f;
            // Convert the position to local coordinates of the graph view
            Vector2 localNewNodePosition = this.contentViewContainer.WorldToLocal(newNodePosition);


            // Create and add the new node
            FSMNode newNode = CreateNode("Extension", localNewNodePosition, FSMNodeType.Extension);

            FSMNode node = (FSMNode)outputPort.node;
            
            FSMConnectionSaveData choiceData = new FSMConnectionSaveData();
            string id = newNode.Id;
            string text = newNode.StateName;
            
            choiceData.NodeId = id;
            choiceData.Text = text;
            
            FSMConnectionSaveData choiceData2 = new FSMConnectionSaveData();
            string id2 = ((FSMNode)inputPort.node).Id;
            string text2 = ((FSMNode)inputPort.node).StateName;
            
            choiceData2.NodeId = id2;
            choiceData2.Text = text2;
            
            node.Choices = new List<FSMConnectionSaveData>();
            node.Choices.Add(choiceData);
            
            newNode.Choices = new List<FSMConnectionSaveData>();
            newNode.Choices.Add(choiceData2);

            AddElement(newNode);

            // Connect the new node to the ports
            ConnectPorts(outputPort, newNode.inputContainer[0] as Port);
            ConnectPorts(newNode.outputContainer[0] as Port, inputPort);
            
            // Remove the edge
            RemoveElement(edge);
            //CheckLoopConditions();
            
            MarkDirtyRepaint();
        }
    }
    
    private void ConnectPorts(Port fromPort, Port toPort)
    {
        Edge edge = new Edge
        {
            output = fromPort,
            input = toPort
        };
        
        edge.input.DisconnectAll();
        edge.output.DisconnectAll();
        edge.input.Connect(edge);
        edge.output.Connect(edge);
        
        edge.input.portColor = Color.white;
        edge.output.portColor = Color.white;
        
        AddElement(edge);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        ports.ForEach((port) =>
        {
            if (startPort == port) return;
            if (startPort.node == port.node) return;
            if (startPort.direction == port.direction) return;
            if (startPort.node.GetType() == typeof(FSMStateNode) && port.node.GetType() == typeof(FSMStateNode)) return;
            compatiblePorts.Add(port);
        });
        return compatiblePorts;
    }

    public FSMNode CreateNode(string nodeName, Vector2 position, FSMNodeType nodeT, bool fixName = true, bool shouldDraw = true)
    {
        Type nodeType = Type.GetType($"FSM{nodeT}Node");
        FSMNode node = (FSMNode)Activator.CreateInstance(nodeType);

        if (fixName)
        {
            if(nodeT == FSMNodeType.Transition || nodeT == FSMNodeType.DualTransition || nodeT == FSMNodeType.Extension)
            {
                int count = 0;
                
                SortedDictionary<string, FSMNodeErrorData> sortedDict = new SortedDictionary<string, FSMNodeErrorData>(_ungroupedNodes);

                foreach (var key in sortedDict.Keys)
                {
                    if (key.StartsWith(nodeName.Split(" ")[0], StringComparison.OrdinalIgnoreCase))
                    {
                        nodeName = nodeName.Split(" ")[0];
                        _ungroupedNodes.UpdateKey(key, nodeName+ " " + count);
                        GetNodeFromGraph(key).SetStateName(nodeName+ " " + count);
                        count++;
                        nodeName = nodeName + " " + count;
                    }
                }
            }
        }
        
        node.Initialize(nodeName, this, position);

        if (shouldDraw)
        {
            node.Draw();
        }

        if (nodeT == FSMNodeType.State) FSMEditorWindow._stateNames.Add(nodeName);
        AddUngroupedNode(node);
        StartDelayedCheckLoopConditions();
        return node;
    }

    public FSMGroup CreateGroup(string title, Vector2 localMousePosition)
    {
        FSMGroup group = new FSMGroup(title, localMousePosition);
        AddGroup(group);
        AddElement(group);
        foreach (GraphElement selectedElement in selection)
        {
            if (!(selectedElement is FSMNode))
            {
                continue;
            }

            FSMNode node = (FSMNode)selectedElement;
            group.AddElement(node);
        }

        return group;
    }

    public void AddUngroupedNode(FSMNode node)
    {
        string nodeName = node.StateName;
        if (!_ungroupedNodes.ContainsKey(nodeName))
        {
            FSMNodeErrorData nodeErrorData = new FSMNodeErrorData();
            nodeErrorData.Nodes.Add(node);
            _ungroupedNodes.Add(nodeName, nodeErrorData);
            return;
        }

        List<FSMNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Add(node);

        Color errorColor = _ungroupedNodes[nodeName].ErrorData.Color;
        node.SetErrorStyle(errorColor);
        
        if (ungroupedNodesList.Count == 2)
        {
            ++RepeatedNameCount;
            ungroupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveUngroupedNode(FSMNode node)
    {
        string nodeName = node.StateName;
        List<FSMNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

        ungroupedNodesList.Remove(node);
        node.ResetStyle();

        if (ungroupedNodesList.Count == 1)
        {
            --RepeatedNameCount;
            ungroupedNodesList[0].ResetStyle();
            return;
        }

        if (ungroupedNodesList.Count == 0)
        {
            _ungroupedNodes.Remove(nodeName);
            if(node.NodeType == FSMNodeType.Transition || node.NodeType == FSMNodeType.DualTransition)
            {
                int count = 0;
                foreach (string stateName in _ungroupedNodes.Keys)
                {
                    if (stateName.Split(" ")[0] == nodeName.Split(" ")[0])
                    {
                        nodeName = nodeName.Split(" ")[0];
                        
                        _ungroupedNodes.UpdateKey(stateName, nodeName + " " + count);
                        GetNodeFromGraph(stateName).SetStateName(nodeName + " " + count);
                        count++;
                    }
                }
            }
        }
    }
    
    private FSMNode GetNodeFromGraph(string nodeName)
    {
        foreach (GraphElement element in graphElements)
        {
            if (element is FSMNode node)
            {
                if (node.StateName == nodeName)
                {
                    return node; 
                }
            }
        }
        Debug.Log("Node Name: " + nodeName);
        return null;
    }
    
    private void DeleteUnconnectedExtensionNodes()
    {
        // Iterate through all elements in the graph
        foreach (VisualElement element in graphElements)
        {
            // Check if the element is a node
            if (element is FSMNode node)
            {
                // Check if the node is an Extension node
                if (node.NodeType is FSMNodeType.Extension)
                {
                    // Check if the Extension node has unconnected ports
                    if (HasUnconnectedPorts(node))
                    {
                        // Delete the Extension node
                        RemoveUngroupedNode(node);
                        node.DisconnectAllPorts();
                        RemoveElement(node);
                    }
                }
            }
        }
    }

    private static bool HasUnconnectedPorts(FSMNode extensionNode)
    {
        // Check all input ports
        foreach (Port inputPort in extensionNode.inputContainer.Children())
        {
            // Check if the input port is unconnected
            if (!inputPort.connected)
            {
                return true; // Return true if an unconnected port is found
            }
        }

        // Check all output ports
        foreach (Port outputPort in extensionNode.outputContainer.Children())
        {
            // Check if the output port is unconnected
            if (!outputPort.connected)
            {
                return true; // Return true if an unconnected port is found
            }
        }

        return false; // Return false if no unconnected ports are found
    }

    private void OnElementsDeleted()
    {
        deleteSelection = (operationName, askUser) =>
        {
            Type groupType = typeof(FSMGroup);
            Type edgeType = typeof(Edge);

            List<FSMGroup> groupsToDelete = new List<FSMGroup>();
            List<Edge> edgesToDelete = new List<Edge>();
            Dictionary<FSMNode, bool> nodesToDeleteDict = new Dictionary<FSMNode, bool>();
            List<FSMNode> nodesToDelete = new List<FSMNode>();
            
            foreach (GraphElement element in selection)
            {
                if (element is FSMNode node && node.NodeType != FSMNodeType.Initial)
                {
                    if (element is FSMStateNode n)
                    {
                        FSMEditorWindow._stateNames.Remove(n.StateName);
                    }

                    if (node.NodeType == FSMNodeType.Extension)
                    { 
                        if(nodesToDeleteDict.Keys.Contains(node)) continue;
                        FSMNode nodeToDelete = ReconnectNodes(node);
                        nodesToDeleteDict.Add(nodeToDelete, true);
                    }
                    else
                    {
                        nodesToDeleteDict.Add(node, false);
                    }
                    
                    continue;
                }

                if (element.GetType() == edgeType)
                {
                    bool hasBeenHandled = false;
                    Edge edge = (Edge)element;
                    if (((FSMNode)edge.input.node).NodeType == FSMNodeType.Extension)
                    {
                        if(nodesToDeleteDict.Keys.Contains((FSMNode)edge.input.node)) continue;
                        FSMNode nodeToDelete = ReconnectNodes((FSMNode)edge.input.node);
                        nodesToDeleteDict.Add(nodeToDelete, true);
                        hasBeenHandled = true;
                    }
                    if (((FSMNode)edge.output.node).NodeType == FSMNodeType.Extension)
                    {
                        if(nodesToDeleteDict.Keys.Contains((FSMNode)edge.output.node)) continue;
                        FSMNode nodeToDelete = ReconnectNodes((FSMNode)edge.output.node);
                        nodesToDeleteDict.Add(nodeToDelete, true);
                        hasBeenHandled = true;
                    }
                    if (hasBeenHandled)continue;
                    edgesToDelete.Add((Edge)element);
                    continue;
                }

                if (element.GetType() != groupType)
                {
                    continue;
                }

                FSMGroup group = (FSMGroup)element;
                groupsToDelete.Add(group);
            }
            
            Debug.Log("Nodes to Delete: " + nodesToDeleteDict.Count);

            foreach (FSMGroup group in groupsToDelete)
            {
                List<FSMNode> groupNodes = new List<FSMNode>();
                foreach (GraphElement element in group.containedElements)
                {
                    if (!(element is FSMNode))
                    {
                        continue;
                    }

                    FSMNode node = (FSMNode)element;
                    groupNodes.Add(node);
                }

                group.RemoveElements(groupNodes);
                RemoveGroup(group);
                RemoveElement(group);
            }
            
            
            foreach (KeyValuePair<FSMNode, bool> node in nodesToDeleteDict)
            {
                if (node.Key.Group != null)
                {
                    node.Key.Group.RemoveElement(node.Key);
                }
                if(node.Value)
                {
                    FSMNode nodeToDelete = node.Key.inputContainer.Children().OfType<Port>().ToList()[0].connections.ToList()[0].output.node as FSMNode;
                    
                    string nextNodeId = nodeToDelete.Choices[0].NodeId;
                    string nextNodeName = nodeToDelete.Choices[0].Text;
                    
                    FSMConnectionSaveData choiceData = new FSMConnectionSaveData();
                    choiceData.NodeId = nextNodeId;
                    choiceData.Text = nextNodeName;
                    
                    RemoveUngroupedNode(node.Key);
                    node.Key.DisconnectAllPorts();
                    RemoveElement(node.Key);
                    
                    nodeToDelete.Choices = new List<FSMConnectionSaveData>();
                    nodeToDelete.Choices.Add(choiceData);
                }
                else {
                    RemoveUngroupedNode(node.Key);
                    node.Key.DisconnectAllPorts();
                    RemoveElement(node.Key);
                }
            }


            foreach (Edge edge in edgesToDelete)
            {
                Debug.Log("Edge Deleted");
                if (edge.input == null || edge.output == null)
                {
                    RemoveElement(edge);
                }
                else
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    RemoveElement(edge);
                }
            }
        };
    }
    
    private FSMNode ReconnectNodes(FSMNode startNode)
    {
        var inputConnections = startNode.inputContainer.Children().ToList();
        
        Port previousNode = (inputConnections[0] as Port).connections.ToList()[0].output;
        
        FSMConnectionSaveData choiceData = new FSMConnectionSaveData();
        string nextNodeId = startNode.Choices[0].NodeId;
        string nextNodeName = startNode.Choices[0].Text;
        
        choiceData.NodeId = nextNodeId;
        choiceData.Text = nextNodeName;
        
        FSMNode nextPreviousNode = (FSMNode)previousNode.node;
        nextPreviousNode.Choices = new List<FSMConnectionSaveData>();
        nextPreviousNode.Choices.Add(choiceData);

        Port prePort = nextPreviousNode.outputContainer.Children().OfType<Port>().ToList()[0];
        FSMNode nextNewNode = GetNodeFromGraphById(startNode.Choices[0].NodeId);
        
        Port nextPort = nextNewNode.inputContainer.Children().OfType<Port>().ToList()[0];
        
        ConnectPorts(prePort, nextPort);
        MarkDirtyRepaint();
        return startNode;
    }
    
    
    
    private FSMNode GetNodeFromGraphById(string nodeId)
    {
        foreach (GraphElement element in graphElements)
        {
            if (element is FSMNode node)
            {
                if (node.Id == nodeId)
                {
                    return node; 
                }
            }
        }
        return null;
    }
    
    private void CheckLoopConditions()
    {
        // Perform depth-first search (DFS) traversal starting from each node
        Dictionary<int, HashSet<FSMNode>> nodeLoopConnections = new Dictionary<int, HashSet<FSMNode>>();

        int count = 0;
        foreach (var element in graphElements)
        {
            if (element is FSMNode startNode && (startNode.NodeType == FSMNodeType.Transition || startNode.NodeType == FSMNodeType.DualTransition || startNode.NodeType == FSMNodeType.Extension))
            {
                startNode.inputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                {
                    port.connections.ToList().ForEach(edge =>
                    {
                        edge.input.portColor = Color.white;
                        AddToSelection(edge);
                        RemoveFromSelection(edge);
                    });
                });
                
                startNode.outputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                {
                    port.connections.ToList().ForEach(edge =>
                    {
                        edge.output.portColor = Color.white;
                        AddToSelection(edge);
                        RemoveFromSelection(edge);
                    });
                });
                
                HashSet<FSMNode> visited = new HashSet<FSMNode>(); // Track visited nodes
                visited = DFS(startNode, visited);
                if (visited.Count > 0)
                {
                    nodeLoopConnections.Add(count, visited);
                    count++;
                }
            }
            AddToSelection(element);
            RemoveFromSelection(element);
        }

        // Highlight the nodes that are part of a loop in the dictionary
        foreach(var item in nodeLoopConnections)
        {
            foreach (var element in graphElements)
            {
                if (item.Value.Contains(element))
                {
                    ((FSMNode)element).inputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                    {
                        port.connections.ToList().ForEach(edge =>
                        {
                            edge.input.portColor = Color.yellow;
                            AddToSelection(edge);
                            RemoveFromSelection(edge);
                        });
                    });
                
                    ((FSMNode)element).outputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                    {
                        port.connections.ToList().ForEach(edge =>
                        {
                            edge.output.portColor = Color.yellow;
                            AddToSelection(edge);
                            RemoveFromSelection(edge);
                        });
                    });
                }
                
                AddToSelection(element);
                RemoveFromSelection(element);
            }
        }
        if(nodeLoopConnections.Count > 0) _window.DisableSaving();
        else _window.EnableSaving();
        MarkDirtyRepaint();
    }

    private HashSet<FSMNode> DFS(FSMNode currentNode, HashSet<FSMNode> visited)
    {
        if (visited.Contains(currentNode))
        {
            // Detected a loop
            return visited;
        }
        
        // Mark current node as visited and add to current path
        visited.Add(currentNode);

        // Recursively traverse each connected node
        foreach (var port in currentNode.outputContainer.Children().OfType<Port>())
        {
            foreach (var edge in port.connections)
            {
                var nextNode = edge.input.node as FSMNode;
                if(nextNode != null)
                {
                    if (nextNode.NodeType != FSMNodeType.Transition && nextNode.NodeType != FSMNodeType.DualTransition && nextNode.NodeType != FSMNodeType.Extension)
                    {
                        return new HashSet<FSMNode>();
                    }
                    return DFS(nextNode, visited);
                }
            }
        }
        return new HashSet<FSMNode>();
    }
    
    private void OnGraphViewChanged()
    {
        graphViewChanged = (changes) =>
        {
            if (changes.edgesToCreate != null)
            {
                foreach (Edge edge in changes.edgesToCreate)
                {
                    FSMNode nextNode = (FSMNode)edge.input.node;
                    nextNode.SetPortColor(Color.white, Direction.Input);
                    AddToSelection(nextNode);
                    RemoveFromSelection(nextNode);
                    
                    FSMConnectionSaveData choiceData = (FSMConnectionSaveData)edge.output.userData;
                    FSMNode previousNode = (FSMNode)edge.output.node;
                    previousNode.SetPortColor(Color.white, Direction.Output);
                    AddToSelection(previousNode);
                    RemoveFromSelection(previousNode);
                    
                    choiceData.NodeId = nextNode.Id;

                    if (choiceData.Text == "Initial Node")
                    {
                        _window.initialState = nextNode.StateName;
                    }
                }
            }
            if (changes.elementsToRemove != null)
            {
                Type edgeType = typeof(Edge);

                foreach (GraphElement element in changes.elementsToRemove)
                {
                    if (element.GetType() != edgeType)
                    {
                        continue;
                    }
                    
                    Edge edge = (Edge)element;
                    FSMNode nextNode = (FSMNode)edge.input.node;
                    if (edge.input.connections.Count() == 0)
                    {
                        nextNode.SetPortColor(Color.red, Direction.Input);
                        AddToSelection(nextNode);
                        RemoveFromSelection(nextNode);
                    }
                    
                    FSMNode previousNode = (FSMNode)edge.output.node;
                    if (edge.output.connections.Count() == 0)
                    {
                       previousNode.SetPortColor(Color.red, Direction.Output); 
                       AddToSelection(previousNode);
                       RemoveFromSelection(previousNode);
                    }

                    FSMConnectionSaveData choiceData = (FSMConnectionSaveData)edge.output.userData;
                    choiceData.NodeId = "";
                    
                    
                    if (choiceData.Text == "Initial Node")
                    {
                        _window.initialState = "";
                    }
                    
                }
            }
            StartDelayedCheckLoopConditions();
            return changes;
        };
    }
    
    private async void StartDelayedCheckLoopConditions()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.01f)); // Adjust the delay as needed
        DeleteUnconnectedExtensionNodes();
        CheckLoopConditions();
    }

    #region Groups Methods

    private void OnGroupElementsAdded()
    {
        elementsAddedToGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is FSMNode))
                {
                    continue;
                }

                FSMGroup newGroup = (FSMGroup)group;
                FSMNode node = (FSMNode)element;
                RemoveUngroupedNode(node);
                AddGroupedNode(node, newGroup);
            }
        };
    }

    private void OnGroupElementsRemoved()
    {
        elementsRemovedFromGroup = (group, elements) =>
        {
            foreach (GraphElement element in elements)
            {
                if (!(element is FSMNode))
                {
                    continue;
                }

                FSMNode node = (FSMNode)element;
                RemoveGroupedNode(node, group);
                AddUngroupedNode(node);
            }
        };
    }

    private void OnGroupRenamed()
    {
        groupTitleChanged = (group, title) =>
        {
            FSMGroup fsmGroup = (FSMGroup)group;
            RemoveGroup(fsmGroup);
            fsmGroup.PreviousTitle = title;
            AddGroup(fsmGroup);
        };
    }
    public void AddGroupedNode(FSMNode node, FSMGroup group)
    {
        string nodeName = node.StateName;
        node.Group = group;
        if (!_groupedNodes.ContainsKey(group))
        {
            _groupedNodes.Add(group, new SerializableDictionary<string, FSMNodeErrorData>());
        }

        if (!_groupedNodes[group].ContainsKey(nodeName))
        {
            FSMNodeErrorData nodeErrorData = new FSMNodeErrorData();
            nodeErrorData.Nodes.Add(node);
            _groupedNodes[group].Add(nodeName, nodeErrorData);
            return;
        }

        List<FSMNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Add(node);
        Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;
        node.SetErrorStyle(errorColor);
        if (groupedNodesList.Count == 2)
        {
            ++RepeatedNameCount;
            groupedNodesList[0].SetErrorStyle(errorColor);
        }
    }

    public void RemoveGroupedNode(FSMNode node, Group group)
    {
        string nodeName = node.StateName;
        node.Group = null;
        List<FSMNode> groupedNodesList = _groupedNodes[group][nodeName].Nodes;

        groupedNodesList.Remove(node);
        node.ResetStyle();

        if (groupedNodesList.Count == 1)
        {
            --RepeatedNameCount;
            groupedNodesList[0].ResetStyle();
            return;
        }

        if (groupedNodesList.Count == 0)
        {
            _groupedNodes[group].Remove(nodeName);
            if (_groupedNodes[group].Count == 0)
            {
                _groupedNodes.Remove(group);
            }
        }
    }

    private void AddGroup(FSMGroup group)
    {
        string groupName = group.title;
        if (!_groups.ContainsKey(groupName))
        {
            FSMGroupErrorData groupErrorData = new FSMGroupErrorData();
            groupErrorData.Groups.Add(group);
            _groups.Add(groupName, groupErrorData);
            return;
        }

        List<FSMGroup> groupsList = _groups[groupName].Groups;
        groupsList.Add(group);
        Color errorColor = _groups[groupName].ErrorData.Color;
        group.SetErrorStyle(errorColor);
        if (groupsList.Count == 2)
        {
            ++RepeatedNameCount;
            groupsList[0].SetErrorStyle(errorColor);
        }
    }

    private void RemoveGroup(FSMGroup group)
    {
        string groupName = group.PreviousTitle;
        List<FSMGroup> groupsList = _groups[groupName].Groups;
        groupsList.Remove(group);
        group.ResetStyle();
        if (groupsList.Count == 1)
        {
            --RepeatedNameCount;
            groupsList[0].ResetStyle();
            return;
        }

        if (groupsList.Count == 0)
        {
            _groups.Remove(groupName);
        }
    }

    #endregion

    #region Utilities

    private void AddMiniMap()
    {
        _miniMap = new MiniMap()
        {
            anchored = true
        };
        _miniMap.SetPosition(new Rect(10, 45, 200, 140));
        Add(_miniMap);

        _miniMap.visible = false;
    }

    private void AddGridBackground()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }

    private void AddStyles()
    {
        this.AddStyleSheets("FSMSystem/FSMGraphViewStyle.uss", "FSMSystem/FSMNodeStyle.uss");
    }

    private void AddMiniMapStyles()
    {
        StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
        StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

        _miniMap.style.backgroundColor = backgroundColor;
        _miniMap.style.borderBottomColor = borderColor;
        _miniMap.style.borderLeftColor = borderColor;
        _miniMap.style.borderRightColor = borderColor;
        _miniMap.style.borderTopColor = borderColor;
    }

    private void AddManipulators()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        this.AddManipulator(CreateStateItemMenu("Attack"));
        this.AddManipulator(CreateStateItemMenu("Patrol"));
        this.AddManipulator(CreateStateItemMenu("Chase"));
        this.AddManipulator(CreateStateItemMenu("Search"));
        this.AddManipulator(CreateTransitionItemMenu("Hearing"));
        this.AddManipulator(CreateTransitionItemMenu("Seeing"));
        this.AddManipulator(CreateTransitionItemMenu("Distance"));
        this.AddManipulator(CreateTransitionItemMenu("Health"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Hearing"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Seeing"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Distance"));
        this.AddManipulator(CreateDualTransitionStateItemMenu("Health"));
        this.AddManipulator(CreateExtensionNodeMenu("Extension Node"));
        this.AddManipulator(CreateGroupContextualMenu());
  		this.AddManipulator(CreateTransitionItemMenu("NextState"));
		this.AddManipulator(CreateDualTransitionStateItemMenu("NextState"));
        this.AddManipulator(CreateCustomStateItemMenu("Custom"));
    }

    private void AddSearchWindow()
    {
        if (_searchWindow == null)
        {
            _searchWindow = ScriptableObject.CreateInstance<FSMSearchWindow>();
            _searchWindow.Initialize(this);
        }

        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    private IManipulator CreateGroupContextualMenu()
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Create Group",
                menuActionEvent => CreateGroup("State Group",
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition)))
        );

        return contextualMenuManipulator;
    }

    private IManipulator CreateStateItemMenu(string nodeName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create State/{nodeName}", menuActionEvent =>
                AddElement(CreateNode(nodeName, GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                    FSMNodeType.State))));

        return contextualMenuManipulator;
    }
    
    private IManipulator CreateCustomStateItemMenu(string nodeName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Custom State", menuActionEvent =>
                AddElement(CreateNode(nodeName, GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                    FSMNodeType.CustomState))));

        return contextualMenuManipulator;
    }

    private IManipulator CreateTransitionItemMenu(string transitionName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Transition/{transitionName}", menuActionEvent =>
                AddElement(CreateNode(transitionName,
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FSMNodeType.Transition))));

        return contextualMenuManipulator;
    }
    private IManipulator CreateDualTransitionStateItemMenu(string transitionName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Dual Transition/{transitionName}", menuActionEvent =>
                AddElement(CreateNode(transitionName,
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FSMNodeType.DualTransition))));

        return contextualMenuManipulator;
    }
    private IManipulator CreateExtensionNodeMenu(string transitionName)
    {
        ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction($"Create Extension Node", menuActionEvent =>
                AddElement(CreateNode(transitionName,
                    GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FSMNodeType.Extension))));

        return contextualMenuManipulator;
    }
    public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
    {
        Vector2 worldMousePosition = mousePosition;
        if (isSearchWindow)
        {
            worldMousePosition -= _window.position.position;
        }

        Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);
        return localMousePosition;
    }

    public void ClearGraph()
    {
        graphElements.ForEach(graphElement => RemoveElement(graphElement));
        _groups.Clear();
        _groupedNodes.Clear();
        _ungroupedNodes.Clear();

        repeatedNameCount = 0;
    }

    public void ToggleMiniMap()
    {
        _miniMap.visible = !_miniMap.visible;
    }

    public FSMEditorWindow GetWindow()
    {
        return _window;
    }
    
    private void AddAllNodesToSelection()
    {
        foreach (var element in graphElements)
        {
            if (element is FSMNode node)
            {
                AddToSelection(node);
            }
        }
    }
    
    private void RemoveAllNodesFromSelection()
    {
        foreach (var element in graphElements)
        {
            if (element is FSMNode node)
            {
                RemoveFromSelection(node);
            }
        }
    }

    #endregion
}
