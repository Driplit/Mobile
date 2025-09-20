using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveSettingsData
    {
        public int initialEnemyCount = 5;
        public float timePerRound = 30f;
        public float spawnRate = 1f;
        public int enemiesIncreasePerRound = 2;
        public float timeBetweenRounds = 5f;
    }

    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;
        [Range(0f, 1f)] public float spawnChance = 0.5f;
    }

    public Transform player;
    public TowerStats tower;
    public EnemySpawnInfo[] enemiesSpawnInfo;
    public GameObject bossEnemyPrefab;
    public WaveSettingsData waveSettings;
    public TierTracker tracker;  // reference to the TierTracker

    public float TimeRemaining { get; private set; }
    public float CooldownRemaining { get; private set; }
    public int CurrentRound => currentRound;
    public int CurrentTier => tracker != null ? tracker.CurrentTier : 1; // fallback if missing

    public float RoundDuration => waveSettings.timePerRound;
    public float CurrentRoundTimer => TimeRemaining;

    protected int currentRound = 1;
    private int enemiesToSpawn;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        tower = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TowerStats>();
        tower?.ResetStats();

        if (TierTracker.Instance != null)
        {
            tracker = TierTracker.Instance;
            tracker.OnTierChanged += OnTierChanged;
        }

        enemiesToSpawn = waveSettings.initialEnemyCount * CurrentTier;
        StartCoroutine(RoundLoop());
    }

    private void OnDestroy()
    {
        if (TierTracker.Instance != null)
            TierTracker.Instance.OnTierChanged -= OnTierChanged;
    }

    private void OnTierChanged(int newTier)
    {
        enemiesToSpawn = waveSettings.initialEnemyCount * newTier;
        Debug.Log($"Tier updated to {newTier}. Enemies to spawn: {enemiesToSpawn}");
    }

    IEnumerator RoundLoop()
    {
        while (true)
        {
            if (currentRound > 1)
            {
                CooldownRemaining = waveSettings.timeBetweenRounds;
                while (CooldownRemaining > 0f)
                {
                    CooldownRemaining -= Time.deltaTime;
                    yield return null;
                }
            }

            TimeRemaining = waveSettings.timePerRound;
            int totalEnemiesThisRound = enemiesToSpawn;

            bool isBossRound = (currentRound % 10 == 0);
            if (isBossRound) totalEnemiesThisRound += 1;

            Coroutine spawnRoutine = StartCoroutine(SpawnEnemies(totalEnemiesThisRound, isBossRound));

            while (TimeRemaining > 0f)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }

            yield return spawnRoutine;

            if (isBossRound)
                enemiesToSpawn += waveSettings.enemiesIncreasePerRound;

            currentRound++;
        }
    }

    IEnumerator SpawnEnemies(int totalCount, bool isBossRound)
    {
        int spawned = 0;
        bool bossSpawned = false;

        while (spawned < totalCount)
        {
            GameObject toSpawn = (isBossRound && !bossSpawned && spawned == totalCount - 1)
                ? bossEnemyPrefab
                : ChooseEnemyBySpawnChance(currentRound);

            SpawnEnemy(toSpawn);
            spawned++;
            yield return new WaitForSeconds(1f / waveSettings.spawnRate);
        }
    }

    GameObject ChooseEnemyBySpawnChance(int round)
    {
        float totalWeight = 0f;
        foreach (var enemyInfo in enemiesSpawnInfo)
            totalWeight += GetSpawnChance(enemyInfo, round);

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var enemyInfo in enemiesSpawnInfo)
        {
            cumulative += GetSpawnChance(enemyInfo, round);
            if (randomValue <= cumulative)
                return enemyInfo.enemyPrefab;
        }

        return enemiesSpawnInfo.Length > 0 ? enemiesSpawnInfo[0].enemyPrefab : null;
    }

    float GetSpawnChance(EnemySpawnInfo enemyInfo, int round)
    {
        return Mathf.Clamp(enemyInfo.spawnChance + 0.01f * (round - 1), 0f, 1f);
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (player == null || enemyPrefab == null) return;

        float angle = Random.Range(0f, 2f * Mathf.PI);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 15f;
        Vector3 spawnPos = player.position + offset;

        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.CalculateStats(currentRound);
        }
    }
}
