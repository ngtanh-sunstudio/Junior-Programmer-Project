using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float zRange = 30f;

    [Header("Score")]
    [SerializeField] private int scoreValue = 10;

    private ScoreKeeper scoreKeeper;
    private Rigidbody enemyRigidbody;
    private bool isDead;

    public event Action Died;

    public void Initialize(ScoreKeeper scoreKeeper)
    {
        this.scoreKeeper = scoreKeeper;
    }

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody>();
        if (enemyRigidbody != null)
        {
            enemyRigidbody.isKinematic = true;
        }

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>(true);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        if (scoreKeeper == null)
        {
            scoreKeeper = FindFirstObjectByType<ScoreKeeper>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
        DestroyOutOfBounds();
    }

    private void MoveEnemy()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
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

        if (currentHealth < 1)
        {
            Die();
        }
    }

    private void DestroyOutOfBounds()
    {
        if (transform.position.z > zRange || transform.position.z < -zRange)
        {
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        
        isDead = true;

        if (scoreKeeper != null)
        {
            scoreKeeper.AddScore(scoreValue);
        }

        Died?.Invoke();
        Destroy(gameObject);
    }
}
