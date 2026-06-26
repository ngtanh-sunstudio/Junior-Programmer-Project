using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    private EnemyHealth health;
    private ScoreKeeper scoreKeeper;

    public event Action Died;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void Start()
    {
        if (scoreKeeper == null)
        {
            Debug.LogError($"{nameof(EnemyController)} is missing a score keeper reference.", this);
        }
    }

    public void Initialize(ScoreKeeper scoreKeeper)
    {
        this.scoreKeeper = scoreKeeper;
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    public void Die()
    {
        health.Die();
    }

    private void HandleDeath()
    {
        if (scoreKeeper != null)
        {
            scoreKeeper.AddScore(scoreValue);
        }

        Died?.Invoke();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDeath;
        }
    }
}
