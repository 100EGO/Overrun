using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Press T to preview a turret, left click to place, Escape/right click to cancel.</summary>
public class TurretPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera placementCamera;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private LayerMask blockingMask = ~0;

    [Header("Placement")]
    [SerializeField, Min(0.1f)] private float maxPlacementDistance = 20f;
    [SerializeField, Min(0f)] private float minimumPlayerDistance = 1.5f;
    [SerializeField, Min(0.1f)] private float footprintRadius = 0.9f;
    [SerializeField, Min(0f)] private float groundOffset = 0f;
    [SerializeField] private Key activationKey = Key.E;
    [SerializeField] private PlayerAbilities abilities;

    [Header("Preview colors")]
    [SerializeField] private Color validColor = new Color(0.2f, 1f, 0.5f, 0.45f);
    [SerializeField] private Color invalidColor = new Color(1f, 0.2f, 0.2f, 0.45f);

    private GameObject preview;
    private Renderer[] previewRenderers;
    private MaterialPropertyBlock propertyBlock;
    private bool validPlacement;
    private Vector3 placementPosition;
    private bool placing;

    public bool IsPlacing => placing;
    public bool CanPlaceTurret => abilities != null && abilities.IsTurretReady;
    public bool BlocksShooting => placing || Time.frameCount == lastPlacementFrame;
    private int lastPlacementFrame = -1;

    private void Awake()
    {
        if (placementCamera == null) placementCamera = Camera.main;
        if (abilities == null) abilities = FindFirstObjectByType<PlayerAbilities>();
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard != null && keyboard[activationKey].wasPressedThisFrame)
        {
            if (placing) CancelPlacement();
            else BeginPlacement();
        }

        if (!placing) return;
        if ((keyboard != null && keyboard.escapeKey.wasPressedThisFrame) ||
            (mouse != null && mouse.rightButton.wasPressedThisFrame))
        {
            CancelPlacement();
            return;
        }

        UpdatePreview();
        if (validPlacement && mouse != null && mouse.leftButton.wasPressedThisFrame && abilities != null && abilities.TryConsumeTurretAbility())
        {
            Instantiate(turretPrefab, placementPosition, turretPrefab.transform.rotation);
            lastPlacementFrame = Time.frameCount;
            CancelPlacement();
        }
    }

    private void BeginPlacement()
    {
        if (!CanPlaceTurret)
        {
            Debug.Log("Turret ability E is on cooldown or PlayerAbilities is not assigned.", this);
            return;
        }

        if (turretPrefab == null)
        {
            Debug.LogWarning("TurretPlacement: assign a Turret Prefab.", this);
            return;
        }

        placing = true;
        preview = Instantiate(turretPrefab);
        preview.name = turretPrefab.name + " (Preview)";

        // The preview must not shoot, block movement or be targeted by enemies.
        foreach (MonoBehaviour behaviour in preview.GetComponentsInChildren<MonoBehaviour>(true))
            behaviour.enabled = false;
        foreach (Collider collider in preview.GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        previewRenderers = preview.GetComponentsInChildren<Renderer>(true);
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        validPlacement = false;
        if (preview == null || placementCamera == null || Mouse.current == null) return;

        Ray ray = placementCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask, QueryTriggerInteraction.Ignore))
        {
            preview.SetActive(false);
            return;
        }

        preview.SetActive(true);
        placementPosition = hit.point + Vector3.up * groundOffset;
        preview.transform.position = placementPosition;

        Vector3 horizontal = hit.point - transform.position;
        horizontal.y = 0f;
        bool withinRange = horizontal.sqrMagnitude <= maxPlacementDistance * maxPlacementDistance;
        bool awayFromPlayer = horizontal.sqrMagnitude >= minimumPlayerDistance * minimumPlayerDistance;

        // OverlapSphere uses a footprint around the placement point. Exclude ground
        // using blockingMask, and exclude the player by filtering its root.
        bool blocked = false;
        Collider[] overlaps = Physics.OverlapSphere(hit.point + Vector3.up * footprintRadius,
            footprintRadius, blockingMask, QueryTriggerInteraction.Ignore);
        foreach (Collider collider in overlaps)
        {
            if (collider.transform.root == transform.root || collider.transform.root == preview.transform.root)
                continue;
            blocked = true;
            break;
        }

        validPlacement = withinRange && awayFromPlayer && !blocked;
        Color tint = validPlacement ? validColor : invalidColor;
        foreach (Renderer renderer in previewRenderers)
        {
            if (renderer == null) continue;
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_BaseColor", tint);
            propertyBlock.SetColor("_Color", tint);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void CancelPlacement()
    {
        placing = false;
        validPlacement = false;
        if (preview != null) Destroy(preview);
        preview = null;
    }

    private void OnDisable() => CancelPlacement();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxPlacementDistance);
    }
}
