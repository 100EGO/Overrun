using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0f)] private float stoppingDistance = 1.5f;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stoppingDistance)
            return;

        Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
        controller.Move(movement);

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
