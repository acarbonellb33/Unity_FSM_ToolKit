#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Data.Error
{
    using UnityEngine;

    /// <summary>
    /// Represents data for FSM errors.
    /// </summary>
    public class FsmErrorData
    {
        /// <summary>
        /// Gets the color associated with the FSM error.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FsmErrorData"/> class.
        /// </summary>
        public FsmErrorData()
        {
            GenerateRandomColor();
        }

        /// <summary>
        /// Generates a random color for the FSM error.
        /// </summary>
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

