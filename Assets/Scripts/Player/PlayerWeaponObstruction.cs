using UnityEngine;

[DefaultExecutionOrder(-50)]
public class PlayerWeaponObstruction : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField, Min(0.001f)] private float contactMargin = 0.02f;
    public bool BlocksFire { get; private set; }

    private bool Ready => weaponCollider != null && weaponCollider.enabled;

    private void GetBox(out Vector3 center, out Vector3 half, out Quaternion rotation)
    {
        Transform t = weaponCollider.transform;
        center = t.TransformPoint(weaponCollider.center);
        Vector3 scale = t.lossyScale;
        half = Vector3.Scale(weaponCollider.size * 0.5f,
            new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
        rotation = t.rotation;
    }

    private bool IsObstacle(Collider c) => c != null && c.transform.root != transform.root;

    private void Update()
    {
        BlocksFire = false;
        if (!Ready) return;
        GetBox(out Vector3 center, out Vector3 half, out Quaternion rotation);
        foreach (Collider c in Physics.OverlapBox(center, half + Vector3.one * contactMargin,
                     rotation, obstacleLayers, QueryTriggerInteraction.Ignore))
            if (IsObstacle(c)) { BlocksFire = true; break; }
    }

    public Vector3 ConstrainMovement(Vector3 displacement)
    {
        if (!Ready) return displacement;
        Vector3 horizontal = new Vector3(displacement.x, 0f, displacement.z);
        float length = horizontal.magnitude;
        if (length < 0.00001f) return displacement;
        GetBox(out Vector3 center, out Vector3 half, out Quaternion rotation);
        Vector3 direction = horizontal / length;
        float allowed = length;
        foreach (RaycastHit hit in Physics.BoxCastAll(center, half, direction, rotation,
                     length + contactMargin, obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            if (!IsObstacle(hit.collider) || Vector3.Dot(direction, hit.normal) >= -0.001f) continue;
            allowed = Mathf.Min(allowed, Mathf.Max(0f, hit.distance - contactMargin));
        }
        // Box casts may miss initial overlaps. Allow retreat, but not deeper penetration.
        foreach (Collider c in Physics.OverlapBox(center, half, rotation, obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            if (!IsObstacle(c)) continue;
            if (Physics.ComputePenetration(weaponCollider, weaponCollider.transform.position,
                    rotation, c, c.transform.position, c.transform.rotation,
                    out Vector3 separation, out _) && Vector3.Dot(direction, separation) < -0.001f)
                allowed = 0f;
        }
        return direction * allowed + Vector3.up * displacement.y;
    }

    public Quaternion ConstrainRotation(Quaternion desired)
    {
        Quaternion start = transform.rotation;
        if (!Ready) return desired;
        GetBox(out Vector3 center, out Vector3 half, out Quaternion boxRotation);
        Vector3 pivot = transform.position;
        Transform box = weaponCollider.transform;
        Vector3 boxPosition = box.position;
        int steps = Mathf.Max(1, Mathf.CeilToInt(Quaternion.Angle(start, desired) / 2f));
        Quaternion accepted = start;
        float previousDepth = PenetrationDepth(center, half, boxRotation, boxPosition);
        for (int i = 1; i <= steps; i++)
        {
            Quaternion candidate = Quaternion.Slerp(start, desired, (float)i / steps);
            Quaternion delta = candidate * Quaternion.Inverse(start);
            float depth = PenetrationDepth(pivot + delta * (center - pivot), half,
                delta * boxRotation, pivot + delta * (boxPosition - pivot));
            if (depth > previousDepth + 0.0001f) break;
            accepted = candidate;
            previousDepth = depth;
        }
        return accepted;
    }

    private float PenetrationDepth(Vector3 center, Vector3 half, Quaternion rotation, Vector3 position)
    {
        float depth = 0f;
        foreach (Collider c in Physics.OverlapBox(center, half, rotation, obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            if (!IsObstacle(c)) continue;
            if (Physics.ComputePenetration(weaponCollider, position, rotation,
                c, c.transform.position, c.transform.rotation, out _, out float distance))
                depth = Mathf.Max(depth, distance);
        }
        return depth;
    }

    private void OnDisable() => BlocksFire = false;
}
