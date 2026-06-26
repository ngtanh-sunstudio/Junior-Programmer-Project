using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static readonly float[][] EnemySpawnWeightsByStage =
    { // enemy_1 - enemy_2 - enemy_3
        new[] { 1f, 0f, 0f },
        new[] { 0.75f, 0.25f, 0f },
        new[] { 0.25f, 0.5f, 0.25f },
        new[] { 0.1f, 0.4f, 0.5f },
        new[] { 0.1f, 0.3f, 0.6f }
    };

    [Header("Configuration")]
    [SerializeField] private GameObject[] enemyPrefabs;
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
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScoreKeeper scoreKeeper;
    [SerializeField] private ObjectPool objectPool;

    [Header("Powerups")]
    [SerializeField] private PowerupEffect[] powerupEffects;
    [SerializeField] private float powerupSpawnDelay = 5f;
    [SerializeField] private float powerupSpawnInterval = 10f;
    private float powerupMinZ = 2f;
    private float powerupMaxZ = 14f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            if (enemyIndex < 0) // Haven't assigned a prefab
            {
                return;
            }

            Vector3 enemyPos = GenerateRandomPosition();
            GameObject spawnedEnemy = Instantiate(enemyPrefabs[enemyIndex], enemyPos,
                        Quaternion.FromToRotation(Vector3.forward, -Vector3.forward));

            EnemyController enemy = spawnedEnemy.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Initialize(scoreKeeper);
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
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("SpawnManager has no enemy prefabs configured.");
            return -1;
        }

        float[] weights = GetCurrentEnemyWeights();
        float randomValue = Random.value;

        if (randomValue < weights[0])
        {
            return GetConfiguredEnemyIndex(0);
        }

        if (randomValue < weights[0] + weights[1])
        {
            return GetConfiguredEnemyIndex(1);
        }

        return GetConfiguredEnemyIndex(2);
    }

    private int GetConfiguredEnemyIndex(int enemyIndex)
    {
        if (enemyIndex < enemyPrefabs.Length && enemyPrefabs[enemyIndex] != null)
        {
            return enemyIndex;
        }

        return GetFirstConfiguredEnemyIndex();
    }

    private int GetFirstConfiguredEnemyIndex()
    {
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                return i;
            }
        }

        Debug.LogWarning("SpawnManager enemy prefab slots are empty.");
        return -1;
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
        GameObject spawnedBoss = Instantiate(
            bossPrefab, 
            bossPos,
            Quaternion.FromToRotation(Vector3.forward, -Vector3.forward));
        BossController boss = spawnedBoss.GetComponent<BossController>();
        BossWeapon weapon = spawnedBoss.GetComponent<BossWeapon>();

        if (boss != null)
        {
            boss.Initialize(scoreKeeper, gameManager);
        }
        if (weapon != null)
        {
            weapon.Initialize(objectPool);
        }
        
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

        PowerupItem powerupItem = spawnedPowerup.GetComponent<PowerupItem>();
        if (powerupItem == null)
        {
            powerupItem = spawnedPowerup.AddComponent<PowerupItem>();
        }

        powerupItem.Initialize(effect);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnEnemyWave));
        CancelInvoke(nameof(SpawnPowerup));
    }
}
