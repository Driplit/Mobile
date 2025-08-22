using UnityEngine;
using TMPro;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public Vector3 nextSpawnPoint = Vector3.zero;

    public float baseSpeed = 5f;
    public float speedIncreaseRate = 0.5f;
    public float maxSpeed = 20f;

    public static float globalSpeed = 5f;

    private int tileIndex = 0; // Keeps track of tile number

    [Header("UI")]
    public TextMeshProUGUI speedText;  // Assign in Inspector

    void Start()
    {
        for (int i = 0; i < 15; i++) // Initial safe zone
        {
            SpawnTile();
        }
    }

    void Update()
    {
        globalSpeed = Mathf.Min(baseSpeed + Time.timeSinceLevelLoad * speedIncreaseRate, maxSpeed);

        if (speedText != null)
        {
            speedText.text = "Speed: " + globalSpeed.ToString("F1") + " m/s";
        }
    }

    public void SpawnTile()
    {
        GameObject tile = Instantiate(groundTilePrefab, nextSpawnPoint, Quaternion.identity);
        GroundTile gt = tile.GetComponent<GroundTile>();
        gt.tileIndex = tileIndex;

        nextSpawnPoint = tile.transform.GetChild(1).position;
        tileIndex++;
    }

    // Called from GroundTile when a tile is destroyed
    public void TileDestroyed()
    {
        SpawnTile();
    }
}
