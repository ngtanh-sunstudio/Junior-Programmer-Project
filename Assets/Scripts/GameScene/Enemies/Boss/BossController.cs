using UnityEngine;

[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BossMovement))]
[RequireComponent(typeof(BossWeapon))]
public class BossController : MonoBehaviour
{
    [SerializeField] private int scoreValue = 500;

    private BossHealth health;

    public int CurrentHealth => health.CurrentHealth;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
    }

    private void OnEnable()
    {
        health.Died += HandleDeath;
    }

    private void Start()
    {
        if (ScoreKeeper.Instance == null)
        {
            Debug.LogError($"{nameof(BossController)} cannot award score because no {nameof(ScoreKeeper)} instance exists.", this);
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError($"{nameof(BossController)} cannot end the game because no {nameof(GameManager)} instance exists.", this);
        }
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    private void HandleDeath()
    {
        ScoreKeeper.Instance?.AddScore(scoreValue);

        GameManager.Instance?.WinGame();
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDeath;
        }
    }
}
