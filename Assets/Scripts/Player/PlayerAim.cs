
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Camera")]
    [SerializeField] private Transform playerCamera;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 15f;


    [Header("Shoulder Camera")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float shoulderOffset = 0.45f;
    [SerializeField] private float shoulderTransitionTime = 0.15f;

    private Vector3 defaultCameraTargetPosition;
    private Vector3 cameraTargetVelocity;


    private InputAction playerAimAction;

    public bool IsAiming { get; private set; }

    private void Awake()
    {
        playerAimAction = inputActions
            .FindActionMap("Player")
            .FindAction("Aim");

        defaultCameraTargetPosition = cameraTarget.localPosition;
    }

    private void Update()
    {
        IsAiming = playerAimAction.IsPressed();

        if (IsAiming)
        {
            RotatePlayerTowardsCamera();
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = defaultCameraTargetPosition;

        if (IsAiming)
        {
            targetPosition += Vector3.right * shoulderOffset;
        }

        cameraTarget.localPosition = Vector3.SmoothDamp(
            cameraTarget.localPosition,
            targetPosition,
            ref cameraTargetVelocity,
            shoulderTransitionTime
        );
    }


    private void RotatePlayerTowardsCamera()
    {
        Vector3 cameraDirection = playerCamera.forward;

        // Ignore the vertical camera angle
        cameraDirection.y = 0f;

        if (cameraDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
