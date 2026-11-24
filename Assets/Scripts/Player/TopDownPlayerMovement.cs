using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TopDownPlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float sprintSpeed = 8f;
    [SerializeField, Min(0f)] private float gravity = 20f;
    [SerializeField] private float groundedVerticalSpeed = -2f;

    [Header("Jump")]
    [SerializeField, Min(0.1f)] private float jumpHeight = 1.2f;
    public bool IsAirborne { get; private set; }

    [Header("Stamina")]
    [SerializeField, Min(1f)] private float maxStamina = 100f;
    [SerializeField, Min(0f)] private float sprintDrainPerSecond = 25f;
    [SerializeField, Min(0f)] private float staminaRegenPerSecond = 20f;
    [SerializeField, Min(0f)] private float staminaRegenDelay = 1f;
    [SerializeField, Min(0f)] private float sprintResumeThreshold = 20f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public bool IsSprinting { get; private set; }

    private CharacterController controller;
    private float verticalSpeed;
    private float lastStaminaUseTime = float.NegativeInfinity;
    private bool exhausted;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        float horizontal = 0f;
        float vertical = 0f;
        bool sprintHeld = false;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;
            sprintHeld = keyboard.leftShiftKey.isPressed;
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        if (TryGetComponent(out ThirdPersonCameraRig rig) && rig.isActiveAndEnabled)
        {
            direction = (rig.Right * horizontal + rig.Forward * vertical).normalized;
            rig.RotateCharacter(direction);
            if (rig.IsInCombat) sprintHeld = false;
        }
        if (exhausted && CurrentStamina >= Mathf.Min(maxStamina, sprintResumeThreshold))
            exhausted = false;

        IsSprinting = sprintHeld && direction.sqrMagnitude > 0f && !exhausted && CurrentStamina > 0f;

        if (IsSprinting)
        {
            CurrentStamina = Mathf.Max(0f, CurrentStamina - sprintDrainPerSecond * Time.deltaTime);
            lastStaminaUseTime = Time.time;
            if (CurrentStamina <= 0f)
            {
                CurrentStamina = 0f;
                exhausted = true;
                IsSprinting = false;
            }
        }
        else if (Time.time >= lastStaminaUseTime + staminaRegenDelay)
        {
            CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + staminaRegenPerSecond * Time.deltaTime);
        }

        if (controller.isGrounded && verticalSpeed <= 0f)
        {
            verticalSpeed = groundedVerticalSpeed;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                verticalSpeed = Mathf.Sqrt(2f * Mathf.Max(0.01f, gravity) * jumpHeight);
        }
        else
            verticalSpeed -= gravity * Time.deltaTime;

        Vector3 velocity = direction * (IsSprinting ? sprintSpeed : moveSpeed) + Vector3.up * verticalSpeed;
        CollisionFlags flags = controller.Move(velocity * Time.deltaTime);

        IsAirborne = (flags & CollisionFlags.Below) == 0;
        if ((flags & CollisionFlags.Above) != 0 && verticalSpeed > 0f)
            verticalSpeed = 0f;

        if ((flags & CollisionFlags.Below) != 0 && verticalSpeed < 0f)
            verticalSpeed = groundedVerticalSpeed;
    }
}
