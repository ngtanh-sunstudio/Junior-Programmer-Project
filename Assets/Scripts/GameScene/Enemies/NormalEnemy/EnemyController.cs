using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(EnemyMovement))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    private EnemyHealth health;

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
        if (ScoreKeeper.Instance == null)
        {
            Debug.LogError($"{nameof(EnemyController)} cannot award score because no {nameof(ScoreKeeper)} instance exists.", this);
        }
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
        ScoreKeeper.Instance?.AddScore(scoreValue);

        Died?.Invoke();
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogError($"{nameof(EnemyController)} cannot return to its pool because no {nameof(PoolManager)} instance exists.", this);
            gameObject.SetActive(false);
            return;
        }

        PoolManager.Instance.ReturnObjectToPool(gameObject);
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDeath;
        }
    }
}
