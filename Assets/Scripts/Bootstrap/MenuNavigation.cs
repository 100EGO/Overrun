using UnityEngine;

/// <summary>Reusable UI Button OnClick navigation for independent menu scenes.</summary>
public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private string homeScene = "Main_Menu";
    [SerializeField] private string contractsScene = "Contracts_Menu";
    [SerializeField] private string shopScene = "Shop_Menu";
    [SerializeField] private string achievementsScene = "Achievements_Menu";
    [SerializeField] private string optionsScene = "Options_Menu";

    public void OpenHome() => SceneLoader.LoadScene(homeScene);
    public void OpenContracts() => SceneLoader.LoadScene(contractsScene);
    public void OpenShop() => SceneLoader.LoadScene(shopScene);
    public void OpenAchievements() => SceneLoader.LoadScene(achievementsScene);
    public void OpenOptions() => SceneLoader.LoadScene(optionsScene);
    public void QuitGame() => Application.Quit();
}
