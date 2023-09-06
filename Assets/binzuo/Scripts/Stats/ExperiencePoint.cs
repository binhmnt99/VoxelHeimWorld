using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;
using QFSW.QC.Suggestors.Tags;

namespace binzuo
{
    public class ExperiencePoint : BaseStats
    {

        public event EventHandler OnLevelUp;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            float expValue = unit.GetStats<LevelPoint>().GetExperiencePointAtLevel(unit.GetStats<LevelPoint>().GetValue());
            StatsModifier flat = new StatsModifier(expValue, StatsModifierType.Flat, this);
            maxValue.AddModifier(flat);
            value = 0;
        }

        [Command("Add-Exp")]
        public override void CalculateValue(float _value)
        {
            value += _value;
            if (value >= maxValue.value)
            {
                value -= maxValue.value;
                OnLevelUp?.Invoke(this, EventArgs.Empty);
                maxValue.RemoveAllModifierFromSource(this);
                float expValue = unit.GetStats<LevelPoint>().GetExperiencePointAtLevel(unit.GetStats<LevelPoint>().GetValue());
                StatsModifier exp = new StatsModifier(expValue, StatsModifierType.Flat, this);
                maxValue.AddModifier(exp);
            }
        }

        public override void ResetValue()
        {
            value = defaultValue;
        }
    }
}

