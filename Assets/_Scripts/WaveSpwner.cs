using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    // ----- Settings for each wave -----
    [System.Serializable]
    public class WaveSettingsData
    {
        public int initialEnemyCount = 5;       // How many enemies spawn in the first round
        public float timePerRound = 30f;        // Duration of each round
        public float spawnRate = 1f;            // Enemies spawned per second
        public int enemiesIncreasePerRound = 2; // How many more enemies per round
        public float timeBetweenRounds = 5f;    // Cooldown between rounds
    }

    // ----- Information for spawning different enemy types -----
    [System.Serializable]
    public class EnemySpawnInfo
    {
        public GameObject enemyPrefab;          // Reference to the enemy prefab
        [Range(0f, 1f)] public float spawnChance = 0.5f; // Probability of spawning
    }

    // ----- Public references -----
    public Transform player;                   // Reference to the player position
    public EnemySpawnInfo[] enemiesSpawnInfo;  // Array of possible enemy types
    public GameObject bossEnemyPrefab;         // Reference to boss prefab

    public WaveSettingsData waveSettings;      // Wave configuration data

    // ----- Properties for UI or other scripts -----
    public float TimeRemaining { get; private set; } // Current countdown for round
    public float CooldownRemaining { get; private set; } // Time left before next round
    public int CurrentRound => currentRound;         // Read-only current round number
    public int CurrentTier => currentTier;           // Read-only current tier (future use)
    public float RoundDuration => waveSettings.timePerRound; // Max duration of round
    public float CurrentRoundTimer => TimeRemaining;        // Timer for UI

    // Shortcut to allow UI scripts to access wave settings
    public WaveSettingsData WaveSettings => waveSettings;

    // ----- Internal state -----
    protected int currentRound = 1;        // Tracks the current round
    protected int currentTier = 1;         // Tracks current tier (can be used later for scaling)
    private int enemiesToSpawn;            // Number of enemies to spawn this round

    void Start()
    {
        // Set initial number of enemies
        enemiesToSpawn = waveSettings.initialEnemyCount;

        // Start the main loop of rounds
        StartCoroutine(RoundLoop());
    }

    // ----- Main round loop -----
    IEnumerator RoundLoop()
    {
        while (true) // Infinite loop, will keep spawning rounds
        {
            // Wait cooldown before starting next round (skip for first round)
            if (currentRound > 1)
            {
                CooldownRemaining = waveSettings.timeBetweenRounds;
                Debug.Log($"Next round starts in {CooldownRemaining} seconds.");

                while (CooldownRemaining > 0f)
                {
                    CooldownRemaining -= Time.deltaTime; // Decrease cooldown
                    yield return null;                    // Wait until next frame
                }
            }

            Debug.Log($"Round {currentRound} started!");

            // Initialize round timer
            TimeRemaining = waveSettings.timePerRound;

            int totalEnemiesThisRound = enemiesToSpawn;

            // Check if it's a boss round (every 10th round)
            bool isBossRound = (currentRound % 10 == 0);
            if (isBossRound)
            {
                totalEnemiesThisRound += 1; // Spawn 1 extra boss
                Debug.Log("Boss round! 1 boss enemy will spawn this round.");
            }

            // Start spawning enemies for this round
            Coroutine spawnRoutine = StartCoroutine(SpawnEnemies(totalEnemiesThisRound, isBossRound));

            // Countdown the round timer
            while (TimeRemaining > 0f)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
            }

            // Wait until all enemies finish spawning
            yield return spawnRoutine;

            // Increase number of enemies for next rounds (only after boss rounds)
            if (isBossRound)
            {
                enemiesToSpawn += waveSettings.enemiesIncreasePerRound;
                //Debug.Log($"Enemies to spawn increased to {enemiesToSpawn} for next rounds.");
            }

            currentRound++; // Move to next round
        }
    }

    // ----- Spawn enemies for current round -----
    IEnumerator SpawnEnemies(int totalCount, bool isBossRound)
    {
        int spawned = 0;
        bool bossSpawned = false;

        while (spawned < totalCount)
        {
            GameObject toSpawn;

            // Decide if boss should spawn
            if (isBossRound && !bossSpawned)
            {
                if (spawned == totalCount - 1)
                {
                    toSpawn = bossEnemyPrefab; // Spawn boss last
                    bossSpawned = true;
                }
                else
                {
                    toSpawn = ChooseEnemyBySpawnChance(currentRound); // Spawn regular enemy
                }
            }
            else
            {
                toSpawn = ChooseEnemyBySpawnChance(currentRound);
            }

            // Spawn the enemy
            SpawnEnemy(toSpawn);
            spawned++;

            // Wait according to spawn rate
            yield return new WaitForSeconds(1f / waveSettings.spawnRate);
        }
    }

    // ----- Randomly choose an enemy based on spawn chance -----
    GameObject ChooseEnemyBySpawnChance(int round)
    {
        float totalWeight = 0f;

        // Sum total weights of all enemies
        foreach (var enemyInfo in enemiesSpawnInfo)
        {
            totalWeight += GetSpawnChance(enemyInfo, round);
        }

        // Pick a random value
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        // Select enemy based on random value
        foreach (var enemyInfo in enemiesSpawnInfo)
        {
            cumulative += GetSpawnChance(enemyInfo, round);
            if (randomValue <= cumulative)
            {
                return enemyInfo.enemyPrefab;
            }
        }

        // Fallback: return first enemy if array is empty
        return enemiesSpawnInfo.Length > 0 ? enemiesSpawnInfo[0].enemyPrefab : null;
    }

    // ----- Adjust spawn chance per round (optional scaling) -----
    float GetSpawnChance(EnemySpawnInfo enemyInfo, int round)
    {
        return Mathf.Clamp(enemyInfo.spawnChance + 0.01f * (round - 1), 0f, 1f);
    }

    // ----- Instantiate enemy and calculate stats before spawning -----
    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is missing in WaveSpawner!");
            return;
        }

        // Random spawn position around player (circle radius 15)
        float angle = Random.Range(0f, 2f * Mathf.PI);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * 15f;
        Vector3 spawnPos = player.position + offset;

        // Instantiate enemy
        GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Calculate its stats BEFORE spawning
        Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.CalculateStats(currentRound); // can add tier multiplier if needed
        }
    }
}
