using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Mission references")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private BuildingHealth buildingHealth;
    [SerializeField] private WaveManager waveManager;

    public enum MissionState { InProgress, Victory, Defeat }
    public MissionState State { get; private set; } = MissionState.InProgress;

    private void Start()
    {
        if (playerHealth == null || buildingHealth == null || waveManager == null)
        {
            Debug.LogError("MissionManager: assign PlayerHealth, BuildingHealth and WaveManager.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (State != MissionState.InProgress)
            return;

        // Check defeat first so a simultaneous last kill cannot override it.
        if (playerHealth.CurrentHealth <= 0f)
        {
            FinishMission(MissionState.Defeat, "Player defeated");
        }
        else if (buildingHealth.IsDestroyed)
        {
            FinishMission(MissionState.Defeat, "Building destroyed");
        }
        else if (waveManager.IsComplete)
        {
            FinishMission(MissionState.Victory, "All waves cleared");
        }
    }

    private void FinishMission(MissionState result, string reason)
    {
        State = result;

        if (result == MissionState.Defeat)
        {
            waveManager.StopWaves();
            Debug.Log($"MISSION FAILED: {reason}.", this);
        }
        else
        {
            Debug.Log("MISSION COMPLETE: Victory!", this);
        }
    }
}
