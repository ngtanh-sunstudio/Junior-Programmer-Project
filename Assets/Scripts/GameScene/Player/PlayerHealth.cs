using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private bool isShielded;
    [SerializeField] private GameObject shieldIndicator;
    [SerializeField] private ParticleSystem dieParticle;
    

    private PlayerMovement movement;
    private int currentHealth;
    private bool isDead;

    public event Action Died;
    public event Action Shielded;
    public event Action ShieldConsumed;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();

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

        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(isShielded);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        if (isShielded)
        {
            ConsumeShield();
            return;
        }

        ApplyDamage(damage);
    }

    public void SetShielded(bool value)
    {
        isShielded = value;

        if (isShielded)
        {
            Shielded?.Invoke();
        }
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(isShielded);
        }
    }

    private void ConsumeShield()
    {
        if (!isShielded)
        {
            return;
        }

        SetShielded(false);
        ShieldConsumed?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyController enemy = collision.gameObject.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            TakeDamage(1);
            enemy.Die();
            movement.StopPhysicsDrift();
            return;
        }

        BossHealth bossHealth = collision.gameObject.GetComponentInParent<BossHealth>();
        if (bossHealth != null)
        {
            int damageToBoss = currentHealth;
            ConsumeShield();
            ApplyDamage(bossHealth.CurrentHealth);
            bossHealth.TakeDamage(damageToBoss);
            movement.StopPhysicsDrift();
        }
    }

    private void ApplyDamage(int damage)
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
