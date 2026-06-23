using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    private Rigidbody playerRigidbody;
    private float xRange = 15f;
    private float zUpperBound = 15f;
    private float zLowerBound = 0f;

    [Header("Configuration")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float speed = 15f;
    [SerializeField] private bool debugCollisionVelocity;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float firingAnimationResetDelay = 0.1f;
    private float lastFireAnimationTime = float.NegativeInfinity;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float fireCooldown = 0.25f;
    private float nextFireTime;
    private float speedMultiplier = 1f;

    [Header("Powerups")]
    [SerializeField] private bool isShielded = false;
    [SerializeField] private GameObject shieldIndicator;
    [SerializeField] private bool isMultifiring = false;

    [Header("Multishot Configuration")]
    [SerializeField] private int bulletsWhenMultifiring = 3;
    [SerializeField] private float multifireSpreadAngle = 15f;


    void Awake()
    {
        inputActions = new InputSystem_Actions();
        playerRigidbody = GetComponent<Rigidbody>();

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<HealthBar>(true);
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        
        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PauseAction();
        if (gameManager != null && gameManager.IsPaused)
        {
            return;
        }

        MovePlayer();
        FireAction();
        UpdateFiringAnimation();
    }

    void LateUpdate()
    {
        ConstrainPlayerPosition();
        StopPhysicsDrift();
    }

    void MovePlayer()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        transform.Translate(new Vector3(moveInput.x, 0f, moveInput.y) * GetMoveSpeed() * Time.deltaTime);
    }

    private float GetMoveSpeed()
    {
        return speed * speedMultiplier;
    }

    void FireAction()
    {
        bool firePressed = inputActions.Player.Attack.WasPressedThisFrame();

        if (firePressed)
        {
            TriggerFiringAnimation();
        }

        if (firePressed && Time.time >= nextFireTime && projectilePrefab != null)
        {
            Quaternion baseRotation = projectilePrefab.transform.rotation;
            if (isMultifiring)
            {
                int bulletCount = Mathf.Max(1, bulletsWhenMultifiring);
                float startingAngle = -multifireSpreadAngle * (bulletCount - 1) / 2f;

                for (int i = 0; i < bulletCount; i++)
                {
                    float angle = startingAngle + i * multifireSpreadAngle;
                    Quaternion bulletRotation =
                        Quaternion.AngleAxis(angle, Vector3.up) * baseRotation;

                    SpawnProjectile(bulletRotation);
                }
            }
            else
            {
                SpawnProjectile(baseRotation);
            }

            nextFireTime = Time.time + fireCooldown;
        }
    }

    private void TriggerFiringAnimation()
    {
        if (animator == null)
        {
            return;
        }

        lastFireAnimationTime = Time.time;
        animator.SetBool("IsFiring", true);
        animator.SetTrigger("Fire");
    }

    private void UpdateFiringAnimation()
    {
        if (animator == null)
        {
            return;
        }

        bool recentlyFired = Time.time <= lastFireAnimationTime + firingAnimationResetDelay;
        animator.SetBool("IsFiring", recentlyFired);
    }

    private void SpawnProjectile(Quaternion bulletRotation)
    {
        GameObject spawnedProjectile = Instantiate(projectilePrefab, transform.position, bulletRotation);
        ProjectileController projectile = spawnedProjectile.GetComponent<ProjectileController>();
        if (projectile != null)
        {
            projectile.Initialize(ProjectileOwner.Player, projectileDamage);
        }
    }

    void PauseAction()
    {
        if (inputActions.Player.Pause.WasPressedThisFrame() && gameManager != null)
        {
            gameManager.TogglePause();
        }
    }

    void ConstrainPlayerPosition()
    {
        if (transform.position.x < -xRange || transform.position.x > xRange)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -xRange, xRange),
                transform.position.y,
                transform.position.z
            );
        }
        if (transform.position.z < zLowerBound || transform.position.z > zUpperBound)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z, zLowerBound, zUpperBound)
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        LogCollisionVelocity("Before collision handling", collision);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
            StopPhysicsDrift();
            LogCollisionVelocity("After enemy collision handling", collision);
            return;
        }

        BossController boss = collision.gameObject.GetComponentInParent<BossController>();
        if (boss != null)
        {
            int damageToBoss = currentHealth;
            SetShielded(false);
            ApplyDamage(boss.CurrentHealth);
            boss.TakeDamage(damageToBoss);
            StopPhysicsDrift();
            LogCollisionVelocity("After boss collision handling", collision);
        }
    }

    private void StopPhysicsDrift()
    {
        if (playerRigidbody == null)
        {
            return;
        }

        playerRigidbody.linearVelocity = Vector3.zero;
        playerRigidbody.angularVelocity = Vector3.zero;
    }

    private void LogCollisionVelocity(string message, Collision collision)
    {
        if (!debugCollisionVelocity || playerRigidbody == null)
        {
            return;
        }

        Debug.Log(
            $"{message}: hit {collision.gameObject.name}, player velocity {playerRigidbody.linearVelocity}",
            this
        );
    }

    private void TakeDamage(int damage)
    {
        if (isShielded)
        {
            SetShielded(false); // Resets after 1 hit
            return;
        }
        ApplyDamage(damage);
    }

    public void TakeProjectileDamage(int damage)
    {
        TakeDamage(damage);
    }

    private void ApplyDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth < 1)
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            Destroy(gameObject);
        }
    }

    public void SetShielded(bool value)
    {
        isShielded = value;

        if (shieldIndicator != null)
        {
            shieldIndicator.SetActive(isShielded);
        }
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = Mathf.Max(0f, multiplier);
    }

    public void SetMultifiring(bool value)
    {
        isMultifiring = value;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void OnDestroy()
    {
        inputActions.Dispose();
    }
}
