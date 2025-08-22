using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndGame : MonoBehaviour
{
    [SerializeField] private AudioClip endGameSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            AudioManager.instance.PlaySoundFX(endGameSound, transform, 1f);
            GroundSpawner.globalSpeed = 0f; // Stop the ground immediately
            StartCoroutine(DelayedEndGame(1.5f)); // Delay before scene change
        }
    }

    private IEnumerator DelayedEndGame(float delay)
    {
        Debug.Log("Game Over!");
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu");
    }
}