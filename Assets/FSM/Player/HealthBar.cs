namespace FSM.Player
{
    using UnityEngine;
    using UnityEngine.UI;

    public class HealthBar : MonoBehaviour
    {
        private Slider _slider;

        void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            _slider.value = currentHealth / maxHealth;
        }
    }
}