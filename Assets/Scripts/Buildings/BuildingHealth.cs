using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxHealth = 500f;

    private float currentHealth;
    private bool isDestroyed;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDestroyed => isDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Debug.Log($"Building health: {currentHealth}/{maxHealth}", this);

        if (currentHealth <= 0f)
        {
            isDestroyed = true;
            Debug.Log("Building destroyed! Mission failed.", this);
            // Mission failure will be handled by a future mission manager.
        }
    }

    // Temporary test: select the building in the Inspector and use the component context menu.
    [ContextMenu("Test: Take 100 Damage")]
    private void TestTakeDamage()
    {
        if (Application.isPlaying)
            TakeDamage(100f);
    }
}
