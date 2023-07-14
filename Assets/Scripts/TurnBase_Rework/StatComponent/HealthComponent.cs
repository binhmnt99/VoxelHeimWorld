using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public event EventHandler OnDead;
    public event EventHandler OnDamage;
    public event EventHandler OnHealth;

    [SerializeField]private float health;
    private float max_health;

    public void Setup(float value)
    {
        health = value;
        max_health = health;
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health >= max_health)
        {
            health = max_health;
        }
        OnHealth?.Invoke(this, EventArgs.Empty);
    }
    public void Damage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            health = 0;
        }
        OnDamage?.Invoke(this, EventArgs.Empty);
        if (health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized()
    {
        return health / max_health;
    }
}
