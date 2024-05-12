#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using Data.Save;
    public class FsmHitStatePopup : PopupWindowContent
    {
        private const string EnableHitStateKey = "EnableHitState";
        private const string TimeToWaitKey = "TimeToWait";
        private const string CanDieKey = "CanDie";

        private bool _enableHitState;
        private float _timeToWait;
        private bool _canDie;

        public void Initialize(FsmHitSaveData hitData)
        {
            _enableHitState = hitData.HitEnable;
            _timeToWait = hitData.TimeToWait;
            _canDie = hitData.CanDie;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 130); // Increased window size to accommodate new UI element
        }

        public override void OnGUI(Rect rect)
        {
            // Load saved values
            //enableHitState = EditorPrefs.GetBool(EnableHitStateKey, false);
            //timeToWait = EditorPrefs.GetFloat(TimeToWaitKey, 0.0f);
            //_canDie = EditorPrefs.GetBool(CanDieKey, false);

            // Display a label for Hit State Setup
            EditorGUI.LabelField(new Rect(10, 10, rect.width - 20, 20), "Hit State Setup", EditorStyles.boldLabel);

            // Toggle for "Enable Hit State"
            _enableHitState =
                EditorGUI.Toggle(new Rect(10, 40, rect.width - 20, 20), "Enable Hit State", _enableHitState);

            // FloatField for "Time to Wait"
            _timeToWait = EditorGUI.FloatField(new Rect(10, 70, rect.width - 20, 20), "Time to Wait:", _timeToWait);

            // Toggle for "Can Die"
            _canDie = EditorGUI.Toggle(new Rect(10, 100, rect.width - 20, 20), "Can Die", _canDie);

            // Save values
            EditorPrefs.SetBool(EnableHitStateKey, _enableHitState);
            EditorPrefs.SetFloat(TimeToWaitKey, _timeToWait);
            EditorPrefs.SetBool(CanDieKey, _canDie);
        }

        #region Utilities

        public bool IsHitStateEnabled()
        {
            return _enableHitState;
        }

        public void SetHitStateEnabled(bool value)
        {
            _enableHitState = value;
        }

        public float GetTimeToWait()
        {
            return _timeToWait;
        }

        public void SetTimeToWait(float value)
        {
            _timeToWait = value;
        }

        public bool CanDie()
        {
            return _canDie;
        }

        public void SetCanDie(bool value)
        {
            _canDie = value;
        }

        #endregion
    }
}
#endif