using System;
using System.Collections;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    [SerializeField] private PoolType projectilePoolType = PoolType.BossProjectile;
    [SerializeField] private Transform turret;
    [SerializeField] private float spreadAngle = 15f;
    [SerializeField] private int bulletsPerVolley = 3;
    [SerializeField] private int volleysFired = 5;
    [SerializeField] private float fireCooldown = 0.1f;
    [SerializeField] private float movementDelayAfterFiring = 1.5f;
    [SerializeField] private int projectileDamage = 1;

    private Coroutine fireRoutine;

    public bool IsFiring { get; private set; }
    public event Action Fired;

    public void TryStartFiring(Action onFinished)
    {
        if (IsFiring)
        {
            return;
        }

        fireRoutine = StartCoroutine(FireSequence(onFinished));
    }

    private IEnumerator FireSequence(Action onFinished)
    {
        IsFiring = true;

        int volleyCount = Mathf.Max(1, volleysFired);
        for (int i = 0; i < volleyCount; i++)
        {
            FireVolley();
            yield return new WaitForSeconds(fireCooldown);
        }

        yield return new WaitForSeconds(movementDelayAfterFiring);
        IsFiring = false;
        fireRoutine = null;
        onFinished?.Invoke();
    }

    private void FireVolley()
    {
        PoolManager poolManager = PoolManager.Instance;
        if (poolManager == null)
        {
            Debug.LogError($"{nameof(BossWeapon)} cannot fire because no {nameof(PoolManager)} instance exists.", this);
            return;
        }

        Transform firingPoint = turret != null ? turret : transform;
        Vector3 baseDirection = firingPoint.forward;
        int bulletCount = Mathf.Max(1, bulletsPerVolley);
        float startingAngle = -spreadAngle * (bulletCount - 1) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startingAngle + i * spreadAngle;
            Vector3 bulletDirection = Quaternion.AngleAxis(angle, Vector3.up) * baseDirection;
            SpawnProjectile(poolManager, firingPoint.position, bulletDirection);
        }

        Fired?.Invoke();
    }

    private void SpawnProjectile(PoolManager poolManager, Vector3 spawnPosition, Vector3 bulletDirection)
    {
        Quaternion bulletRotation = Quaternion.FromToRotation(Vector3.up, bulletDirection);
        GameObject spawnedProjectile = poolManager.GetObjectFromPool(projectilePoolType, spawnPosition, bulletRotation);
        
        if (spawnedProjectile == null)
        {
            Debug.LogWarning("Projectile pool is exhausted.");
            return;
        }
        
        ProjectileController projectile = spawnedProjectile.GetComponent<ProjectileController>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{projectilePoolType} is missing a {nameof(ProjectileController)} component.",
                spawnedProjectile
            );
            poolManager.ReturnObjectToPool(spawnedProjectile);
            return;
        }

        projectile.Initialize(ProjectileOwner.Boss, projectileDamage);
    }

    private void OnDisable()
    {
        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }

        IsFiring = false;
    }
}
