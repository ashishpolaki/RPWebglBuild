using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    Menu, Race, Winner
};


public class SceneLoadingManager : MonoBehaviour
{
    public static SceneLoadingManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);        
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);        
    }
    public void LoadScene(Scene scene)
    {
        LoadScene((int)scene);
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReloadActiveScene(float _time)
    {
        Invoke(nameof(ReloadCurrentScene), _time);
    }
}
