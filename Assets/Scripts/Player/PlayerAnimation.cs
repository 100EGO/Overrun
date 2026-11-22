using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField, Min(0f)] private float animationDamping = 0.1f;

    private CharacterController controller;
    private TopDownPlayerMovement movement;
    private bool wasSprinting;
    private bool isDead;
    private static readonly int SprintHash = Animator.StringToHash("sprint");
    private static readonly int SprintActiveHash = Animator.StringToHash("SprintActive");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int AutoFireHash = Animator.StringToHash("AutoFire");
    private static readonly int ReloadingHash = Animator.StringToHash("Reloading");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        movement = GetComponent<TopDownPlayerMovement>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogWarning("PlayerAnimation: no Animator found on the player or its children.", this);
    }

    public void PlayShoot()
    {
        if (!isDead && animator != null)
            animator.SetTrigger(ShootHash);
    }

    public void SetAutoFire(bool active)
    {
        if (!isDead && animator != null)
            animator.SetBool(AutoFireHash, active);
    }

    public void SetReloading(bool active)
    {
        if (isDead || animator == null)
            return;

        if (active)
        {
            animator.SetBool(AutoFireHash, false);
            animator.ResetTrigger(ShootHash);
        }
        animator.SetBool(ReloadingHash, active);
    }

    public void PlayDeath()
    {
        if (isDead)
            return;

        isDead = true;
        if (animator == null)
            return;

        animator.SetBool(AutoFireHash, false);
        animator.SetBool(ReloadingHash, false);
        animator.ResetTrigger(ShootHash);
        animator.ResetTrigger(SprintHash);
        animator.SetBool(SprintActiveHash, false);
        animator.SetFloat(SpeedHash, 0f);
        animator.SetFloat(MoveXHash, 0f);
        animator.SetFloat(MoveYHash, 0f);

        int upperBodyLayer = animator.GetLayerIndex("UpperBody");
        if (upperBodyLayer >= 0)
            animator.SetLayerWeight(upperBodyLayer, 0f);

        animator.SetTrigger(DieHash);
    }

    private void LateUpdate()
    {
        if (animator == null || isDead)
            return;

        bool sprinting = movement != null && movement.IsSprinting;
        if (sprinting && !wasSprinting)
            animator.SetTrigger(SprintHash);
        else if (!sprinting && wasSprinting)
            animator.ResetTrigger(SprintHash);
        animator.SetBool(SprintActiveHash, sprinting);
        wasSprinting = sprinting;

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
