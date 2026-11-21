using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(EnemyTargeting))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.5f;

    [Header("Turret avoidance")]
    [SerializeField, Min(0f)] private float avoidancePadding = 0.4f;
    [SerializeField, Min(0.1f)] private float avoidanceLookAhead = 4f;

    private CharacterController controller;
    private EnemyTargeting targeting;
    private TurretAvoidanceZone currentObstacle;
    private float avoidanceSide;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        targeting = GetComponent<EnemyTargeting>();
    }

    private void Update()
    {
        Transform target = targeting.Target;
        if (target == null)
            return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.magnitude <= stoppingDistance)
            return;

        Vector3 direction = GetAvoidanceDirection(toTarget.normalized);
        controller.Move(direction * moveSpeed * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private Vector3 GetAvoidanceDirection(Vector3 desired)
    {
        Vector3 position = transform.position;
        position.y = 0f;
        TurretAvoidanceZone closest = null;
        float nearest = float.PositiveInfinity;

        foreach (TurretAvoidanceZone zone in TurretAvoidanceZone.ActiveZones)
        {
            if (zone == null)
                continue;

            Vector3 center = zone.transform.position;
            center.y = 0f;
            Vector3 offset = center - position;
            float radius = zone.Radius + avoidancePadding + controller.radius;
            float along = Vector3.Dot(offset, desired);
            float lookAhead = Mathf.Max(avoidanceLookAhead, radius);

            // Ignore zones behind the enemy or too far from its intended path.
            if (along < -radius || along > lookAhead + radius)
                continue;

            Vector3 closestOnPath = position + desired * Mathf.Clamp(along, 0f, lookAhead);
            float pathDistance = Vector3.Distance(closestOnPath, center);
            if (pathDistance >= radius)
                continue;

            float distance = offset.sqrMagnitude;
            if (distance < nearest)
            {
                nearest = distance;
                closest = zone;
            }
        }

        if (closest == null)
        {
            currentObstacle = null;
            return desired;
        }

        Vector3 away = position - closest.transform.position;
        away.y = 0f;
        float safeRadius = closest.Radius + avoidancePadding + controller.radius;
        if (away.sqrMagnitude < 0.0001f)
            away = -desired;

        if (closest != currentObstacle)
        {
            currentObstacle = closest;
            float cross = Vector3.Cross(desired, away).y;
            avoidanceSide = Mathf.Abs(cross) > 0.01f
                ? Mathf.Sign(cross)
                : (transform.position.x + transform.position.z >= 0f ? 1f : -1f);
        }

        // Tangential movement around the clearance circle, plus an outward push.
        Vector3 radial = away.normalized;
        Vector3 tangent = avoidanceSide > 0f
            ? new Vector3(radial.z, 0f, -radial.x)
            : new Vector3(-radial.z, 0f, radial.x);

        float distanceToCenter = away.magnitude;
        float outwardWeight = distanceToCenter < safeRadius ? 1.5f : 0.35f;
        return (tangent + radial * outwardWeight).normalized;
    }
}
