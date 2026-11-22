using TMPro;
using UnityEngine;

/// <summary>Displays the last mission's results in the dedicated MissionResults scene.</summary>
public class MissionResultsUI : MonoBehaviour
{
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

    private void Start()
    {
        if (!MissionResultData.HasResult)
        {
            Debug.LogWarning("MissionResultsUI: no completed mission results available.", this);
            if (resultsPanel != null) resultsPanel.SetActive(false);
            return;
        }

        if (resultsPanel != null) resultsPanel.SetActive(true);
        bool victory = MissionResultData.Victory;
        if (outcomeText != null) outcomeText.text = victory ? "MISSION COMPLETE" : "MISSION FAILED";
        int seconds = Mathf.FloorToInt(MissionResultData.DurationSeconds);
        if (durationText != null) durationText.text = $"TIME  {seconds / 60:00}:{seconds % 60:00}";
        if (baseHealthText != null) baseHealthText.text = $"BASE HP  {Mathf.RoundToInt(MissionResultData.BaseHealthPercent * 100f)}%";
        if (baseCreditsText != null) baseCreditsText.text = $"BASE REWARD  {MissionResultData.BaseCredits} CR";
        if (healthMultiplierText != null) healthMultiplierText.text = victory ? $"BASE CONDITION  x{MissionResultData.HealthMultiplier:0.00}" : "BASE CONDITION  --";
        if (timeMultiplierText != null) timeMultiplierText.text = victory ? $"TIME BONUS  x{MissionResultData.TimeMultiplier:0.00}" : "TIME BONUS  --";
        if (earnedCreditsText != null) earnedCreditsText.text = $"+{MissionResultData.CreditsEarned} CR";
        if (totalCreditsText != null) totalCreditsText.text = $"TOTAL  {MissionResultData.TotalCredits} CR";
    }
}
