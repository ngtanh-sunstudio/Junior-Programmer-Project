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
    [SerializeField] private ProjectileOwner owner = ProjectileOwner.Player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
            Destroy(gameObject);
        }
    }

    public void Initialize(ProjectileOwner projectileOwner, int damage)
    {
        owner = projectileOwner;
        projectileDamage = damage;
    }

    void OnTriggerEnter(Collider other)
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
            Destroy(gameObject);
            return;
        }

        BossController boss = other.GetComponentInParent<BossController>();
        if (boss != null)
        {
            boss.TakeDamage(projectileDamage);
            Destroy(gameObject);
        }
    }

    private void DamagePlayer(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            return;
        }

        player.TakeProjectileDamage(projectileDamage);
        Destroy(gameObject);
    }
}
