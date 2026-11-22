using UnityEngine;

/// <summary>Loot falls naturally, then flies toward the player when close enough.</summary>
public class ResourcePickup : MonoBehaviour
{
    [SerializeField] private ResourceType resourceType;
    [SerializeField, Min(1)] private int amount = 1;
    [SerializeField, Min(0.1f)] private float pickupRadius = 0.6f;
    [SerializeField] private Transform player;

    [Header("Magnet attraction")]
    [SerializeField, Min(0.1f)] private float attractionRadius = 3f;
    [SerializeField, Min(0.1f)] private float attractionStartSpeed = 4f;
    [SerializeField, Min(0f)] private float attractionAcceleration = 18f;
    [SerializeField, Min(0f)] private float targetHeight = 1f;

    [Header("Lifetime")]
    [Tooltip("Seconds before uncollected loot disappears. Set to 0 to keep it indefinitely.")]
    [SerializeField, Min(0f)] private float lifetimeSeconds = 45f;

    private Rigidbody body;
    private Collider[] colliders;
    private float despawnAt;
    private bool attracted;
    private float currentSpeed;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        despawnAt = lifetimeSeconds > 0f ? Time.time + lifetimeSeconds : float.PositiveInfinity;
    }

    public void Initialize(ResourceType type, int quantity)
    {
        resourceType = type;
        amount = Mathf.Max(1, quantity);
    }

    private void Start()
    {
        if (player == null)
        {
            PlayerHealth health = FindAnyObjectByType<PlayerHealth>();
            if (health != null) player = health.transform;
        }
    }

    private void Update()
    {
        if (Time.time >= despawnAt)
        {
            Destroy(gameObject);
            return;
        }

        if (player == null || EconomyInventory.Instance == null) return;
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null && health.IsDead)
        {
            if (attracted) StopAttraction();
            return;
        }

        Vector3 horizontal = player.position - transform.position;
        horizontal.y = 0f;
        if (!attracted && horizontal.sqrMagnitude <= attractionRadius * attractionRadius)
            BeginAttraction();

        if (!attracted) return;

        Vector3 target = player.position + Vector3.up * targetHeight;
        if (Vector3.Distance(transform.position, target) <= pickupRadius)
        {
            EconomyInventory.Instance.Collect(resourceType, amount);
            Destroy(gameObject);
            return;
        }

        currentSpeed += attractionAcceleration * Time.deltaTime;
        Vector3 nextPosition = Vector3.MoveTowards(transform.position, target, currentSpeed * Time.deltaTime);
        if (body != null) body.MovePosition(nextPosition);
        else transform.position = nextPosition;
    }

    private void BeginAttraction()
    {
        attracted = true;
        currentSpeed = attractionStartSpeed;
        if (body != null)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.useGravity = false;
            body.isKinematic = true;
        }

        // Disable physical collisions during flight, so the pickup cannot obstruct movement.
        foreach (Collider col in colliders)
            if (col != null) col.enabled = false;
    }

    private void StopAttraction()
    {
        attracted = false;
        currentSpeed = 0f;
        if (body != null)
        {
            body.isKinematic = false;
            body.useGravity = true;
        }
        foreach (Collider col in colliders)
            if (col != null) col.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
