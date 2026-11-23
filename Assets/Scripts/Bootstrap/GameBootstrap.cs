using UnityEngine;

/// <summary>Single entry point for player builds. Owns persistent services, not mission gameplay.</summary>
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private string initialScene = "Main_Menu";

    private void Awake()
    {
        if (EconomyInventory.Instance == null)
        {
            GameObject services = new GameObject("PersistentServices");
            services.AddComponent<EconomyInventory>();
        }
    }

    private void Start()
    {
        if (SceneLoader.Instance == null)
        {
            GameObject loader = new GameObject("SceneLoader");
            loader.AddComponent<SceneLoader>();
        }

        if (!string.IsNullOrWhiteSpace(initialScene))
            SceneLoader.Instance.Load(initialScene);
    }
}
