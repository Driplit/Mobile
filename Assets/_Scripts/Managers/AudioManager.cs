using UnityEditor.PackageManager;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource soundFXObject;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign clip
        audioSource.clip = audioClip;
        // assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get lenth of sound
        float clipLength = audioSource.clip.length;
        //destroy the clip after
        Destroy(audioSource.gameObject, clipLength);
    }
}
