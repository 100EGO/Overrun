using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 20f;
    [SerializeField, Min(0.1f)] private float shotsPerSecond = 4f;
    [SerializeField, Min(0f)] private float damage = 25f;
    [SerializeField] private float shotHeight = 0.5f;
    [SerializeField, Min(0f)] private float heldFireAnimationDelay = 0.3f;
    [SerializeField, Min(1)] private int magazineSize = 30;
    [SerializeField, Min(0.1f)] private float reloadDuration = 2f;

    private PlayerAnimation playerAnimation;
    private int ammoInMagazine;
    private bool isReloading;
    private float reloadEndTime;
    private float nextShotTime;
    private float buttonPressedTime;
    private bool wasFireHeld;

    public int AmmoInMagazine => ammoInMagazine;
    public int MagazineSize => magazineSize;
    public bool IsReloading => isReloading;

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        ammoInMagazine = magazineSize;
    }

    private void Update()
    {
        if (isReloading && Time.time >= reloadEndTime)
            FinishReload();

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            StartReload();

        if (isReloading)
        {
            StopFireAnimation();
            return;
        }

        bool fireHeld = Mouse.current != null && Mouse.current.leftButton.isPressed;
        if (!fireHeld)
        {
            StopFireAnimation();
            return;
        }

        if (ammoInMagazine <= 0)
        {
            StartReload();
            StopFireAnimation();
            return;
        }

        if (!wasFireHeld)
        {
            buttonPressedTime = Time.time;
            playerAnimation?.SetAutoFire(false);
            playerAnimation?.PlayShoot();
        }

        wasFireHeld = true;
        playerAnimation?.SetAutoFire(Time.time - buttonPressedTime >= heldFireAnimationDelay);

        if (Time.time < nextShotTime)
            return;

        Shoot();
        ammoInMagazine--;
        nextShotTime = Time.time + 1f / shotsPerSecond;

        if (ammoInMagazine == 0)
            StartReload();
    }

    private void StartReload()
    {
        if (isReloading || ammoInMagazine >= magazineSize)
            return;

        isReloading = true;
        reloadEndTime = Time.time + reloadDuration;
        StopFireAnimation();
        playerAnimation?.SetReloading(true);
        Debug.Log("Reloading...", this);
    }

    private void FinishReload()
    {
        ammoInMagazine = magazineSize;
        isReloading = false;
        playerAnimation?.SetReloading(false);
        Debug.Log("Reload complete.", this);
    }

    private void StopFireAnimation()
    {
        if (wasFireHeld)
            playerAnimation?.SetAutoFire(false);
        wasFireHeld = false;
    }

    private void OnDisable()
    {
        isReloading = false;
        StopFireAnimation();
        playerAnimation?.SetReloading(false);
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
