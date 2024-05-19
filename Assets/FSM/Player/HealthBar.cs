namespace FSM.Player
{
    using UnityEngine;
    using UnityEngine.UI;
    /// <summary>
    /// Manages the health bar UI component.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        private Slider _slider;

        void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        /// <summary>
        /// Updates the health bar UI based on the current health and maximum health.
        /// </summary>
        /// <param name="maxHealth">Maximum health value.</param>
        /// <param name="currentHealth">Current health value.</param>
        public void UpdateHealthBar(float maxHealth, float currentHealth)
        {
            _slider.value = currentHealth / maxHealth;
        }
    }
}