#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Data.Error
{
    using UnityEngine;
    public class FsmErrorData
    {
        public Color Color { get; private set; }

        public FsmErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                (byte)Random.Range(65, 256),
                (byte)Random.Range(50, 176),
                (byte)Random.Range(50, 176),
                255
            );
        }
    }
}
#endif
