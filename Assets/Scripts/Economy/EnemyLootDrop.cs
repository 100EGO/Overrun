using UnityEngine;

/// <summary>Configure separately on each enemy prefab to choose its resource and quantity.</summary>
public class EnemyLootDrop : MonoBehaviour
{
    [SerializeField] private ResourcePickup pickupPrefab;
    [SerializeField] private ResourceType resourceType = ResourceType.Scrap;
    [SerializeField, Min(1)] private int minimumAmount = 1;
    [SerializeField, Min(1)] private int maximumAmount = 3;
    [SerializeField, Range(0f, 1f)] private float dropChance = 1f;
    [SerializeField] private Vector3 dropOffset = Vector3.zero;

    public void Drop()
    {
        if (pickupPrefab == null || Random.value > dropChance) return;
        int amount = Random.Range(minimumAmount, Mathf.Max(minimumAmount, maximumAmount) + 1);
        ResourcePickup pickup = Instantiate(pickupPrefab, transform.position + dropOffset, Quaternion.identity);
        pickup.Initialize(resourceType, amount);
    }
}
