using UnityEngine;

/// <summary>
/// Finds the closest enemy and rotates the turret head and gun toward it.
/// Shooting will be added separately.
/// </summary>
public class TurretAutoTarget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform yawPivot;
    [SerializeField] private Transform pitchPivot;

    [Header("Targeting")]
    [SerializeField, Min(0.1f)] private float detectionRange = 15f;
    [SerializeField, Min(0.05f)] private float scanInterval = 0.2f;
    [SerializeField, Min(1f)] private float rotationSpeed = 180f;
    [SerializeField, Min(1f)] private float pitchRotationSpeed = 90f;

    [Header("Yaw axes (local to yaw pivot)")]
    [SerializeField] private Vector3 yawAxis = Vector3.up;
    [SerializeField] private Vector3 forwardAxis = Vector3.forward;

    [Header("Pitch axes (local to pitch pivot)")]
    [SerializeField] private Vector3 pitchAxis = Vector3.right;
    [SerializeField] private Vector3 pitchForwardAxis = Vector3.forward;

    private EnemyHealth target;
    private float nextScanTime;

    private void Update()
    {
        if (Time.time >= nextScanTime)
        {
            FindNearestEnemy();
            nextScanTime = Time.time + scanInterval;
        }

        if (target == null)
            return;

        RotateTowardTarget(yawPivot, yawAxis, forwardAxis, rotationSpeed);
        RotateTowardTarget(pitchPivot, pitchAxis, pitchForwardAxis, pitchRotationSpeed);
    }

    private void RotateTowardTarget(Transform pivot, Vector3 localAxis, Vector3 localForward, float speed)
    {
        if (pivot == null || localAxis.sqrMagnitude < 0.0001f || localForward.sqrMagnitude < 0.0001f)
            return;

        Vector3 worldAxis = pivot.TransformDirection(localAxis.normalized);
        Vector3 toTarget = target.transform.position - pivot.position;
        Vector3 flatTarget = Vector3.ProjectOnPlane(toTarget, worldAxis);
        Vector3 flatForward = Vector3.ProjectOnPlane(
            pivot.TransformDirection(localForward.normalized), worldAxis);

        if (flatTarget.sqrMagnitude < 0.0001f || flatForward.sqrMagnitude < 0.0001f)
            return;

        float angle = Vector3.SignedAngle(flatForward, flatTarget, worldAxis);
        float step = Mathf.Clamp(angle, -speed * Time.deltaTime, speed * Time.deltaTime);
        pivot.rotation = Quaternion.AngleAxis(step, worldAxis) * pivot.rotation;
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
