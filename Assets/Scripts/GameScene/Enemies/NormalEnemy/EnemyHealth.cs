using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private WorldHealthBar healthBar;

    private int currentHealth;
    private bool isDead;
    private bool isInitialized;

    public int CurrentHealth => currentHealth;
    public event Action Died;

    private void Awake()
    {
        isInitialized = healthBar != null && healthBar.Initialize();

        if (!isInitialized)
        {
            Debug.LogError("Enemy health bar is not correctly configured.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            return;
        }

        // Pooled enemies must begin every activation with a fresh health state.
        currentHealth = maxHealth;
        isDead = false;

        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!isInitialized || isDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        if (currentHealth > 0)
        {
            return;
        }

        Die();
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Died?.Invoke();
    }
}
