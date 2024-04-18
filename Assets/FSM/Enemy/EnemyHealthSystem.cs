using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyHealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private EnemyHealthBar enemyHealthBar;

    private void Awake()
    {
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        enemyHealthBar.UpdateHealthBar(maxHealth, currentHealth);
    }

    void Update()
    {
        if (currentHealth <= 0 && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0) Die();
        enemyHealthBar.UpdateHealthBar(maxHealth, currentHealth);
    }
    private void Die()
    {
        Destroy(gameObject);
    }

    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }
}
