using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class ActionPoint : BaseStats
    {

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
        }

        public override void ResetValue()
        {
            value = maxValue.value;
        }
    }
}
