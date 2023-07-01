using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class HealthSystem : MonoBehaviour
    {
        public event EventHandler OnDead;
        public event EventHandler OnDamage;
        public event EventHandler OnHealth;
        [SerializeField] int health = 10;
        private int healthMax;

        void Awake()
        {
            healthMax = health;
        }

        public void AddHealth(int amount)
        {
            health += amount;
            if (health >= healthMax)
            {
                health = healthMax;
            }
            OnHealth?.Invoke(this,EventArgs.Empty);
            Debug.Log("Health " + health);
        }
        public void Damage(int damageAmount)
        {
            health -= damageAmount;

            if (health <= 0)
            {
                health = 0;
            }
            OnDamage?.Invoke(this,EventArgs.Empty);
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
            return (float)health / healthMax;
        }
    }

}