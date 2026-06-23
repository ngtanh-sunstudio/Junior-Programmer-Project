using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float xMoveInterval = 1.5f;
    [SerializeField] private float xRange = 15f;
    [SerializeField] private float targetZ = 15f;
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    [SerializeField] private HealthBar healthBar;
    private Vector3 targetPosition;
    private bool hasReachedBattlePosition;

    [Header("Shooting Mechanism")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform turret;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private int bulletsPerVolley = 3;
    [SerializeField] private int volleysFired = 5;
    [SerializeField] private float fireCooldown = 0.1f;
    [SerializeField] private int projectileDamage = 1;
    private bool isFiring;
    private Coroutine fireRoutine;

    [Header("Score")]
    [SerializeField] private int scoreValue = 500;
    private ScoreKeeper scoreKeeper;

    public void Initialize(ScoreKeeper scoreKeeper)
    {
        this.scoreKeeper = scoreKeeper;
    }

    private void Awake()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>(true);
        }
    }

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
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);
    }

    void Update()
    {
        if (!hasReachedBattlePosition)
        {
            MoveToBattlePosition();
        }
        else
        {
            MoveSideToSide();
        }
    }

    private void MoveToBattlePosition()
    {
        targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Mathf.Approximately(transform.position.z, targetZ))
        {
            hasReachedBattlePosition = true;
            PickNewXTarget();
        }
    }

    private void MoveSideToSide()
    {
        if (isFiring)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (HasReachedTargetPosition())
        {
            fireRoutine = StartCoroutine(FireThenPickNextTarget());
        }
    }

    private void PickNewXTarget()
    {
        float randomX = Random.Range(-xRange, xRange);
        targetPosition = new Vector3(randomX, transform.position.y, targetZ);
    }

    private bool HasReachedTargetPosition()
    {
        return Vector3.Distance(transform.position, targetPosition) <= 0.01f;
    }

    private IEnumerator FireThenPickNextTarget()
    {
        isFiring = true;

        int volleyCount = Mathf.Max(1, volleysFired);
        for (int i = 0; i < volleyCount; i++)
        {
            FireVolley();
            yield return new WaitForSeconds(fireCooldown);
        }

        yield return new WaitForSeconds(xMoveInterval);
        PickNewXTarget();
        isFiring = false;
        fireRoutine = null;
    }

    private void FireVolley()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Transform firingPoint = turret != null ? turret : transform;
        Vector3 baseDirection = firingPoint.forward;
        int bulletCount = Mathf.Max(1, bulletsPerVolley);
        float startingAngle = -spreadAngle * (bulletCount - 1) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startingAngle + i * spreadAngle;
            Vector3 bulletDirection =
                Quaternion.AngleAxis(angle, Vector3.up) * baseDirection;

            SpawnProjectile(firingPoint.position, bulletDirection);
        }
    }

    private void SpawnProjectile(Vector3 spawnPosition, Vector3 bulletDirection)
    {
        Quaternion bulletRotation = Quaternion.FromToRotation(Vector3.up, bulletDirection);
        GameObject spawnedProjectile = Instantiate(projectilePrefab, spawnPosition, bulletRotation);
        ProjectileController projectile = spawnedProjectile.GetComponent<ProjectileController>();
        if (projectile != null)
        {
            projectile.Initialize(ProjectileOwner.Boss, projectileDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth < 1)
        {
            if (scoreKeeper != null)
            {
                scoreKeeper.AddScore(scoreValue);
            }

            if (gameManager != null)
            {
                gameManager.WinGame();
            }

            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }
}
