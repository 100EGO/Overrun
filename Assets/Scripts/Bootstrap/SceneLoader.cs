using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Persistent asynchronous scene transitions. Optional loading UI can subscribe to ProgressChanged.</summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public static event System.Action<float> ProgressChanged;

    private bool loading;
    public bool IsLoading => loading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Load(string sceneName)
    {
        if (loading || string.IsNullOrWhiteSpace(sceneName))
            return;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"SceneLoader: scene '{sceneName}' is not included in Build Settings.", this);
            return;
        }

        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        loading = true;
        ProgressChanged?.Invoke(0f);

        // Yield once so a future loading overlay can render before loading starts.
        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (operation == null)
        {
            loading = false;
            yield break;
        }

        while (!operation.isDone)
        {
            ProgressChanged?.Invoke(Mathf.Clamp01(operation.progress / 0.9f));
            yield return null;
        }

        ProgressChanged?.Invoke(1f);
        loading = false;
    }

    public static void LoadScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.Load(sceneName);
            return;
        }

        // Editor convenience: allow Play directly from the mission or results scene.
        GameObject loader = new GameObject("SceneLoader");
        loader.AddComponent<SceneLoader>();
        Instance.Load(sceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            ProgressChanged = null;
        }
    }
}
