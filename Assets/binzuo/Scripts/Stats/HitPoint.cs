using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class HitPoint : BaseStats
    {
        public event EventHandler OnDead;

        protected override void Awake()
        {
            base.Awake();
            StatsModifier flat = new StatsModifier(defaultValue, StatsModifierType.Flat, this);
            maxValue.AddModifier(flat);
            value = maxValue.value;
        }

        public override void CalculateValue(float _value)
        {
            value -= _value;
            if (value <= 0)
            {
                value = 0;
            }
            if (value == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDead?.Invoke(this, EventArgs.Empty);
        }

        public override void ResetValue()
        {
            value = maxValue.value;
        }
    }

}