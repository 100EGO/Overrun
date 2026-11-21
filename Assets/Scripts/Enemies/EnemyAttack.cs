using UnityEngine;

[RequireComponent(typeof(EnemyTargeting))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float attacksPerSecond = 1f;

    private EnemyTargeting targeting;
    private float nextAttackTime;

    private void Awake() => targeting = GetComponent<EnemyTargeting>();

    private void Update()
    {
        Transform target = targeting.Target;
        if (target == null || Time.time < nextAttackTime) return;
        if (GetHorizontalDistanceSqr(target) > attackRange * attackRange) return;
        targeting.DealDamage(damage);
        nextAttackTime = Time.time + 1f / attacksPerSecond;
    }

    private float GetHorizontalDistanceSqr(Transform target)
    {
        // Buildings and turrets may have colliders extending far from their root pivot.
        if (target.GetComponent<BuildingHealth>() != null ||
            target.GetComponent<TurretHealth>() != null)
        {
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            float nearest = float.PositiveInfinity;
            foreach (Collider collider in colliders)
            {
                if (!collider.enabled || collider.isTrigger) continue;
                Vector3 point = collider.ClosestPoint(transform.position);
                Vector3 offset = point - transform.position;
                offset.y = 0f;
                nearest = Mathf.Min(nearest, offset.sqrMagnitude);
            }
            if (!float.IsPositiveInfinity(nearest)) return nearest;
        }

        Vector3 delta = target.position - transform.position;
        delta.y = 0f;
        return delta.sqrMagnitude;
    }
}
