using UnityEngine;

public class AimRaycaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerAim playerAim;

    [Header("Settings")]
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask aimLayers = ~0;

    public bool HasHit { get; private set; }
    public RaycastHit HitInfo { get; private set; }
    public Vector3 AimPoint { get; private set; }

    private void Update()
    {
        if (!playerAim.IsAiming)
        {
            HasHit = false;
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimLayers))
        {
            HasHit = true;
            HitInfo = hit;
            AimPoint = hit.point;
        }
        else
        {
            HasHit = false;
            AimPoint = ray.GetPoint(maxDistance);
        }

        Debug.DrawRay(
            ray.origin,
            ray.direction * maxDistance,
            HasHit ? Color.green : Color.red
        );
    }
}