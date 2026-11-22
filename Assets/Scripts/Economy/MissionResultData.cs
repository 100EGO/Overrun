/// <summary>Snapshot of the most recently finished mission, preserved across scene loads.</summary>
public static class MissionResultData
{
    public static bool HasResult { get; private set; }
    public static bool Victory { get; private set; }
    public static float DurationSeconds { get; private set; }
    public static float BaseHealthPercent { get; private set; }
    public static int BaseCredits { get; private set; }
    public static float HealthMultiplier { get; private set; }
    public static float TimeMultiplier { get; private set; }
    public static int CreditsEarned { get; private set; }
    public static int TotalCredits { get; private set; }

    public static void Record(bool victory, float seconds, float healthPercent, MissionRewards rewards, int credits)
    {
        HasResult = true;
        Victory = victory;
        DurationSeconds = seconds;
        BaseHealthPercent = healthPercent;
        BaseCredits = victory && rewards != null ? rewards.BaseCredits : 0;
        HealthMultiplier = victory && rewards != null ? rewards.LastHealthMultiplier : 0f;
        TimeMultiplier = victory && rewards != null ? rewards.LastTimeMultiplier : 0f;
        CreditsEarned = victory ? credits : 0;
        TotalCredits = EconomyInventory.Instance != null ? EconomyInventory.Instance.Credits : 0;
    }
}
