using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        slider.value = currentHealth / maxHealth;
    }
}
