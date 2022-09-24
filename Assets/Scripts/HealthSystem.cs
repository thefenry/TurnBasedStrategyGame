using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 100;

    public event EventHandler OnDeath;
    public event EventHandler OnHealthChanged;

    private float _maxHealth;

    private void Awake()
    {
        _maxHealth = health;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health < 0)
        {
            health = 0;
        }

        if (health == 0)
        {
            Die();
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public float GetNormalizedHealth() => health / _maxHealth;
}
