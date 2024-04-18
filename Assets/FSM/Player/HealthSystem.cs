using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float maxKevlar = 100f;
    
    private float currentHealth;
    public float kevlar;
    [SerializeField]
    private AudioClip hurtSound;
    [SerializeField]
    private GameObject gameOverScreen;

    void Update()
    {
        if (currentHealth <= 0 && gameObject.activeSelf)
        {
            gameOverScreen.SetActive(true);
            GetComponent<CharacterController>().enabled = false;
            Camera.main.gameObject.GetComponent<MouseLook>().enabled = false;
            if (Input.anyKey) 
            { 
                gameOverScreen.SetActive(false);
                GetComponent<CharacterController>().enabled = true;
                Camera.main.gameObject.GetComponent<MouseLook>().enabled = true;
                kevlar = maxKevlar;
                currentHealth = maxHealth;
            }
        }
    }
    public void TakeDamage(float amount)
    {
        if (kevlar > 0)
        {
            kevlar -= amount * 0.75f;
            currentHealth -= amount * 0.25f;
            if (kevlar < 0)
            {
                currentHealth += kevlar;
                kevlar = 0;
            }
        }
        else
        {
            currentHealth -= amount;
        }
    }
    
    public void Heal()
    {
        currentHealth = maxHealth;
    }
    
    public void RepairKevlar()
    {
        kevlar = maxKevlar;
    }

    #region Getters

    public float GetMaxHealth() { return maxHealth; }
    public float GetMaxKevlar() { return maxKevlar; }
    
    public float GetCurrentHealth() { return currentHealth; }
    public float GetKevlarAmount() { return kevlar; }

    #endregion
}
