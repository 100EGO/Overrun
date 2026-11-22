using UnityEngine;

/// <summary>Configurable mission payout based on base HP and elapsed time.</summary>
public class MissionRewards : MonoBehaviour
{
    [SerializeField, Min(0)] private int baseCredits = 1000;
    [SerializeField, Min(1f)] private float targetTimeSeconds = 300f;
    [SerializeField, Range(0f, 1f)] private float minimumTimeMultiplier = 0.75f;
    [SerializeField, Min(1f)] private float maximumTimeMultiplier = 1.25f;

    public int LastCreditsAwarded { get; private set; }
    public float ElapsedSeconds { get; private set; }

    private void Update()
    {
        MissionManager mission = GetComponent<MissionManager>();
        if (mission != null && mission.State == MissionManager.MissionState.InProgress)
            ElapsedSeconds += Time.deltaTime;
    }

    public int Award(float currentBaseHealth, float maximumBaseHealth)
    {
        float healthRatio = maximumBaseHealth > 0f ? Mathf.Clamp01(currentBaseHealth / maximumBaseHealth) : 0f;
        float timeFactor = Mathf.Clamp(targetTimeSeconds / Mathf.Max(1f, ElapsedSeconds),
            Mathf.Min(minimumTimeMultiplier, maximumTimeMultiplier),
            Mathf.Max(minimumTimeMultiplier, maximumTimeMultiplier));
        LastCreditsAwarded = Mathf.RoundToInt(baseCredits * (0.5f + 0.5f * healthRatio) * timeFactor);
        if (EconomyInventory.Instance != null)
            EconomyInventory.Instance.AddCredits(LastCreditsAwarded);
        return LastCreditsAwarded;
    }
}
