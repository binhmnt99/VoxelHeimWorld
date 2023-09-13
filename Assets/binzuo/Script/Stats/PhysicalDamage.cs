using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class PhysicalDamage : BaseStats
    {
        protected override void Awake()
        {
            base.Awake();
            StatsModifier flat = new StatsModifier(defaultValue, StatsModifierType.Flat, this);
            maxValue.AddModifier(flat);
            value = maxValue.value;
        }
        public override void CalculateValue(float _value){}

        public override void ResetValue()
        {
            value = defaultValue;
        }
    }

}
