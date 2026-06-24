using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private ParticleSystem dieParticle;
    
    private int currentHealth;
    private bool isDead;

    public event Action Died;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>(true);
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
