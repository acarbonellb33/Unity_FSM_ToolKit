#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Elements
{
    using Data.Save;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;
    using Utilities;
    using Windows;
    using FSM.Enumerations;
    public class FsmExtensionNode : FsmNode
    {
        public override void Initialize(string nodeName, FsmGraphView fsmGraphView, Vector2 vectorPos)
        {
            base.Initialize(nodeName, fsmGraphView, vectorPos);
            NodeType = FsmNodeType.Extension;
            var connectionSaveData = new FsmConnectionSaveData()
            {
                Text = "Extension Node",
            };
            Choices.Add(connectionSaveData);
            mainContainer.AddToClassList("fsm-node_main-container-extension");
            extensionContainer.AddToClassList("fsm-node_extension-container");
        }

        public override void Draw()
        {
            mainContainer.Remove(titleContainer);

            InputPort = this.CreatePort();
            inputContainer.Add(InputPort);
            inputContainer.AddToClassList("fsm-node_input-output-container-extension");
            
            foreach (var connection in Choices)
            {
                OutputPort = this.CreatePort("", Orientation.Horizontal, Direction.Output);
                OutputPort.userData = connection;
                outputContainer.Add(OutputPort);
                outputContainer.AddToClassList("fsm-node_input-output-container-extension");
            }

            mainContainer.Add(inputContainer);
            mainContainer.Add(outputContainer);

            mainContainer.AddToClassList("fsm-node_main-container");

            RefreshExpandedState();
        }

        public override void SetStateName(string stateName)
        {
            StateName = stateName;
        }
    }
}
#endif