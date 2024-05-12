#if UNITY_EDITOR
namespace EditorWindow.FSMSystem.Windows
{
    using UnityEditor;
    using UnityEngine;
    using Data.Save;
    public class FSMHitStatePopup : PopupWindowContent
    {
        private const string EnableHitStateKey = "EnableHitState";
        private const string TimeToWaitKey = "TimeToWait";
        private const string CanDieKey = "CanDie";

        private bool enableHitState = false;
        private float timeToWait = 0.0f;
        private bool canDie = false;

        public void Initialize(FSMHitSaveData hitData)
        {
            enableHitState = hitData.HitEnable;
            timeToWait = hitData.TimeToWait;
            canDie = hitData.CanDie;
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
            //canDie = EditorPrefs.GetBool(CanDieKey, false);

            // Display a label for Hit State Setup
            EditorGUI.LabelField(new Rect(10, 10, rect.width - 20, 20), "Hit State Setup", EditorStyles.boldLabel);

            // Toggle for "Enable Hit State"
            enableHitState =
                EditorGUI.Toggle(new Rect(10, 40, rect.width - 20, 20), "Enable Hit State", enableHitState);

            // FloatField for "Time to Wait"
            timeToWait = EditorGUI.FloatField(new Rect(10, 70, rect.width - 20, 20), "Time to Wait:", timeToWait);

            // Toggle for "Can Die"
            canDie = EditorGUI.Toggle(new Rect(10, 100, rect.width - 20, 20), "Can Die", canDie);

            // Save values
            EditorPrefs.SetBool(EnableHitStateKey, enableHitState);
            EditorPrefs.SetFloat(TimeToWaitKey, timeToWait);
            EditorPrefs.SetBool(CanDieKey, canDie);
        }

        #region Utilities

        public bool IsHitStateEnabled()
        {
            return enableHitState;
        }

        public void SetHitStateEnabled(bool value)
        {
            enableHitState = value;
        }

        public float GetTimeToWait()
        {
            return timeToWait;
        }

        public void SetTimeToWait(float value)
        {
            timeToWait = value;
        }

        public bool CanDie()
        {
            return canDie;
        }

        public void SetCanDie(bool value)
        {
            canDie = value;
        }

        #endregion
    }
}
#endif