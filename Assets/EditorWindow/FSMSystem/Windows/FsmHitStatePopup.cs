using EditorWindow.FSMSystem.Utilities;

#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using Data.Save;

    /// <summary>
    /// Popup window content for configuring hit state settings.
    /// </summary>
    public class FsmHitStatePopup : PopupWindowContent
    {
        private const string EnableHitStateKey = "EnableHitState";

        private bool _enableHitState;
        private float _timeToWait;
        private bool _canDie;

        /// <summary>
        /// Initializes the hit state popup with the specified hit data.
        /// </summary>
        /// <param name="hitData">The hit data to initialize the popup with.</param>
        public void Initialize(FsmHitSaveData hitData)
        {
            _enableHitState = hitData.HitEnable;
            _timeToWait = hitData.TimeToWait;
            _canDie = hitData.CanDie;
        }

        /// <summary>
        /// Gets the size of the popup window.
        /// </summary>
        /// <returns>The size of the popup window.</returns>
        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 130);
        }

        /// <summary>
        /// Renders the GUI for the popup window.
        /// </summary>
        /// <param name="rect">The position and size of the popup window.</param>
        public override void OnGUI(Rect rect)
        {
            EditorGUI.LabelField(new Rect(10, 10, rect.width - 20, 20), "Hit State Setup", EditorStyles.boldLabel);
            
            _enableHitState =
                EditorGUI.Toggle(new Rect(10, 40, rect.width - 20, 20), "Can get Hit", _enableHitState);
            
            _timeToWait = EditorGUI.FloatField(new Rect(10, 70, rect.width - 20, 20), "Time to Wait:", _timeToWait);
            
            _canDie = EditorGUI.Toggle(new Rect(10, 100, rect.width - 20, 20), "Can Die", _canDie);
            
            EditorPrefs.SetBool(EnableHitStateKey, _enableHitState);
            FsmIOUtility.UpdateHitEnableOverrides(_enableHitState);
        }

        #region Utilities
        /// <summary>
        /// Checks if hit state is enabled.
        /// </summary>
        /// <returns>True if hit state is enabled, otherwise false.</returns>
        public bool IsHitStateEnabled()
        {
            return _enableHitState;
        }

        /// <summary>
        /// Gets the time to wait for hit state.
        /// </summary>
        /// <returns>The time to wait for hit state.</returns>
        public float GetTimeToWait()
        {
            return _timeToWait;
        }

        /// <summary>
        /// Checks if the entity can die from hit state.
        /// </summary>
        /// <returns>True if the entity can die, otherwise false.</returns>
        public bool CanDie()
        {
            return _canDie;
        }
        #endregion
    }
}
#endif
