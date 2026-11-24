using UnityEngine;

[DefaultExecutionOrder(100)]
public class PlayerAimIK : MonoBehaviour
{
    [SerializeField] private Transform spineBone;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float rotationMultiplier = 1f;
    [SerializeField] private float minAngle = -35f;
    [SerializeField] private float maxAngle = 65f;
    [SerializeField, Min(0.1f)] private float blendSpeed = 8f;
    [SerializeField] private bool invertAxis;
    private ThirdPersonCameraRig cameraRig;
    private PlayerShooting shooting;
    private PlayerHealth health;
    private TurretPlacement placement;
    private float angle;
    private Quaternion originalRotation;
    private Quaternion appliedRotation;
    private bool applied;

    private void Awake()
    {
        cameraRig = GetComponent<ThirdPersonCameraRig>();
        shooting = GetComponent<PlayerShooting>();
        health = GetComponent<PlayerHealth>();
        placement = GetComponent<TurretPlacement>();
        if (spineBone == null || cameraRig == null)
        {
            Debug.LogWarning("PlayerAimIK: assign Spine Bone.", this);
            enabled = false;
        }
    }

    private void Update() => RestorePose();

    private void LateUpdate()
    {
        bool active = cameraRig.isActiveAndEnabled && cameraRig.IsInCombat &&
            (shooting == null || !shooting.IsReloading) &&
            (health == null || !health.IsDead) &&
            (placement == null || !placement.BlocksShooting);
        float target = active ? Mathf.Clamp(cameraRig.Pitch * rotationMultiplier, minAngle, maxAngle) : 0f;
        angle = Mathf.Lerp(angle, target, 1f - Mathf.Exp(-blendSpeed * Time.deltaTime));
        if (rotationAxis.sqrMagnitude < 0.0001f) return;
        originalRotation = spineBone.localRotation;
        appliedRotation = originalRotation * Quaternion.AngleAxis(invertAxis ? -angle : angle, rotationAxis.normalized);
        spineBone.localRotation = appliedRotation;
        applied = true;
    }

    private void RestorePose()
    {
        if (applied && spineBone != null && Quaternion.Angle(spineBone.localRotation, appliedRotation) < 0.01f)
            spineBone.localRotation = originalRotation;
        applied = false;
    }

    private void OnDisable()
    {
        RestorePose();
        angle = 0f;
    }
}
