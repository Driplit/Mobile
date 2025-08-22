using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    [Tooltip("The road section prefab to instantiate.")]
    public GameObject roadSection;

    [Tooltip("Z offset where the new section will spawn.")]
    public float spawnZOffset = 74f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Usually, it's the player entering
        {
            Vector3 newPosition = new Vector3(0, 0, transform.position.z + spawnZOffset);
            Instantiate(roadSection, newPosition, Quaternion.identity);
            //Debug.Log("Spawned new road section at " + newPosition);
        }


    }

    public GameObject obstaclePrefab;

    void SpawnObstacle()
    {
        // choose random point to spawn obstical
        
        int obstacleSpawnIndex = Random.Range(-1,2);
        Transform spawnPoint = transform.GetChild(obstacleSpawnIndex).transform;

        //spawn at the posisiton
        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity);
    }

 
}