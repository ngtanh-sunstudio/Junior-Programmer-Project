using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float firingAnimationResetDelay = 0.1f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private float fireCooldown = 0.25f;

    [Header("Multifire")]
    [SerializeField] private bool isMultifiring;
    [SerializeField] private int bulletsWhenMultifiring = 3;
    [SerializeField] private float multifireSpreadAngle = 15f;

    private float lastFireAnimationTime = float.NegativeInfinity;
    private float nextFireTime;

    public event Action Fired;
    public event Action Multifire;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            return;
        }

        bool recentlyFired = Time.time <= lastFireAnimationTime + firingAnimationResetDelay;
        animator.SetBool("IsFiring", recentlyFired);
    }

    public void TryFire()
    {
        TriggerFiringAnimation();

        if (Time.time < nextFireTime || projectilePrefab == null)
        {
            return;
        }

        Quaternion baseRotation = projectilePrefab.transform.rotation;
        if (isMultifiring)
        {
            FireSpread(baseRotation);
        }
        else
        {
            SpawnProjectile(baseRotation);
        }

        nextFireTime = Time.time + fireCooldown;
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

    private void FireSpread(Quaternion baseRotation)
    {
        int bulletCount = Mathf.Max(1, bulletsWhenMultifiring);
        float startingAngle = -multifireSpreadAngle * (bulletCount - 1) / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startingAngle + i * multifireSpreadAngle;
            Quaternion bulletRotation = Quaternion.AngleAxis(angle, Vector3.up) * baseRotation;
            SpawnProjectile(bulletRotation);
        }
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
}
