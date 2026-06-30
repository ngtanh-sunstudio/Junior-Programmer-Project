using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    private static readonly Quaternion EnemySpawnRotation = Quaternion.Euler(0f, 180f, 0f);

    private static readonly PoolType[] EnemyPoolTypes =
    {
        PoolType.Enemy1,
        PoolType.Enemy2,
        PoolType.Enemy3
    };

    private static readonly float[][] EnemySpawnWeightsByStage =
    { // enemy_1 - enemy_2 - enemy_3
        new[] { 1f, 0f, 0f },
        new[] { 0.75f, 0.25f, 0f },
        new[] { 0.25f, 0.5f, 0.25f },
        new[] { 0.1f, 0.4f, 0.5f },
        new[] { 0.1f, 0.3f, 0.6f }
    };

    [Header("Configuration")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private float timeToSpawn = 3f;
    [SerializeField] private float spawnInterval = 3f;
    [Tooltip("Number of enemies spawned in each wave at the start of the game.")]
    [SerializeField] private int startingEnemiesPerWave = 1;
    [Tooltip("Maximum number of enemies spawned in each wave after difficulty has fully ramped up.")]
    [SerializeField] private int maxEnemiesPerWave = 10;
    [Tooltip("Seconds it takes for waves to grow from the starting enemy count to the maximum enemy count.")]
    [SerializeField] private float timeToMaxEnemies = 60f;
    [Tooltip("Seconds between enemy spawn weight stages. Lower values change enemy mix sooner.")]
    [SerializeField] private float enemyWeightStageDuration = 15f;
    private float spawnStartTime;
    private bool isBossAlive = false;
    private float xRange = 15f;

    [Header("Powerups")]
    [SerializeField] private PowerupEffect[] powerupEffects;
    [SerializeField] private float powerupSpawnDelay = 5f;
    [SerializeField] private float powerupSpawnInterval = 10f;
    private float powerupMinZ = 2f;
    private float powerupMaxZ = 14f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogError($"{nameof(SpawnManager)} cannot start because no {nameof(PoolManager)} instance exists.", this);
            enabled = false;
            return;
        }

        spawnStartTime = Time.time;
        InvokeRepeating(nameof(SpawnEnemyWave), timeToSpawn, spawnInterval);
        InvokeRepeating(nameof(SpawnBoss), timeToMaxEnemies, timeToMaxEnemies);
        InvokeRepeating(nameof(SpawnPowerup), powerupSpawnDelay, powerupSpawnInterval);
    }

    private Vector3 GenerateRandomPosition()
    {
        return new Vector3(Random.Range(-xRange, xRange), 0.5f, 30f);
    }

    private void SpawnEnemyWave()
    {
        int enemiesThisWave = GetCurrentEnemiesPerWave();

        for (int i = 0; i < enemiesThisWave; i++)
        {
            int enemyIndex = GetRandomEnemyIndex();
            PoolType enemyPoolType = EnemyPoolTypes[enemyIndex];
            Vector3 enemyPosition = GenerateRandomPosition();
            GameObject spawnedEnemy = PoolManager.Instance.GetObjectFromPool(
                enemyPoolType,
                enemyPosition,
                EnemySpawnRotation
                );

            if (spawnedEnemy == null)
            {
                continue;
            }

            if (!spawnedEnemy.TryGetComponent<EnemyController>(out _))
            {
                Debug.LogError($"{enemyPoolType} is missing an {nameof(EnemyController)} component.", spawnedEnemy);
                PoolManager.Instance.ReturnObjectToPool(spawnedEnemy);
                continue;
            }
        }
    }

    private int GetCurrentEnemiesPerWave()
    {
        if (timeToMaxEnemies <= 0f)
        {
            return maxEnemiesPerWave;
        }

        float elapsedTime = Time.time - spawnStartTime;
        float progress = Mathf.Clamp01(elapsedTime / timeToMaxEnemies);
        return Mathf.RoundToInt(Mathf.Lerp(startingEnemiesPerWave, maxEnemiesPerWave, progress));
    }

    private int GetRandomEnemyIndex()
    {
        float[] weights = GetCurrentEnemyWeights();
        float randomValue = Random.value;

        if (randomValue < weights[0])
        {
            return 0;
        }

        if (randomValue < weights[0] + weights[1])
        {
            return 1;
        }

        return 2;
    }

    private float[] GetCurrentEnemyWeights()
    {
        if (enemyWeightStageDuration <= 0f)
        {
            return EnemySpawnWeightsByStage[EnemySpawnWeightsByStage.Length - 1];
        }

        float elapsedTime = Time.time - spawnStartTime;
        int stageIndex = Mathf.FloorToInt(elapsedTime / enemyWeightStageDuration);
        stageIndex = Mathf.Clamp(stageIndex, 0, EnemySpawnWeightsByStage.Length - 1);

        return EnemySpawnWeightsByStage[stageIndex];
    }

    private void SpawnBoss()
    {
        if (isBossAlive)
        {
            return;
        }

        Vector3 bossPos = GenerateRandomPosition();
        Instantiate(
            bossPrefab, 
            bossPos,
            EnemySpawnRotation
            );
        
        isBossAlive = true;
    }

    private void SpawnPowerup()
    {
        if (powerupEffects == null || powerupEffects.Length == 0)
        {
            Debug.LogError($"{nameof(SpawnManager)} cannot spawn powerups because no powerup effects are configured.", this);
            return;
        }

        PowerupEffect effect = powerupEffects[Random.Range(0, powerupEffects.Length)];
        if (effect == null || effect.PowerupPrefab == null)
        {
            Debug.LogWarning("SpawnManager has an unconfigured powerup effect.");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            Random.Range(-xRange, xRange),
            0.5f,
            Random.Range(powerupMinZ, powerupMaxZ)
        );

        GameObject spawnedPowerup = Instantiate(
            effect.PowerupPrefab,
            spawnPosition,
            effect.PowerupPrefab.transform.rotation
        );

        if (!spawnedPowerup.TryGetComponent(out PowerupItem powerupItem))
        {
            Debug.LogError(
                $"{effect.PowerupPrefab.name} is missing a {nameof(PowerupItem)} component.",
                spawnedPowerup
            );
            Destroy(spawnedPowerup);
            return;
        }

        powerupItem.Initialize(effect);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnEnemyWave));
        CancelInvoke(nameof(SpawnPowerup));
        CancelInvoke(nameof(SpawnBoss));
    }
}
