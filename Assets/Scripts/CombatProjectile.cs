using UnityEngine;

/// <summary>
/// A moving projectile shared by player weapons and automatic turrets.
/// Uses a sphere sweep each frame to prevent tunnelling through thin colliders.
/// </summary>
public class CombatProjectile : MonoBehaviour
{
    [SerializeField, Min(0.001f)] private float collisionRadius = 0.06f;
    [SerializeField] private LayerMask collisionLayers = ~0;

    private Vector3 direction;
    private float speed;
    private float damage;
    private float remainingDistance;
    private Transform ownerRoot;
    private bool initialized;

    public void Initialize(Vector3 travelDirection, float travelSpeed, float shotDamage,
        float maxDistance, Transform shooterRoot)
    {
        if (travelDirection.sqrMagnitude < 0.0001f || travelSpeed <= 0f || maxDistance <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        direction = travelDirection.normalized;
        speed = travelSpeed;
        damage = shotDamage;
        remainingDistance = maxDistance;
        ownerRoot = shooterRoot;
        transform.rotation = Quaternion.LookRotation(direction);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        float distance = Mathf.Min(speed * Time.deltaTime, remainingDistance);
        if (distance <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, collisionRadius,
            direction, distance, collisionLayers, QueryTriggerInteraction.Ignore);

        float closestDistance = float.PositiveInfinity;
        Collider closestCollider = null;

        foreach (RaycastHit hit in hits)
        {
            if (IsOwnerCollider(hit.collider))
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestCollider = hit.collider;
            }
        }

        if (closestCollider != null)
        {
            transform.position += direction * closestDistance;
            EnemyHealth enemy = closestCollider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        transform.position += direction * distance;
        remainingDistance -= distance;

        if (remainingDistance <= 0f)
            Destroy(gameObject);
    }

    private bool IsOwnerCollider(Collider collider)
    {
        if (ownerRoot == null)
            return false;

        Transform hitTransform = collider.transform;
        return hitTransform == ownerRoot || hitTransform.IsChildOf(ownerRoot);
    }
}
