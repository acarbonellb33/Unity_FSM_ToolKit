using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    [SerializeField]
    private HealthBar healthBar;
    
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0) Die();
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
    public void Heal()
    {
        currentHealth = maxHealth;
    }
    private void Die()
    {
        //Destroy(gameObject);
    }

    #region Getters

    public float GetMaxHealth() { return maxHealth; }

    public float GetCurrentHealth() { return currentHealth; }

    #endregion
}
