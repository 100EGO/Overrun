using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        Debug.Log($"Player health: {currentHealth}/{maxHealth}", this);

        if (currentHealth <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;
        // Keep the GameObject and Animator active so the death animation can play.
        if (TryGetComponent(out TopDownPlayerMovement movement))
            movement.enabled = false;
        if (TryGetComponent(out TopDownPlayerAim aim))
            aim.enabled = false;
        if (TryGetComponent(out PlayerShooting shooting))
            shooting.enabled = false;
        if (TryGetComponent(out PlayerAnimation animation))
            animation.PlayDeath();

        Debug.Log("Player defeated!", this);
    }
}
