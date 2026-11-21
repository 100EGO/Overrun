using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogWarning("PlayerAnimation: no Animator found on the player or its children.", this);
    }

    private void LateUpdate()
    {
        if (animator == null)
            return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        float speed = velocity.sqrMagnitude > 0.01f ? 1f : 0f;
        animator.SetFloat(SpeedHash, speed);
    }
}
