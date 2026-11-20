using UnityEngine;

[RequireComponent(typeof(EnemyTargeting))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float attacksPerSecond = 1f;

    private EnemyTargeting targeting;
    private float nextAttackTime;

    private void Awake()
    {
        targeting = GetComponent<EnemyTargeting>();
    }

    private void Update()
    {
        Transform target = targeting.Target;
        if (target == null || Time.time < nextAttackTime)
            return;

        float distanceSqr = GetHorizontalDistanceSqr(target);
        if (distanceSqr > attackRange * attackRange)
            return;

        targeting.DealDamage(damage);
        nextAttackTime = Time.time + 1f / attacksPerSecond;
    }

    private float GetHorizontalDistanceSqr(Transform target)
    {
        // Buildings can be much larger than their attack range.
        // Measure from the enemy to the nearest collider surface, not the building center.
        if (target.TryGetComponent<BuildingHealth>(out _))
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            float nearestDistanceSqr = float.PositiveInfinity;

            foreach (Collider collider in colliders)
            {
                if (!collider.enabled || collider.isTrigger)
                    continue;

                Vector3 nearestPoint = collider.ClosestPoint(transform.position);
                Vector3 toSurface = nearestPoint - transform.position;
                toSurface.y = 0f;
                nearestDistanceSqr = Mathf.Min(nearestDistanceSqr, toSurface.sqrMagnitude);
            }

            if (!float.IsPositiveInfinity(nearestDistanceSqr))
                return nearestDistanceSqr;
        }

        // Preserve the existing player attack behavior (and fallback for missing colliders).
        Vector3 offset = target.position - transform.position;
        offset.y = 0f;
        return offset.sqrMagnitude;
    }
}
