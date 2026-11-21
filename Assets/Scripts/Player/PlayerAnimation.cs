using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float animationDamping = 0.1f;

    private CharacterController controller;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int AutoFireHash = Animator.StringToHash("AutoFire");
    private static readonly int ReloadingHash = Animator.StringToHash("Reloading");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning("PlayerAnimation: no Animator found on the player or its children.", this);
    }

    public void PlayShoot()
    {
        if (animator != null)
            animator.SetTrigger(ShootHash);
    }

    public void SetAutoFire(bool active)
    {
        if (animator != null)
            animator.SetBool(AutoFireHash, active);
    }

    public void SetReloading(bool active)
    {
        if (animator == null)
            return;

        if (active)
        {
            animator.SetBool(AutoFireHash, false);
            animator.ResetTrigger(ShootHash);
        }

        animator.SetBool(ReloadingHash, active);
    }

    private void LateUpdate()
    {
        if (animator == null)
            return;

        Vector3 velocity = controller.velocity;
        velocity.y = 0f;

        float speed = velocity.sqrMagnitude > 0.01f ? 1f : 0f;
        Vector3 localMovement = transform.InverseTransformDirection(velocity);
        localMovement.y = 0f;
        localMovement = localMovement.sqrMagnitude > 0.01f ? localMovement.normalized : Vector3.zero;

        animator.SetFloat(SpeedHash, speed);
        animator.SetFloat(MoveXHash, localMovement.x, animationDamping, Time.deltaTime);
        animator.SetFloat(MoveYHash, localMovement.z, animationDamping, Time.deltaTime);
    }
}
