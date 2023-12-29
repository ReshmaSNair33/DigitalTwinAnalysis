using UnityEngine;
using UnityEngine.SceneManagement; // Namespace for scene management

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
