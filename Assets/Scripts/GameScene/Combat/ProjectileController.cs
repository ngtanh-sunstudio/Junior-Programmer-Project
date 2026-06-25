using System;
using UnityEngine;

public enum ProjectileOwner
{
    Player,
    Boss
}

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float zRange = 30f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private ParticleSystem hitParticle;
    [SerializeField] private ProjectileOwner owner = ProjectileOwner.Player;
    [SerializeField] private ObjectPool objectPool;

    public event Action Hit;

    private void Awake()
    {
        objectPool = GetComponentInParent<ObjectPool>();
    }

    private void Update()
    {
        MoveProjectile();
        DestroyOutOfBounds();
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void DestroyOutOfBounds()
    {
        if (transform.position.z > zRange || transform.position.z < -zRange)
        {
            ReturnToPool();
        }
    }

    public void Initialize(ProjectileOwner projectileOwner, int damage)
    {
        owner = projectileOwner;
        projectileDamage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner == ProjectileOwner.Player)
        {
            DamageEnemyOrBoss(other);
        }
        else if (owner == ProjectileOwner.Boss)
        {
            DamagePlayer(other);
        }
    }

    private void DamageEnemyOrBoss(Collider other)
    {
        EnemyController enemy = other.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(projectileDamage);
            HandleHit();
            return;
        }

        BossHealth bossHealth = other.GetComponentInParent<BossHealth>();
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(projectileDamage);
            HandleHit();
        }
    }

    private void DamagePlayer(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(projectileDamage);
        HandleHit();
    }

    private void HandleHit()
    {
        Hit?.Invoke();
        PlayHitParticle();
        ReturnToPool();
    }

    private void PlayHitParticle()
    {
        if (hitParticle != null)
        {
            ParticleSystem effect = Instantiate(
                hitParticle,
                transform.position,
                hitParticle.transform.rotation
            );

            effect.Play();
            Destroy(
                effect.gameObject,
                effect.main.duration + effect.main.startLifetime.constant
            );
        }
    }

    private void ReturnToPool()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObjectToPool(gameObject);
        }
        else
        {
            Debug.LogError("Projectile has no ObjectPool reference.", this);
            gameObject.SetActive(false);
        }
    }
}
