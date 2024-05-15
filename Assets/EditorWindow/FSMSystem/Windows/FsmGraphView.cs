#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Data.Error;
    using Data.Save;
    using Elements;
    using Utilities;
    using FSM.Enumerations;
    using FSM.Utilities;
    public class FsmGraphView : GraphView
    {
        private FsmSearchWindow _searchWindow;
        private readonly FsmEditorWindow _window;

        private MiniMap _miniMap;

        private readonly SerializableDictionary<string, FsmNodeErrorData> _ungroupedNodes;

        private int _repeatedNameCount;

        private int RepeatedNameCount
        {
            get => _repeatedNameCount;
            set
            {
                _repeatedNameCount = value;
                if (_repeatedNameCount == 0)
                    _window.EnableSaving();
                else
                    _window.DisableSaving();
            }
        }

        public FsmGraphView(FsmEditorWindow window)
        {
            _window = window;
            _ungroupedNodes = new SerializableDictionary<string, FsmNodeErrorData>();

            AddManipulators();
            AddSearchWindow();
            AddMiniMap();
            AddGridBackground();
            OnElementsDeleted();
            OnGraphViewChanged();
            AddStyles();
            AddMiniMapStyles();

            RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == (int)MouseButton.MiddleMouse && evt.target is Edge edge)
            {
                Port inputPort = edge.input;
                Port outputPort = edge.output;
                
                Vector2 mousePosition = contentViewContainer.WorldToLocal(evt.mousePosition);
                
                FsmNode newNode = CreateNode("Extension", mousePosition, FsmNodeType.Extension);
                FsmNode node = (FsmNode)outputPort.node;

                FsmConnectionSaveData choiceData = new FsmConnectionSaveData();
                string id = newNode.Id;
                string text = newNode.StateName;

                choiceData.NodeId = id;
                choiceData.Text = text;

                FsmConnectionSaveData choiceData2 = new FsmConnectionSaveData();
                string id2 = ((FsmNode)inputPort.node).Id;
                string text2 = ((FsmNode)inputPort.node).StateName;

                choiceData2.NodeId = id2;
                choiceData2.Text = text2;

                node.Choices.Clear();
                node.Choices.Add(choiceData);

                newNode.Choices.Clear();
                newNode.Choices.Add(choiceData2);

                AddElement(newNode);
                
                ConnectPorts(outputPort, newNode.inputContainer[0] as Port);
                ConnectPorts(newNode.outputContainer[0] as Port, inputPort);
                
                RemoveElement(edge);
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
                if (startPort.node.GetType() == typeof(FsmStateNode) &&
                    port.node.GetType() == typeof(FsmStateNode)) return;
                compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        #region Nodes
        public FsmNode CreateNode(string nodeName, Vector2 position, FsmNodeType nodeT, bool fixName = true,
            bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"EditorWindow.FSMSystem.Elements.Fsm{nodeT}Node");
            FsmNode node = new FsmNode();
            if (nodeType != null) node = (FsmNode)Activator.CreateInstance(nodeType);

            if (fixName)
            {
                if (nodeT == FsmNodeType.Transition || nodeT == FsmNodeType.DualTransition ||
                    nodeT == FsmNodeType.Extension)
                {
                    int count = 0;

                    SortedDictionary<string, FsmNodeErrorData> sortedDict =
                        new SortedDictionary<string, FsmNodeErrorData>(_ungroupedNodes);

                    foreach (var key in sortedDict.Keys)
                    {
                        if (key.StartsWith(nodeName.Split(" ")[0], StringComparison.OrdinalIgnoreCase))
                        {
                            nodeName = nodeName.Split(" ")[0];
                            _ungroupedNodes.UpdateKey(key, nodeName + " " + count);
                            GetNodeFromGraph(key).SetStateName(nodeName + " " + count);
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
            
            AddNode(node);
            StartDelayedCheckLoopConditions();
            return node;
        }
        private void AddNode(FsmNode node)
        {
            string nodeName = node.StateName;
            if (!_ungroupedNodes.ContainsKey(nodeName))
            {
                FsmNodeErrorData nodeErrorData = new FsmNodeErrorData();
                nodeErrorData.Nodes.Add(node);
                _ungroupedNodes.Add(nodeName, nodeErrorData);
                return;
            }

            List<FsmNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = _ungroupedNodes[nodeName].ErrorData.Color;
            node.SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++RepeatedNameCount;
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        private void RemoveNode(FsmNode node)
        {
            string nodeName = node.StateName;
            List<FsmNode> ungroupedNodesList = _ungroupedNodes[nodeName].Nodes;

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
                if (node.NodeType == FsmNodeType.Transition || node.NodeType == FsmNodeType.DualTransition)
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
        #endregion
        private FsmNode GetNodeFromGraph(string nodeName)
        {
            foreach (GraphElement element in graphElements)
            {
                if (element is FsmNode node)
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
            foreach (var element in graphElements)
            {
                // Check if the element is a node
                if (element is FsmNode node)
                {
                    // Check if the node is an Extension node
                    if (node.NodeType is FsmNodeType.Extension)
                    {
                        // Check if the Extension node has unconnected ports
                        if (HasUnconnectedPorts(node))
                        {
                            // Delete the Extension node
                            RemoveNode(node);
                            node.DisconnectAllPorts();
                            RemoveElement(node);
                        }
                    }
                }
            }
        }
        private void SetPortColorToEmptyNodes()
        {
            // Iterate through all elements in the graph
            foreach (var element in graphElements)
            {
                // Check if the element is a node
                if (element is FsmNode node)
                {
                    if (((FsmNode)element).NodeType == FsmNodeType.Initial)
                    {
                        if (node.outputContainer.Children().OfType<Port>().ToList().Count == 0) continue;
                        Port outputPortInitial = node.outputContainer.Children().OfType<Port>().ToList()[0];
                        if (!outputPortInitial.connected) outputPortInitial.portColor = Color.red;
                        MarkDirtyRepaint();
                    }

                    if (node.inputContainer.Children().OfType<Port>().ToList().Count == 0) continue;
                    if (node.outputContainer.Children().OfType<Port>().ToList().Count == 0) continue;

                    Port inputPort = node.inputContainer.Children().OfType<Port>().ToList()[0];
                    Port outputPort = node.outputContainer.Children().OfType<Port>().ToList()[0];

                    if (!inputPort.connected) inputPort.portColor = Color.red;
                    if (!outputPort.connected) outputPort.portColor = Color.red;
                    MarkDirtyRepaint();
                }
            }
        }
        private bool HasUnconnectedPorts(FsmNode extensionNode)
        {
            foreach (VisualElement childElement in extensionNode.inputContainer.Children())
            {
                if (childElement is Port inputPort)
                {
                    if (!inputPort.connected)
                    {
                        foreach (var port in extensionNode.outputContainer.Children().OfType<Port>().ToList())
                        {
                            foreach (var port2 in port.connections.ToList())
                            {
                                port2.input.portColor = Color.red;
                            }
                            MarkDirtyRepaint();
                        }
                        return true;
                    }
                }
            }
            
            foreach (VisualElement childElement in extensionNode.outputContainer.Children())
            {
                if (childElement is Port outputPort)
                {
                    if (!outputPort.connected)
                    {
                        foreach (Port port in extensionNode.inputContainer.Children().OfType<Port>().ToList())
                        {
                            foreach (Edge port2 in port.connections.ToList())
                            {
                                port2.output.portColor = Color.red;
                            }
                            MarkDirtyRepaint();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        private void OnElementsDeleted()
        {
            deleteSelection = (_, _) =>
            {
                Type edgeType = typeof(Edge);
                List<Edge> edgesToDelete = new List<Edge>();
                Dictionary<FsmNode, bool> nodesToDeleteDict = new Dictionary<FsmNode, bool>();

                foreach (var element in selection.OfType<GraphElement>())
                {
                    if (element is FsmNode node && node.NodeType != FsmNodeType.Initial)
                    {
                        if (node.NodeType == FsmNodeType.Extension)
                        {
                            if (nodesToDeleteDict.Keys.Contains(node)) continue;
                            FsmNode nodeToDelete = ReconnectNodes(node);
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
                        if (((FsmNode)edge.input.node).NodeType == FsmNodeType.Extension)
                        {
                            if (nodesToDeleteDict.Keys.Contains((FsmNode)edge.input.node)) continue;
                            FsmNode nodeToDelete = ReconnectNodes((FsmNode)edge.input.node);
                            nodesToDeleteDict.Add(nodeToDelete, true);
                            hasBeenHandled = true;
                        }

                        if (((FsmNode)edge.output.node).NodeType == FsmNodeType.Extension)
                        {
                            if (nodesToDeleteDict.Keys.Contains((FsmNode)edge.output.node)) continue;
                            FsmNode nodeToDelete = ReconnectNodes((FsmNode)edge.output.node);
                            nodesToDeleteDict.Add(nodeToDelete, true);
                            hasBeenHandled = true;
                        }

                        if (hasBeenHandled) continue;
                        edgesToDelete.Add((Edge)element);
                    }
                }
                
                foreach (KeyValuePair<FsmNode, bool> node in nodesToDeleteDict)
                {
                    if (node.Value)
                    {
                        var nodeToDelete = node.Key.inputContainer.Children().OfType<Port>().ToList()[0].connections.ToList()[0].output.node as FsmNode;
                        if (nodeToDelete == null) continue;
                        
                        var nextNodeId = nodeToDelete.Choices[0].NodeId;
                        var nextNodeName = nodeToDelete.Choices[0].Text;

                        var choiceData = new FsmConnectionSaveData
                        {
                            NodeId = nextNodeId,
                            Text = nextNodeName
                        };

                        RemoveNode(node.Key);
                        node.Key.DisconnectAllPorts();
                        RemoveElement(node.Key);

                        nodeToDelete.Choices.Clear();
                        nodeToDelete.Choices.Add(choiceData);
                    }
                    else
                    {
                        RemoveNode(node.Key);
                        node.Key.DisconnectAllPorts();
                        RemoveElement(node.Key);
                    }
                }


                foreach (var edge in edgesToDelete)
                {
                    if (edge.input == null || edge.output == null)
                    {
                        RemoveElement(edge);
                    }
                    else
                    {
                        edge.input.Disconnect(edge);
                        edge.output.Disconnect(edge);
                        RemoveElement(edge);

                        var nextNode = (FsmNode)edge.input.node;
                        var previousNode = (FsmNode)edge.output.node;

                        AddToSelection(nextNode);
                        AddToSelection(previousNode);
                        RemoveFromSelection(nextNode);
                        RemoveFromSelection(previousNode);
                    }
                }

                SetPortColorToEmptyNodes();
            };
        }
        private FsmNode ReconnectNodes(FsmNode startNode)
        {
            var inputConnections = startNode.inputContainer.Children().ToList();
            if (inputConnections.Count == 0 || !(inputConnections[0] is Port firstPort))
                return startNode;
    
            var connections = firstPort.connections.ToList();
            if (connections.Count == 0)
                return startNode;
    
            var previousNode = connections[0].output;
            var nextNodeId = startNode.Choices[0].NodeId;
            var nextNodeName = startNode.Choices[0].Text;
            
            var choiceData = new FsmConnectionSaveData
            {
                NodeId = nextNodeId,
                Text = nextNodeName
            };

            var nextPreviousNode = (FsmNode)previousNode.node;
            nextPreviousNode.Choices.Clear();
            nextPreviousNode.Choices.Add(choiceData);

            var prePort = nextPreviousNode.outputContainer.Children().OfType<Port>().ToList()[0];
            var nextNewNode = GetNodeFromGraphById(startNode.Choices[0].NodeId);

            var nextPort = nextNewNode.inputContainer.Children().OfType<Port>().ToList()[0];

            ConnectPorts(prePort, nextPort);
            MarkDirtyRepaint();
    
            return startNode;
        }
        private FsmNode GetNodeFromGraphById(string nodeId)
        {
            foreach (GraphElement element in graphElements)
            {
                if (element is FsmNode node)
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
            Dictionary<int, HashSet<FsmNode>> nodeLoopConnections = new Dictionary<int, HashSet<FsmNode>>();

            int count = 0;
            foreach (var element in graphElements)
            {
                if (element is FsmNode startNode && (startNode.NodeType == FsmNodeType.Transition ||
                                                     startNode.NodeType == FsmNodeType.DualTransition ||
                                                     startNode.NodeType == FsmNodeType.Extension))
                {
                    startNode.inputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                    {
                        port.connections.ToList().ForEach(edge =>
                        {
                            AddToSelection(edge);
                            RemoveFromSelection(edge);
                        });
                    });

                    startNode.outputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                    {
                        port.connections.ToList().ForEach(edge =>
                        {
                            AddToSelection(edge);
                            RemoveFromSelection(edge);
                        });
                    });

                    HashSet<FsmNode> visited = new HashSet<FsmNode>();
                    visited = Dfs(startNode, visited);
                    if (visited.Count > 0)
                    {
                        nodeLoopConnections.Add(count, visited);
                        count++;
                    }
                }

                AddToSelection(element);
                RemoveFromSelection(element);
            }
            
            foreach (var item in nodeLoopConnections)
            {
                foreach (var element in graphElements)
                {
                    if (item.Value.Contains(element))
                    {
                        ((FsmNode)element).inputContainer.Children().OfType<Port>().ToList().ForEach(port =>
                        {
                            port.connections.ToList().ForEach(edge =>
                            {
                                edge.input.portColor = Color.yellow;
                                AddToSelection(edge);
                                RemoveFromSelection(edge);
                            });
                        });

                        ((FsmNode)element).outputContainer.Children().OfType<Port>().ToList().ForEach(port =>
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

            if (nodeLoopConnections.Count > 0) _window.DisableSaving();
            else _window.EnableSaving();
            MarkDirtyRepaint();
        }
        private HashSet<FsmNode> Dfs(FsmNode currentNode, HashSet<FsmNode> visited)
        {
            if (visited.Contains(currentNode))
            {
                return visited;
            }

            visited.Add(currentNode);
            
            foreach (var port in currentNode.outputContainer.Children().OfType<Port>())
            {
                foreach (var edge in port.connections)
                {
                    var nextNode = edge.input.node as FsmNode;
                    if (nextNode != null)
                    {
                        if (nextNode.NodeType != FsmNodeType.Transition &&
                            nextNode.NodeType != FsmNodeType.DualTransition &&
                            nextNode.NodeType != FsmNodeType.Extension)
                        {
                            return new HashSet<FsmNode>();
                        }

                        return Dfs(nextNode, visited);
                    }
                }
            }
            return new HashSet<FsmNode>();
        }
        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (var edge in changes.edgesToCreate)
                    {
                        var nextNode = (FsmNode)edge.input.node;
                        nextNode.SetPortColor(Color.white, Direction.Input);
                        AddToSelection(nextNode);
                        RemoveFromSelection(nextNode);

                        var choiceData = (FsmConnectionSaveData)edge.output.userData;
                        var previousNode = (FsmNode)edge.output.node;
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
                    var edgeType = typeof(Edge);

                    foreach (var element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        var edge = (Edge)element;
                        var nextNode = (FsmNode)edge.input.node;
                        if (!edge.input.connections.Any())
                        {
                            nextNode.SetPortColor(Color.red, Direction.Input);
                            AddToSelection(nextNode);
                            RemoveFromSelection(nextNode);
                        }

                        var previousNode = (FsmNode)edge.output.node;
                        if (!edge.output.connections.Any())
                        {
                            previousNode.SetPortColor(Color.red, Direction.Output);
                            AddToSelection(previousNode);
                            RemoveFromSelection(previousNode);
                        }

                        var choiceData = (FsmConnectionSaveData)edge.output.userData;
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
            await Task.Delay(TimeSpan.FromSeconds(0.01f));
            DeleteUnconnectedExtensionNodes();
            CheckLoopConditions();
        }

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
            this.AddManipulator(CreateTransitionItemMenu("NextState"));
            this.AddManipulator(CreateDualTransitionStateItemMenu("NextState"));
            this.AddManipulator(CreateCustomStateItemMenu("Custom"));
        }
        private void AddSearchWindow()
        {
            if (_searchWindow == null)
            {
                _searchWindow = ScriptableObject.CreateInstance<FsmSearchWindow>();
                _searchWindow.Initialize(this);
            }

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }
        private IManipulator CreateStateItemMenu(string nodeName)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction($"Create State/{nodeName}", menuActionEvent =>
                    AddElement(CreateNode(nodeName, GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                        FsmNodeType.State))));

            return contextualMenuManipulator;
        }
        private IManipulator CreateCustomStateItemMenu(string nodeName)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction($"Create Custom State", menuActionEvent =>
                    AddElement(CreateNode(nodeName, GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                        FsmNodeType.CustomState))));

            return contextualMenuManipulator;
        }
        private IManipulator CreateTransitionItemMenu(string transitionName)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction($"Create Transition/{transitionName}", menuActionEvent =>
                    AddElement(CreateNode(transitionName,
                        GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FsmNodeType.Transition))));

            return contextualMenuManipulator;
        }
        private IManipulator CreateDualTransitionStateItemMenu(string transitionName)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction($"Create Dual Transition/{transitionName}", menuActionEvent =>
                    AddElement(CreateNode(transitionName,
                        GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition),
                        FsmNodeType.DualTransition))));

            return contextualMenuManipulator;
        }
        private IManipulator CreateExtensionNodeMenu(string transitionName)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction($"Create Extension Node", menuActionEvent =>
                    AddElement(CreateNode(transitionName,
                        GetLocalMousePosition(menuActionEvent.eventInfo.localMousePosition), FsmNodeType.Extension))));

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
            graphElements.ForEach(RemoveElement);
            _ungroupedNodes.Clear();

            _repeatedNameCount = 0;
        }
        public void ToggleMiniMap()
        {
            _miniMap.visible = !_miniMap.visible;
        }
        public FsmEditorWindow GetWindow()
        {
            return _window;
        }

        #endregion
    }
}
#endif