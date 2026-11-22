using UnityEngine;

/// <summary>Configurable mission payout based on base HP and elapsed time.</summary>
public class MissionRewards : MonoBehaviour
{
    [SerializeField, Min(0)] private int baseCredits = 1000;
    [SerializeField, Min(1f)] private float targetTimeSeconds = 300f;
    [SerializeField, Range(0f, 1f)] private float minimumTimeMultiplier = 0.75f;
    [SerializeField, Min(1f)] private float maximumTimeMultiplier = 1.25f;

    public int BaseCredits => baseCredits;
    public float LastHealthMultiplier { get; private set; } = 1f;
    public float LastTimeMultiplier { get; private set; } = 1f;
    public int LastCreditsAwarded { get; private set; }
    public float ElapsedSeconds { get; private set; }

    private MissionManager mission;

    private void Awake()
    {
        mission = GetComponent<MissionManager>();
    }

    private void Update()
    {
        if (mission != null && mission.State == MissionManager.MissionState.InProgress)
            ElapsedSeconds += Time.deltaTime;
    }

    public int Award(float currentBaseHealth, float maximumBaseHealth)
    {
        float healthRatio = maximumBaseHealth > 0f ? Mathf.Clamp01(currentBaseHealth / maximumBaseHealth) : 0f;
        float timeFactor = Mathf.Clamp(targetTimeSeconds / Mathf.Max(1f, ElapsedSeconds),
            Mathf.Min(minimumTimeMultiplier, maximumTimeMultiplier),
            Mathf.Max(minimumTimeMultiplier, maximumTimeMultiplier));
        LastHealthMultiplier = 0.5f + 0.5f * healthRatio;
        LastTimeMultiplier = timeFactor;
        LastCreditsAwarded = Mathf.RoundToInt(baseCredits * LastHealthMultiplier * LastTimeMultiplier);
        if (EconomyInventory.Instance != null)
            EconomyInventory.Instance.AddCredits(LastCreditsAwarded);
        return LastCreditsAwarded;
    }
}
