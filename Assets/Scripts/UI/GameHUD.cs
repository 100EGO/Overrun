using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Placeholder HUD. Assign scene references and UI elements in the Inspector.
/// Uses existing gameplay values; it does not control health, ammo or waves.
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("Gameplay references")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private BuildingHealth buildingHealth;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private WaveManager waveManager;

    [Header("Top center: base and waves")]
    [SerializeField] private Image baseHealthFill;
    [SerializeField] private TMP_Text baseHealthText;
    [SerializeField] private TMP_Text wavesText;

    [Header("Bottom left: player")]
    [SerializeField] private Image playerHealthFill;
    [SerializeField] private TMP_Text playerHealthText;

    [Header("Bottom right: weapon")]
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text reloadText;

    private void Start()
    {
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (buildingHealth == null) buildingHealth = FindFirstObjectByType<BuildingHealth>();
        if (playerShooting == null) playerShooting = FindFirstObjectByType<PlayerShooting>();
        if (waveManager == null) waveManager = FindFirstObjectByType<WaveManager>();
    }

    private void Update()
    {
        if (buildingHealth != null)
        {
            float max = buildingHealth.MaxHealth;
            if (baseHealthFill != null)
                baseHealthFill.fillAmount = max > 0f ? Mathf.Clamp01(buildingHealth.CurrentHealth / max) : 0f;
            if (baseHealthText != null)
                baseHealthText.text = $"BASE  {Mathf.CeilToInt(buildingHealth.CurrentHealth)} / {Mathf.CeilToInt(max)}";
        }

        if (playerHealth != null)
        {
            float max = playerHealth.MaxHealth;
            if (playerHealthFill != null)
                playerHealthFill.fillAmount = max > 0f ? Mathf.Clamp01(playerHealth.CurrentHealth / max) : 0f;
            if (playerHealthText != null)
                playerHealthText.text = $"HP  {Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(max)}";
        }

        if (waveManager != null && wavesText != null)
        {
            wavesText.text = waveManager.IsComplete
                ? "ALL WAVES CLEARED"
                : waveManager.IsStopped
                    ? "DEFENSE FAILED"
                    : waveManager.CurrentWave == 0
                        ? $"WAVES REMAINING  {waveManager.TotalWaves}"
                        : $"WAVE {waveManager.CurrentWave} / {waveManager.TotalWaves}  |  {waveManager.WavesRemaining} REMAINING";
        }

        if (playerShooting != null)
        {
            if (ammoText != null)
                ammoText.text = $"{playerShooting.AmmoInMagazine} / {playerShooting.MagazineSize}";
            if (reloadText != null)
                reloadText.text = playerShooting.IsReloading ? "RELOADING..." : "R  RELOAD";
        }
    }
}
