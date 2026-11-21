using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays one segment per wave, filling a segment only after its wave is cleared.
/// Attach to WaveProgressBar and assign WaveFill and WaveDividers in the Inspector.
/// </summary>
public class WaveProgressUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Image waveFill;
    [SerializeField] private RectTransform dividerContainer;

    [Header("Dividers")]
    [SerializeField] private Color dividerColor = Color.black;
    [SerializeField, Min(1f)] private float dividerWidth = 3f;

    private int displayedWaveCount = -1;

    private void Start()
    {
        if (waveManager == null)
            waveManager = FindFirstObjectByType<WaveManager>();

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (waveManager == null)
            return;

        int total = waveManager.TotalWaves;
        if (total != displayedWaveCount)
        {
            CreateDividers(total);
            displayedWaveCount = total;
        }

        if (waveFill != null)
            waveFill.fillAmount = total > 0
                ? Mathf.Clamp01((float)waveManager.CompletedWaves / total)
                : 0f;
    }

    private void CreateDividers(int total)
    {
        if (dividerContainer == null)
            return;

        for (int i = dividerContainer.childCount - 1; i >= 0; i--)
            Destroy(dividerContainer.GetChild(i).gameObject);

        for (int i = 1; i < total; i++)
        {
            GameObject divider = new GameObject(
                "Divider_" + i,
                typeof(RectTransform),
                typeof(Image)
            );

            divider.transform.SetParent(dividerContainer, false);
            RectTransform rect = divider.GetComponent<RectTransform>();
            float position = (float)i / total;

            rect.anchorMin = new Vector2(position, 0f);
            rect.anchorMax = new Vector2(position, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(dividerWidth, 0f);
            rect.anchoredPosition = Vector2.zero;

            Image image = divider.GetComponent<Image>();
            image.color = dividerColor;
            image.raycastTarget = false;
        }
    }
}
