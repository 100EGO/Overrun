using UnityEngine;

/// <summary>Attach to any Shop_Menu object and wire these public methods to UI Button OnClick.</summary>
public class ShopMenuNavigation : MonoBehaviour
{
    [SerializeField] private string missionScene = "Overrun_TopDown";

    [SerializeField] private string mainMenuScene = "Main_Menu";

    public void ReturnHome() => SceneLoader.LoadScene(mainMenuScene);

    // Legacy test shortcut; normal mission selection happens in Contracts_Menu.
    public void StartMission() => SceneLoader.LoadScene(missionScene);
    public void QuitGame() => Application.Quit();
}
