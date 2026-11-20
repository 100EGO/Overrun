using UnityEngine;

[RequireComponent(typeof(EnemyTargeting))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField, Min(0f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float attacksPerSecond = 1f;

    private EnemyTargeting targeting;
    private float nextAttackTime;

    private void Awake()
    {
        targeting = GetComponent<EnemyTargeting>();
    }

    private void Update()
    {
        Transform target = targeting.Target;
        if (target == null || Time.time < nextAttackTime)
            return;

        Vector3 offset = target.position - transform.position;
        offset.y = 0f;

        if (offset.sqrMagnitude > attackRange * attackRange)
            return;

        targeting.DealDamage(damage);
        nextAttackTime = Time.time + 1f / attacksPerSecond;
    }
}
