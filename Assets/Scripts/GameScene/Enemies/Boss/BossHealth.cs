using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private WorldHealthBar healthBar;
    [SerializeField] private ParticleSystem dieParticle;
    
    private int currentHealth;
    private bool isDead;
    private bool isInitialized;

    public event Action Died;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        isInitialized = healthBar != null && healthBar.Initialize();

        if (!isInitialized)
        {
            Debug.LogError("Boss health bar is not correctly configured.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {
            return;
        }

        currentHealth = maxHealth;

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

        isDead = true;
        Died?.Invoke();
        PlayDieParticle();
        Destroy(gameObject);
    }

    private void PlayDieParticle()
    {
        if (dieParticle != null)
        {
            ParticleSystem effect = Instantiate(
                dieParticle,
                transform.position,
                dieParticle.transform.rotation
            );

            effect.Play();
            Destroy(
                effect.gameObject,
                effect.main.duration + effect.main.startLifetime.constant
            );
        }
    }
}
