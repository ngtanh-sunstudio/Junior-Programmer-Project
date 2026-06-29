using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private PoolType projectilePoolType = PoolType.PlayerProjectile;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float firingAnimationResetDelay = 0.1f;

    [Header("Shooting")]
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float fireCooldown = 0.25f;

    [Header("Multifire")]
    [SerializeField] private bool isMultifiring;
    [SerializeField] private int bulletsWhenMultifiring = 3;
    [SerializeField] private float multifireSpreadAngle = 15f;

    private float lastFireAnimationTime = float.NegativeInfinity;
    private float nextFireTime;
    private bool loggedMissingAnimator;
    private bool loggedMissingPoolManager;

    public event Action Fired;
    public event Action Multifire;

    private void Awake()
    {
        if (animator == null)
        {
            LogMissingAnimator();
        }
    }

    private void Start()
    {
        if (PoolManager.Instance == null)
        {
            LogMissingPoolManager();
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            LogMissingAnimator();
            return;
        }

        bool recentlyFired = Time.time <= lastFireAnimationTime + firingAnimationResetDelay;
        animator.SetBool("IsFiring", recentlyFired);
    }

    public void TryFire()
    {
        TriggerFiringAnimation();
        PoolManager poolManager = PoolManager.Instance;

        if (Time.time < nextFireTime || poolManager == null)
        {
            if (poolManager == null)
            {
                LogMissingPoolManager();
            }
            return;
        }

        Quaternion baseRotation = poolManager.GetDefaultRotation(projectilePoolType);
        if (isMultifiring)
        {
            FireSpread(poolManager, baseRotation);
        }
        else
        {
            SpawnProjectile(poolManager, baseRotation);
        }

        nextFireTime = Time.time + fireCooldown;
        // Firing sound still plays even when pool is exhausted
        // => Intentional design
        Fired?.Invoke(); 
    }

    public void SetMultifiring(bool value)
    {
        if (value == true)
        {
            Multifire?.Invoke();
        }
        isMultifiring = value;
    }

    private void FireSpread(PoolManager poolManager, Quaternion baseRotation)
    {
        int bulletCount = Mathf.Max(1, bulletsWhenMultifiring);
        float startingAngle = -multifireSpreadAngle * (bulletCount - 1) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startingAngle + i * multifireSpreadAngle;
            Quaternion bulletRotation = Quaternion.AngleAxis(angle, Vector3.up) * baseRotation;
            SpawnProjectile(poolManager, bulletRotation);
        }
    }

    private void SpawnProjectile(PoolManager poolManager, Quaternion bulletRotation)
    {
        GameObject spawnedProjectile = 
            poolManager.GetObjectFromPool(projectilePoolType, transform.position, bulletRotation);

        if(spawnedProjectile == null)
        {
            Debug.LogWarning("Pool is exhausted");
            return;
        }

        ProjectileController projectile = 
            spawnedProjectile.GetComponent<ProjectileController>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{projectilePoolType} is missing a {nameof(ProjectileController)} component.",
                spawnedProjectile
            );
            poolManager.ReturnObjectToPool(spawnedProjectile);
            return;
        }

        projectile.Initialize(ProjectileOwner.Player, projectileDamage);
    }

    private void TriggerFiringAnimation()
    {
        if (animator == null)
        {
            LogMissingAnimator();
            return;
        }

        lastFireAnimationTime = Time.time;
        animator.SetBool("IsFiring", true);
        animator.SetTrigger("Fire");
    }

    private void LogMissingAnimator()
    {
        if (loggedMissingAnimator)
        {
            return;
        }

        Debug.LogError($"{nameof(PlayerWeapon)} is missing an animator reference.", this);
        loggedMissingAnimator = true;
    }

    private void LogMissingPoolManager()
    {
        if (loggedMissingPoolManager)
        {
            return;
        }

        Debug.LogError($"{nameof(PlayerWeapon)} cannot fire because no {nameof(PoolManager)} instance exists.", this);
        loggedMissingPoolManager = true;
    }
}
