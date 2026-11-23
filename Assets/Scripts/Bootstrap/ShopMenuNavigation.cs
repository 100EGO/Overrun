using UnityEngine;

/// <summary>Shop-specific navigation. Missions are launched from Contracts_Menu.</summary>
public class ShopMenuNavigation : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "Main_Menu";

    public void ReturnHome() => SceneLoader.LoadScene(mainMenuScene);
}
