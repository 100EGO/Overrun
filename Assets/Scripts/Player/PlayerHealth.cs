using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;

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
        {
            isDead = true;
            Debug.Log("Player defeated!", this);
            // Respawn and game-over screen will be added later.
        }
    }
}
