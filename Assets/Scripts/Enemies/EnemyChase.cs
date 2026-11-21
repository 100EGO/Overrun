using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(EnemyTargeting))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.5f;

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
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.magnitude <= stoppingDistance) return;

        controller.Move(direction.normalized * moveSpeed * Time.deltaTime);
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
