using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public GameObject coinPrefab;
    public int tileIndex;

    private GroundSpawner spawner;
    private bool tileSpawned = false;

    void Start()
    {
        spawner = FindFirstObjectByType<GroundSpawner>();

        if (tileIndex >= 3)
        {
            SpawnObstacle();
            SpawnCoins();
        }
    }

    void Update()
    {
        transform.Translate(Vector3.back * GroundSpawner.globalSpeed * Time.deltaTime);

        // Early trigger to spawn next tile before a gap forms
        if (!tileSpawned && transform.position.z < 5f)
        {
            spawner.SpawnTile();
            tileSpawned = true;
        }

        // Destroy when far enough behind
        if (transform.position.z < -15f)
        {
            spawner.TileDestroyed(); // Optional: only if tracking number of active tiles
            Destroy(gameObject);
        }
    }

    private int obstacleSpawnIndex = -1; // Store the chosen obstacle spawn index

    void SpawnObstacle()
    {
        // Choose a random child index for obstacle spawn (2 to 4 inclusive)
        obstacleSpawnIndex = Random.Range(2, 5);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex);

        GameObject obstacle = Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity);
        obstacle.transform.SetParent(transform);
    }

    void SpawnCoins()
    {
        // Use the same range of spawn indices as obstacles
        List<int> spawnIndices = new List<int> { 2, 3, 4 };

        // Remove the index used by the obstacle
        spawnIndices.Remove(obstacleSpawnIndex);

        // Random number of coins to spawn (0 to number of available spots)
        int coinsToSpawn = Random.Range(1, spawnIndices.Count + 1);

        for (int i = 0; i < coinsToSpawn; i++)
        {
            int randomIndex = Random.Range(0, spawnIndices.Count);
            int spawnIndex = spawnIndices[randomIndex];
            spawnIndices.RemoveAt(randomIndex); // Prevent duplicates

            Transform spawnPoint = transform.GetChild(spawnIndex);
            GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
            coin.transform.SetParent(transform);
        }
    }

}
