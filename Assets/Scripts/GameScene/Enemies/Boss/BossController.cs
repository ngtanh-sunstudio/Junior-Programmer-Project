using UnityEngine;

[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(BossMovement))]
[RequireComponent(typeof(BossWeapon))]
public class BossController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int scoreValue = 500;

    private BossHealth health;
    private ScoreKeeper scoreKeeper;

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
        if (scoreKeeper == null)
        {
            Debug.LogError($"{nameof(BossController)} is missing a score keeper reference.", this);
        }

        if (gameManager == null)
        {
            Debug.LogError($"{nameof(BossController)} is missing a game manager reference.", this);
        }
    }

    public void Initialize(ScoreKeeper scoreKeeper, GameManager gameManager)
    {
        this.scoreKeeper = scoreKeeper;
        this.gameManager = gameManager;
    }

    public void TakeDamage(int damage)
    {
        health.TakeDamage(damage);
    }

    private void HandleDeath()
    {
        if (scoreKeeper != null)
        {
            scoreKeeper.AddScore(scoreValue);
        }

        if (gameManager != null)
        {
            gameManager.WinGame();
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.Died -= HandleDeath;
        }
    }
}
