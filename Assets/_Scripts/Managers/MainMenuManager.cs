using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public void OnPlayButton(string sceneName)
    {
        Wallet.Instance.ResetCash();
        
        SceneManager.LoadScene(sceneName);
    }
    public void OnExitButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}