using UnityEngine;

/// <summary>
/// Automatically tracks the nearest enemy with yaw only, then fires straight ahead.
/// No gun pitch or projectile drop.
/// </summary>
public class TurretAutoTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform yawPivot;
    [SerializeField] private Transform muzzlePoint;

    [Header("Targeting")]
    [SerializeField, Min(0.1f)] private float detectionRange = 15f;
    [SerializeField, Min(0.05f)] private float scanInterval = 0.2f;
    [SerializeField, Min(1f)] private float rotationSpeed = 180f;

    [Header("Yaw axes (local to yaw pivot)")]
    [SerializeField] private Vector3 yawAxis = Vector3.up;
    [SerializeField] private Vector3 forwardAxis = Vector3.forward;

    [Header("Shooting")]
    [SerializeField, Min(0.1f)] private float shotsPerSecond = 2f;
    [SerializeField, Min(0f)] private float damage = 15f;
    [SerializeField, Range(0.1f, 30f)] private float aimToleranceDegrees = 5f;
    [SerializeField] private LayerMask hitLayers = ~0;

    private EnemyHealth target;
    private float nextScanTime;
    private float nextShotTime;

    private void Update()
    {
        if (Time.time >= nextScanTime)
        {
            FindNearestEnemy();
            nextScanTime = Time.time + scanInterval;
        }

        if (yawPivot == null || target == null || yawAxis.sqrMagnitude < 0.0001f ||
            forwardAxis.sqrMagnitude < 0.0001f)
            return;

        Vector3 worldAxis = yawPivot.TransformDirection(yawAxis.normalized);
        Vector3 toTarget = target.transform.position - yawPivot.position;
        Vector3 flatTarget = Vector3.ProjectOnPlane(toTarget, worldAxis);
        Vector3 flatForward = Vector3.ProjectOnPlane(
            yawPivot.TransformDirection(forwardAxis.normalized), worldAxis);

        if (flatTarget.sqrMagnitude < 0.0001f || flatForward.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.SignedAngle(flatForward, flatTarget, worldAxis);
        float step = Mathf.Clamp(angle, -rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
        yawPivot.rotation = Quaternion.AngleAxis(step, worldAxis) * yawPivot.rotation;

        // Recalculate after rotating. Fire only once the barrel faces the enemy.
        flatForward = Vector3.ProjectOnPlane(
            yawPivot.TransformDirection(forwardAxis.normalized), worldAxis).normalized;
        flatTarget = Vector3.ProjectOnPlane(target.transform.position - yawPivot.position, worldAxis);

        if (Time.time >= nextShotTime &&
            flatTarget.sqrMagnitude > 0.0001f &&
            Vector3.Angle(flatForward, flatTarget) <= aimToleranceDegrees)
        {
            Fire(flatForward);
            nextShotTime = Time.time + 1f / shotsPerSecond;
        }
    }

    private void Fire(Vector3 direction)
    {
        Vector3 origin = muzzlePoint != null ? muzzlePoint.position : yawPivot.position;
        Debug.DrawRay(origin, direction * detectionRange, Color.yellow, 0.2f);

        // Find the first obstacle, ignoring the turret's own colliders.
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, detectionRange,
            hitLayers, QueryTriggerInteraction.Ignore);

        float closestDistance = detectionRange + 0.001f;
        Collider closestCollider = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestCollider = hit.collider;
            }
        }

        if (closestCollider != null &&
            closestCollider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemy)
        {
            enemy.TakeDamage(damage);
        }
    }

    private void FindNearestEnemy()
    {
        target = null;
        float bestDistanceSquared = detectionRange * detectionRange;

        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            float distanceSquared = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSquared < bestDistanceSquared)
            {
                bestDistanceSquared = distanceSquared;
                target = enemy;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
