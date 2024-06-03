#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Utilities;
    using Windows;
    using FSM.Enumerations;

    /// <summary>
    /// Represents an FSM extension node, inheriting from <see cref="FsmNode"/>. This type of node is used to draw more comprehensive connections between states.
    /// </summary>
    public class FsmExtensionNode : FsmNode
    {
        /// <summary>
        /// Initializes the FSM extension node.
        /// </summary>
        /// <param name="nodeName">The name of the node.</param>
        /// <param name="fsmGraphView">The graph view this node belongs to.</param>
        /// <param name="vectorPos">The position of the node in the graph.</param>
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Extension;

            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Extension Node",
            };

            Connections.Add(connectionSaveData);

            mainContainer.AddToClassList("fsm-node_main-container-extension");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }

        /// <summary>
        /// Draws the node, adding the input and output ports and styling the containers. This draw does not inherit from the base class, as the extension node has a different layout.
        /// </summary>
        public override void Draw()
        {
            mainContainer.Remove(titleContainer);

            InputPort = this.CreatePort();
            inputContainer.Add(InputPort);
            inputContainer.AddToClassList("fsm-node_input-output-container-extension");
            
            foreach (var connection in Connections)
            {
                var outputPort = this.CreatePort("", Orientation.Horizontal, Direction.Output);

                outputPort.userData = connection;

                OutputPort.Add(outputPort);
                outputContainer.Add(outputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container-extension");
            }
            
            mainContainer.Add(inputContainer);
            mainContainer.Add(outputContainer);
            mainContainer.AddToClassList("fsm-node_main-container");

            RefreshExpandedState();
        }

        /// <summary>
        /// Sets the name of the state.
        /// </summary>
        /// <param name="stateName">The new name of the state.</param>
        public override void SetStateName(string stateName)
        {
            StateName = stateName;
        }
    }
}
#endif
