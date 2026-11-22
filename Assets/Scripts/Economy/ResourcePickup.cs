using UnityEngine;

/// <summary>Collects when the player comes within the configured distance.</summary>
public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField, Min(1)] private int amount = 1;
    [SerializeField, Min(0.1f)] private float pickupRadius = 1f;
    [SerializeField] private Transform player;

    public void Initialize(ResourceType type, int quantity)
    {
        resourceType = type;
        amount = Mathf.Max(1, quantity);
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
            if (health != null) player = health.transform;
        }
    }

    private void Update()
    {
        if (player == null || EconomyInventory.Instance == null) return;
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null && health.IsDead) return;

        Vector3 delta = player.position - transform.position;
        delta.y = 0f;
        if (delta.sqrMagnitude > pickupRadius * pickupRadius) return;

        EconomyInventory.Instance.Collect(resourceType, amount);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
