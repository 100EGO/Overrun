using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 20f;
    [SerializeField, Min(0.1f)] private float shotsPerSecond = 4f;
    [SerializeField] private float shotHeight = 0.5f;

    private float nextShotTime;

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.isPressed)
            return;

        if (Time.time < nextShotTime)
            return;

        Shoot();
        nextShotTime = Time.time + 1f / shotsPerSecond;
    }

    private void Shoot()
    {
        Vector3 origin = transform.position + Vector3.up * shotHeight;
        Vector3 direction = transform.forward;

        // Visible in the Scene view while the game is running.
        Debug.DrawRay(origin, direction * range, Color.yellow, 0.25f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            Debug.Log("Shot hit: " + hit.collider.name, hit.collider);
        }
    }
}
