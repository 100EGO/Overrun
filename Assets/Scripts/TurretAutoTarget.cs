using UnityEngine;

/// <summary>
/// Prototype: finds the closest enemy and rotates the turret head toward it.
/// No shooting or gun pitch yet.
/// </summary>
public class TurretAutoTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform yawPivot;

    [Header("Targeting")]
    [SerializeField, Min(0.1f)] private float detectionRange = 15f;
    [SerializeField, Min(0.05f)] private float scanInterval = 0.2f;
    [SerializeField, Min(1f)] private float rotationSpeed = 180f;

    [Header("Imported model axes (local to yaw pivot)")]
    [SerializeField] private Vector3 yawAxis = Vector3.up;
    [SerializeField] private Vector3 forwardAxis = Vector3.forward;

    private EnemyHealth target;
    private float nextScanTime;

    private void Update()
    {
        if (Time.time >= nextScanTime)
        {
            FindNearestEnemy();
            nextScanTime = Time.time + scanInterval;
        }

        if (yawPivot == null || target == null)
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
