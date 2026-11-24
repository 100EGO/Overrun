using UnityEngine;
using UnityEngine.Animations.Rigging;

[DefaultExecutionOrder(100)]
public class PlayerAimIK : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private Transform aimTarget;
    [SerializeField, Min(1f)] private float aimDistance = 100f;
    [SerializeField, Min(0.1f)] private float blendSpeed = 8f;
    private ThirdPersonCameraRig cameraRig;
    private PlayerShooting shooting;
    private PlayerHealth health;
    private TurretPlacement placement;

    private void Awake()
    {
        cameraRig = GetComponent<ThirdPersonCameraRig>();
        shooting = GetComponent<PlayerShooting>();
        health = GetComponent<PlayerHealth>();
        placement = GetComponent<TurretPlacement>();
        if (aimRig == null || aimTarget == null || cameraRig == null)
        {
            Debug.LogWarning("PlayerAimIK: assign Aim Rig and Aim Target.", this);
            enabled = false;
            return;
        }
        aimRig.weight = 0f;
    }

    private void Update()
    {
        bool active = cameraRig.isActiveAndEnabled && cameraRig.IsInCombat &&
            (shooting == null || !shooting.IsReloading) &&
            (health == null || !health.IsDead) &&
            (placement == null || !placement.BlocksShooting);
        Vector3 origin = shooting != null && shooting.MuzzlePoint != null
            ? shooting.MuzzlePoint.position : transform.position + Vector3.up * 1.5f;
        aimTarget.position = cameraRig.GetAimPoint(origin, aimDistance);
        aimRig.weight = Mathf.MoveTowards(aimRig.weight, active ? 1f : 0f,
            blendSpeed * Time.deltaTime);
    }

    private void OnDisable()
    {
        if (aimRig != null) aimRig.weight = 0f;
    }
}
