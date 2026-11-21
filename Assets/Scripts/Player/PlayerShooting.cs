using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 20f;
    [SerializeField, Min(0.1f)] private float shotsPerSecond = 4f;
    [SerializeField, Min(0f)] private float damage = 25f;
    [SerializeField] private float shotHeight = 0.5f;
    [SerializeField, Min(0f)] private float heldFireAnimationDelay = 0.3f;

    private PlayerAnimation playerAnimation;
    private float nextShotTime;
    private float buttonPressedTime;
    private bool wasFireHeld;

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        bool fireHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (!fireHeld)
        {
            if (wasFireHeld)
                playerAnimation?.SetAutoFire(false);

            wasFireHeld = false;
            return;
        }

        if (!wasFireHeld)
        {
            buttonPressedTime = Time.time;
            playerAnimation?.SetAutoFire(false);
            // Trigger the single-shot pose once per press, not once per bullet.
            playerAnimation?.PlayShoot();
        }

        wasFireHeld = true;
        playerAnimation?.SetAutoFire(Time.time - buttonPressedTime >= heldFireAnimationDelay);

        if (Time.time < nextShotTime)
            return;

        Shoot();
        nextShotTime = Time.time + 1f / shotsPerSecond;
    }

    private void OnDisable()
    {
        wasFireHeld = false;
        playerAnimation?.SetAutoFire(false);
    }

    private void Shoot()
    {
        Vector3 origin = transform.position + Vector3.up * shotHeight;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * range, Color.yellow, 0.25f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            Debug.Log("Shot hit: " + hit.collider.name, hit.collider);

            if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth)
                enemyHealth.TakeDamage(damage);
        }
    }
}
