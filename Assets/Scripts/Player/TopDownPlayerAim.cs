using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerAim : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;

    private void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;
    }

    private void Update()
    {
        if (aimCamera == null || Mouse.current == null)
            return;

        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Aim at a horizontal plane passing through the player's position.
        Plane aimPlane = new Plane(Vector3.up, transform.position);

        if (!aimPlane.Raycast(ray, out float distance))
            return;

        Vector3 mouseWorldPosition = ray.GetPoint(distance);
        Vector3 lookDirection = mouseWorldPosition - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }
}
