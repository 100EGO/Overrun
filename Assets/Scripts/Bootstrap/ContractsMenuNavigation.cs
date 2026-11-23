using UnityEngine;

/// <summary>Launches the selected contract's mission scene from Contracts_Menu.</summary>
public class ContractsMenuNavigation : MonoBehaviour
{
    [SerializeField] private string missionScene = "Overrun_TopDown";
    [SerializeField] private string mainMenuScene = "Main_Menu";

    public void StartMission() => SceneLoader.LoadScene(missionScene);
    public void ReturnHome() => SceneLoader.LoadScene(mainMenuScene);
}
