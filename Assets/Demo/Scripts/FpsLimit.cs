namespace Demo.scripts
{
    using UnityEngine;
    using UnityEngine.UI;
    public class FPSLimit : MonoBehaviour
    {
        public float timer, refresh, avgFramerate;
        public string display = "{0} FPS";
        public Text m_Text;
        [SerializeField]
        private int frameRateLimit = 60;
        [SerializeField]
        private bool UpdateText = true;
        // Start is called before the first frame update
        void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = frameRateLimit;
        }

        // Update is called once per frame
        void Update()
        {
            if (UpdateText)
            {
                float timelapse = Time.smoothDeltaTime;
                timer = timer <= 0 ? refresh : timer -= timelapse;
                if (timer <= 0) avgFramerate = (int)(1f / timelapse);
                m_Text.text = string.Format(display, avgFramerate.ToString());
            }
        }
    }
}