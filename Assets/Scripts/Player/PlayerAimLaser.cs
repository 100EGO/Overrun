using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerAimLaser : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 20f;
    [SerializeField] private float shotHeight = 0.5f;
    [SerializeField, Min(0.001f)] private float lineWidth = 0.035f;
    [SerializeField, Range(0f, 1f)] private float opacity = 0.45f;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.startColor = new Color(1f, 0f, 0f, opacity);
        line.endColor = new Color(1f, 0f, 0f, opacity);
        line.enabled = true;
    }

    private void LateUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * shotHeight;
        Vector3 direction = transform.forward;
        Vector3 end = origin + direction * range;

        // Ignore colliders belonging to this player, and find the closest obstacle.
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, range, ~0, QueryTriggerInteraction.Ignore);
        float closestDistance = range;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                continue;

            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                end = hit.point;
            }
        }

        line.SetPosition(0, origin);
        line.SetPosition(1, end);
    }

    private void OnValidate()
    {
        if (TryGetComponent(out LineRenderer renderer))
        {
            renderer.startWidth = lineWidth;
            renderer.endWidth = lineWidth;
            renderer.startColor = new Color(1f, 0f, 0f, opacity);
            renderer.endColor = new Color(1f, 0f, 0f, opacity);
        }
    }
}
