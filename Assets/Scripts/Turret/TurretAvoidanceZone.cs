using System.Collections.Generic;
using UnityEngine;

/// <summary>A small circular clearance area around a turret. No physics collider required.</summary>
public class TurretAvoidanceZone : MonoBehaviour
{
    public static readonly List<TurretAvoidanceZone> ActiveZones = new List<TurretAvoidanceZone>();

    [SerializeField, Min(0.2f)] private float radius = 1.2f;
    public float Radius => radius;

    private void OnEnable()
    {
        if (!ActiveZones.Contains(this))
            ActiveZones.Add(this);
    }

    private void OnDisable() => ActiveZones.Remove(this);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.85f, 1f, 0.9f);
        const int segments = 48;
        Vector3 center = transform.position;
        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            float b = (i + 1) * Mathf.PI * 2f / segments;
            Gizmos.DrawLine(center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius,
                center + new Vector3(Mathf.Cos(b), 0f, Mathf.Sin(b)) * radius);
        }
    }
}
