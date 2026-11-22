using TMPro;
using UnityEngine;

/// <summary>Displays mission outcome and credit breakdown when the mission ends.</summary>
public class MissionResultsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private MissionRewards missionRewards;
    [SerializeField] private BuildingHealth buildingHealth;

    [Header("Results UI")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text outcomeText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private TMP_Text baseHealthText;
    [SerializeField] private TMP_Text baseCreditsText;
    [SerializeField] private TMP_Text healthMultiplierText;
    [SerializeField] private TMP_Text timeMultiplierText;
    [SerializeField] private TMP_Text earnedCreditsText;
    [SerializeField] private TMP_Text totalCreditsText;

    private bool shown;

    private void Start()
    {
        if (missionManager == null) missionManager = FindFirstObjectByType<MissionManager>();
        if (missionRewards == null) missionRewards = FindFirstObjectByType<MissionRewards>();
        if (buildingHealth == null) buildingHealth = FindFirstObjectByType<BuildingHealth>();
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    private void Update()
    {
        if (shown || missionManager == null ||
            missionManager.State == MissionManager.MissionState.InProgress) return;

        shown = true;
        bool victory = missionManager.State == MissionManager.MissionState.Victory;
        if (resultsPanel != null) resultsPanel.SetActive(true);

        if (outcomeText != null) outcomeText.text = victory ? "MISSION COMPLETE" : "MISSION FAILED";

        float seconds = missionRewards != null ? missionRewards.ElapsedSeconds : 0f;
        int totalSeconds = Mathf.FloorToInt(seconds);
        if (durationText != null)
            durationText.text = $"TIME  {totalSeconds / 60:00}:{totalSeconds % 60:00}";

        float baseRatio = buildingHealth != null && buildingHealth.MaxHealth > 0f
            ? Mathf.Clamp01(buildingHealth.CurrentHealth / buildingHealth.MaxHealth) : 0f;
        if (baseHealthText != null)
            baseHealthText.text = $"BASE HP  {Mathf.RoundToInt(baseRatio * 100f)}%";

        if (baseCreditsText != null)
            baseCreditsText.text = victory && missionRewards != null
                ? $"BASE REWARD  {missionRewards.BaseCredits} CR" : "BASE REWARD  0 CR";
        if (healthMultiplierText != null)
            healthMultiplierText.text = victory && missionRewards != null
                ? $"BASE CONDITION  x{missionRewards.LastHealthMultiplier:0.00}" : "BASE CONDITION  --";
        if (timeMultiplierText != null)
            timeMultiplierText.text = victory && missionRewards != null
                ? $"TIME BONUS  x{missionRewards.LastTimeMultiplier:0.00}" : "TIME BONUS  --";

        int earned = victory && missionRewards != null ? missionRewards.LastCreditsAwarded : 0;
        if (earnedCreditsText != null) earnedCreditsText.text = $"+{earned} CR";
        if (totalCreditsText != null)
            totalCreditsText.text = $"TOTAL  {(EconomyInventory.Instance != null ? EconomyInventory.Instance.Credits : 0)} CR";
    }
}
