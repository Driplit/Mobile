using UnityEngine;


public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float bobSpeed = 2f;     // Speed of bobbing motion
    [SerializeField] private float bobHeight = 0.25f; // Height of bobbing motion

    [SerializeField] private AudioClip coinClip;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Rotate the coin on its Z axis
        transform.Rotate(0, 0, turnSpeed * Time.deltaTime);

        // Bob up and down using a sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react if collided with player
        if (!other.CompareTag("Player"))
            return;

        // Add to score
        ScoreManager.Instance?.AddScore(1);

        // Play coin sound
        AudioManager.instance.PlaySoundFX(coinClip, transform, 1f);

        // Destroy coin
        Destroy(gameObject);
    }
}
