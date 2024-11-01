using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreenObject;

    // Singleton instance
    public static LoadingScreen Instance { get; private set; }

    private void Awake()
    {
        // Ensure there's only one instance of this class in the scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Show()
    {
        loadingScreenObject.SetActive(true);
    }
    public void Hide()
    {
        loadingScreenObject.SetActive(false);
    }
    public async Task PerformAsyncWithLoading(Func<Task> asyncOperation, Action action = null)
    {
        try
        {
            Show();
            await asyncOperation();
        }
        finally
        {
            Hide();
            action?.Invoke();
        }
    }

    public async Task<T> PerformAsyncWithLoading<T>(Func<Task<T>> asyncOperation, Action action = null)
    {
        try
        {
            Show();
            T result = await asyncOperation();
            return result;
        }
        finally
        {
            Hide();
            action?.Invoke();
        }
    }

    public void LoadSceneAdditiveAsync(int index, Action action = null)
    {
        StartCoroutine(LoadSceneCoroutine(index, action));
    }

    IEnumerator LoadSceneCoroutine(int index, Action action = null)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;
        Show();
        while (!asyncOperation.isDone)
        {
            // Output the progress percentage
            //float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            //Debug.Log("Loading progress: " + (progress * 100) + "%");

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                // Activate the scene
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
        action?.Invoke();
        Hide();
    }
}
