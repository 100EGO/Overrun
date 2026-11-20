using UnityEngine;

public enum EnemyTargetType
{
    Player,
    Building
}

public class EnemyTargeting : MonoBehaviour
{
    [SerializeField] private EnemyTargetType targetType = EnemyTargetType.Player;

    private Transform target;
    private PlayerHealth playerHealth;
    private BuildingHealth buildingHealth;

    public Transform Target
    {
        get
        {
            if (target == null)
                FindTarget();
            return target;
        }
    }

    public void DealDamage(float damage)
    {
        if (Target == null)
            return;

        if (targetType == EnemyTargetType.Player && playerHealth != null)
            playerHealth.TakeDamage(damage);
        else if (targetType == EnemyTargetType.Building && buildingHealth != null)
            buildingHealth.TakeDamage(damage);
    }

    private void Awake()
    {
        FindTarget();
    }

    private void FindTarget()
    {
        if (targetType == EnemyTargetType.Player)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
            target = playerHealth != null ? playerHealth.transform : null;
        }
        else
        {
            buildingHealth = FindFirstObjectByType<BuildingHealth>();
            target = buildingHealth != null ? buildingHealth.transform : null;
        }
    }
}
