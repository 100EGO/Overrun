using UnityEngine;

/// <summary>Attach to any Shop_Menu object and wire these public methods to UI Button OnClick.</summary>
public class ShopMenuNavigation : MonoBehaviour
{
    [SerializeField] private string missionScene = "Overrun_TopDown";

    public void StartMission() => SceneLoader.LoadScene(missionScene);
    public void QuitGame() => Application.Quit();
}
