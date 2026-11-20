using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private PlayerHealth target;
    [SerializeField, Min(0f)] private float attackRange = 1.5f;
    [SerializeField, Min(0f)] private float damage = 10f;
    [SerializeField, Min(0.1f)] private float attacksPerSecond = 1f;

    private float nextAttackTime;

    private void Update()
    {
        if (target == null || Time.time < nextAttackTime)
            return;

        Vector3 offset = target.transform.position - transform.position;
        offset.y = 0f;

        if (offset.sqrMagnitude > attackRange * attackRange)
            return;

        target.TakeDamage(damage);
        nextAttackTime = Time.time + 1f / attacksPerSecond;
    }
}
