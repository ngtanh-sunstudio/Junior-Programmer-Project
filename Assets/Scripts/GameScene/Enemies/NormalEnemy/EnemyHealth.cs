using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private HealthBar healthBar;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public event Action Died;

    private void Awake()
    {
        if (healthBar == null)
        {
            Debug.LogError($"{nameof(EnemyHealth)} is missing a health bar reference.", this);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

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
        Destroy(gameObject);
    }
}
