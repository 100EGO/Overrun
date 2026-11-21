using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Two placeholder abilities triggered by Q and E, each with its own cooldown.
/// Attach to the Abilities panel and assign the UI references in the Inspector.
/// </summary>
public class PlayerAbilities : MonoBehaviour
{
    [System.Serializable]
    private class AbilitySlot
    {
        public string abilityName = "ABILITY";
        [Min(0.1f)] public float cooldownSeconds = 5f;
        public Image fillImage;
        public Image iconImage;
        public TMP_Text nameText;

        [HideInInspector] public float readyAt;
        [HideInInspector] public Color originalIconColor;
    }

    [Header("Q ability")]
    [SerializeField] private AbilitySlot abilityQ = new AbilitySlot { abilityName = "ABILITY Q" };

    [Header("E ability")]
    [SerializeField] private AbilitySlot abilityE = new AbilitySlot { abilityName = "ABILITY E" };

    [Header("Appearance")]
    [SerializeField] private Color cooldownTint = new Color(0.45f, 0.45f, 0.45f, 1f);

    private void Awake()
    {
        Initialize(abilityQ);
        Initialize(abilityE);
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.qKey.wasPressedThisFrame)
                TryActivate(abilityQ, "Q");

            if (keyboard.eKey.wasPressedThisFrame)
                TryActivate(abilityE, "E");
        }

        Refresh(abilityQ);
        Refresh(abilityE);
    }

    private void Initialize(AbilitySlot slot)
    {
        slot.readyAt = 0f;
        if (slot.iconImage != null)
            slot.originalIconColor = slot.iconImage.color;

        Refresh(slot);
    }

    private void TryActivate(AbilitySlot slot, string key)
    {
        if (Time.time < slot.readyAt)
            return;

        slot.readyAt = Time.time + slot.cooldownSeconds;

        // Placeholder: replace with the actual ability effect later.
        Debug.Log("Ability " + key + " activated: " + slot.abilityName, this);
        Refresh(slot);
    }

    private void Refresh(AbilitySlot slot)
    {
        float remaining = Mathf.Max(0f, slot.readyAt - Time.time);
        bool coolingDown = remaining > 0f;

        // Full when ready; drains to empty over the cooldown.
        if (slot.fillImage != null)
            slot.fillImage.fillAmount = coolingDown
                ? Mathf.Clamp01(remaining / slot.cooldownSeconds)
                : 1f;

        if (slot.iconImage != null)
            slot.iconImage.color = coolingDown ? cooldownTint : slot.originalIconColor;

        if (slot.nameText != null)
            slot.nameText.text = coolingDown
                ? remaining.ToString("0.0") + "s"
                : slot.abilityName;
    }
}
