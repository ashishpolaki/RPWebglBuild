using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public void LoadMainScene()
    {
       // SceneLoadingManager.Instance.LoadScene(Scene.Main);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
