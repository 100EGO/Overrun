using System.Collections.Generic;
using UnityEngine;

/// <summary>Horizontal clearance radius used by EnemyChase; not a physics collider.</summary>
public class TurretAvoidanceZone : MonoBehaviour
{
    public static readonly List<TurretAvoidanceZone> ActiveZones = new List<TurretAvoidanceZone>();

    [SerializeField, Min(0.2f)] private float radius = 2.5f;
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
            Vector3 p1 = center + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(b), 0f, Mathf.Sin(b)) * radius;
            Gizmos.DrawLine(p1, p2);
        }
    }
}
