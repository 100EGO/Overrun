using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Drives an independent tracking target for two Cinemachine shoulder cameras.</summary>
[DefaultExecutionOrder(-100)]
public class ThirdPersonCameraRig : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private GameObject explorationCamera;
    [SerializeField] private GameObject aimingCamera;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private float aimSensitivity = 0.07f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 65f;
    [Header("Character rotation")]
    [SerializeField, Min(1f)] private float movementRotationSpeed = 720f;
    [SerializeField, Min(1f)] private float combatRotationSpeed = 1080f;
    private float yaw;
    private float pitch = 10f;
    private PlayerHealth health;
    private TurretPlacement placement;
    public bool IsAiming { get; private set; }
    public bool IsInCombat => IsAiming ||
        (Mouse.current != null && Mouse.current.leftButton.isPressed &&
         (placement == null || !placement.BlocksShooting));
    public Vector3 Forward => Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
    public Vector3 Right => Quaternion.Euler(0f, yaw, 0f) * Vector3.right;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        placement = GetComponent<TurretPlacement>();
        yaw = transform.eulerAngles.y;
        if (cameraTarget == null || explorationCamera == null || aimingCamera == null ||
            explorationCamera == aimingCamera || cameraTarget.IsChildOf(transform))
        {
            Debug.LogError("ThirdPersonCameraRig: assign an independent CameraTarget and two distinct Cinemachine camera objects.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetCamera(false);
        UpdateTarget();
    }

    private void Update()
    {
        if (health != null && health.IsDead) return;
        Mouse mouse = Mouse.current;
        if (mouse == null) return;
        bool aiming = mouse.rightButton.isPressed && (placement == null || !placement.IsPlacing);
        if (aiming != IsAiming) SetCamera(aiming);
        Vector2 delta = mouse.delta.ReadValue() * (aiming ? aimSensitivity : sensitivity);
        yaw += delta.x;
        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
        UpdateTarget();
    }

    public void RotateCharacter(Vector3 movementDirection)
    {
        if (health != null && health.IsDead) return;
        bool combat = IsInCombat;
        Vector3 direction = combat ? Forward : movementDirection;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            (combat ? combatRotationSpeed : movementRotationSpeed) * Time.deltaTime);
    }

    private void LateUpdate() => UpdateTarget();

    private void UpdateTarget()
    {
        if (cameraTarget != null)
            cameraTarget.SetPositionAndRotation(transform.position + targetOffset, Quaternion.Euler(pitch, yaw, 0f));
    }

    private void SetCamera(bool aiming)
    {
        IsAiming = aiming;
        if (aimingCamera != null) aimingCamera.SetActive(aiming);
        if (explorationCamera != null) explorationCamera.SetActive(!aiming);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public Vector3 GetShotDirection(Vector3 origin, float range)
    {
        Camera cam = Camera.main;
        if (cam == null) return Forward;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float distance = range + Vector3.Distance(cam.transform.position, origin);
        Vector3 target = ray.GetPoint(distance);
        float nearest = distance;
        foreach (RaycastHit hit in Physics.RaycastAll(ray, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform.root == transform.root || hit.distance >= nearest) continue;
            nearest = hit.distance;
            target = hit.point;
        }
        Vector3 direction = target - origin;
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : ray.direction;
    }
}
