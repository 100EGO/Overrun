using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float range = 20f;
    [SerializeField, Min(0.1f)] private float shotsPerSecond = 4f;
    [SerializeField, Min(0f)] private float damage = 25f;
    [SerializeField] private float shotHeight = 0.5f;
    [SerializeField] private Transform muzzlePoint;
    [Header("Projectile")]
    [SerializeField] private CombatProjectile projectilePrefab;
    [SerializeField, Min(0.1f)] private float projectileSpeed = 35f;
    [Header("Reload / Animation")]
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
    public Transform MuzzlePoint => muzzlePoint;

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

        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlayerShooting: assign a CombatProjectile prefab in the Inspector.", this);
            return;
        }

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
        Debug.Log("Reload complete.");
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
        Vector3 origin = muzzlePoint != null
            ? muzzlePoint.position
            : transform.position + Vector3.up * shotHeight;
        Vector3 direction = transform.forward;

        CombatProjectile projectile = Instantiate(projectilePrefab, origin,
            Quaternion.LookRotation(direction));
        projectile.Initialize(direction, projectileSpeed, damage, range, transform.root);
    }
}
