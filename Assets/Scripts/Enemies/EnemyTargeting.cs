using UnityEngine;

public enum EnemyTargetType
{
    Player,
    Building
}

public class EnemyTargeting : MonoBehaviour
{
    [SerializeField] private EnemyTargetType targetType = EnemyTargetType.Player;
    [Header("Turret priority")]
    [SerializeField] private bool prioritizeTurrets = true;
    [SerializeField, Min(0.1f)] private float turretDetectionRange = 20f;

    private Transform originalTarget;
    private PlayerHealth playerHealth;
    private BuildingHealth buildingHealth;
    private TurretHealth turretTarget;

    public Transform Target
    {
        get
        {
            if (originalTarget == null) FindTarget();
            RefreshTurretTarget();
            return turretTarget != null ? turretTarget.transform : originalTarget;
        }
    }

    public void DealDamage(float damage)
    {
        Transform current = Target;
        if (current == null) return;

        if (turretTarget != null)
            turretTarget.TakeDamage(damage);
        else if (targetType == EnemyTargetType.Player && playerHealth != null)
            playerHealth.TakeDamage(damage);
        else if (targetType == EnemyTargetType.Building && buildingHealth != null)
            buildingHealth.TakeDamage(damage);
    }

    private void Awake() => FindTarget();

    private void FindTarget()
    {
        if (targetType == EnemyTargetType.Player)
        {
            playerHealth = FindAnyObjectByType<PlayerHealth>();
            originalTarget = playerHealth != null ? playerHealth.transform : null;
        }
        else
        {
            buildingHealth = FindAnyObjectByType<BuildingHealth>();
            originalTarget = buildingHealth != null ? buildingHealth.transform : null;
        }
    }

    private void RefreshTurretTarget()
    {
        if (!prioritizeTurrets)
        {
            turretTarget = null;
            return;
        }

        // Keep a living turret target until it is destroyed or leaves the acquisition radius.
        float maxSqr = turretDetectionRange * turretDetectionRange;
        if (turretTarget != null && HorizontalDistanceSqr(turretTarget.transform.position) <= maxSqr)
            return;

        turretTarget = null;
        float nearest = maxSqr;
        foreach (TurretHealth turret in TurretHealth.ActiveTurrets)
        {
            if (turret == null || turret.CurrentHealth <= 0f) continue;
            float sqr = HorizontalDistanceSqr(turret.transform.position);
            if (sqr < nearest)
            {
                nearest = sqr;
                turretTarget = turret;
            }
        }
    }

    private float HorizontalDistanceSqr(Vector3 point)
    {
        Vector3 delta = point - transform.position;
        delta.y = 0f;
        return delta.sqrMagnitude;
    }
}
