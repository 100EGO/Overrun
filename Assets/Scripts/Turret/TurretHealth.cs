using System.Collections.Generic;
using UnityEngine;

public class TurretHealth : MonoBehaviour
{
    public static readonly List<TurretHealth> ActiveTurrets = new List<TurretHealth>();
    [SerializeField, Min(1f)] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake() => currentHealth = maxHealth;
    private void OnEnable()
    {
        if (!ActiveTurrets.Contains(this)) ActiveTurrets.Add(this);
    }
    private void OnDisable() => ActiveTurrets.Remove(this);

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || currentHealth <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        Debug.Log($"{name} turret: {currentHealth}/{maxHealth} HP", this);
        if (currentHealth <= 0f)
        {
            ActiveTurrets.Remove(this);
            Destroy(gameObject);
        }
    }
}
