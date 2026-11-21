using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(EnemyTargeting))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.5f;

    [Header("Turret clearance")]
    [SerializeField, Min(0f)] private float avoidancePadding = 0.1f;

    private CharacterController controller;
    private EnemyTargeting targeting;

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

        Vector3 movement = toTarget.normalized * moveSpeed * Time.deltaTime;
        movement = SlideAlongTurretZones(movement);
        controller.Move(movement);

        Vector3 facing = movement;
        facing.y = 0f;
        if (facing.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(facing);
    }

    private Vector3 SlideAlongTurretZones(Vector3 movement)
    {
        Vector3 position = transform.position;
        position.y = 0f;

        // Apply the circle constraint a few times so adjacent turret zones work too.
        for (int pass = 0; pass < 3; pass++)
        {
            foreach (TurretAvoidanceZone zone in TurretAvoidanceZone.ActiveZones)
            {
                if (zone == null)
                    continue;

                Vector3 center = zone.transform.position;
                center.y = 0f;
                float radius = zone.Radius + controller.radius + avoidancePadding;

                Vector3 fromCenter = position - center;
                float distance = fromCenter.magnitude;
                Vector3 normal = distance > 0.0001f ? fromCenter / distance : Vector3.right;

                // Outside the circle: only change movement if this frame would enter it.
                Vector3 next = position + movement;
                Vector3 nextOffset = next - center;
                if (nextOffset.sqrMagnitude >= radius * radius && distance >= radius)
                    continue;

                // Slide by removing the component that points into the obstacle.
                float inward = Vector3.Dot(movement, normal);
                if (inward < 0f)
                    movement -= normal * inward;

                // Exactly head-on, the tangent is zero. Give a small, stable
                // sideways nudge to let the enemy start sliding around the edge.
                if (movement.sqrMagnitude < 0.000001f && inward < 0f)
                {
                    Vector3 tangent = new Vector3(-normal.z, 0f, normal.x);
                    float side = Vector3.Dot(tangent, transform.forward) >= 0f ? 1f : -1f;
                    movement = tangent * side * (-inward * 0.35f);
                }

                // Push an enemy that is already inside the circle gently back out.
                if (distance < radius)
                    movement += normal * Mathf.Min(radius - distance, moveSpeed * Time.deltaTime);
            }
        }

        return movement;
    }
}
