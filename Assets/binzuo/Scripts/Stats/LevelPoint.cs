using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace binzuo
{
    public class LevelPoint : BaseStats
    {
        [SerializeField] private List<UnitLevel> unitLevels;

        protected override void Awake()
        {
            base.Awake();
            defaultValue = unitLevels.Capacity;
            StatsModifier flat = new StatsModifier(defaultValue, StatsModifierType.Flat, this);
            maxValue.AddModifier(flat);
            value = unitLevels[0].GetLevelPoint();
        }

        public void LevelUp()
        {
            if (value < maxValue.value)
            {
                value += 1;
            }
        }

        public float GetExperiencePointAtLevel(float levelPoint)
        {
            foreach (UnitLevel unitLevel in unitLevels)
            {
                if (unitLevel.GetLevelPoint() == levelPoint)
                {
                    return unitLevel.GetExperiencePoint();
                }
            }
            return 0f;
        }

        public override void CalculateValue(float _value){}

        public override void ResetValue()
        {
            value = 1;
        }
    }

    [Serializable]
    public class UnitLevel
    {
        [SerializeField] private int levelPoint;
        [SerializeField] private int experiencePoint;

        public int GetLevelPoint() => levelPoint;
        public void SetLevelPoint(int flatValue) => levelPoint = flatValue;
        public int GetExperiencePoint() => experiencePoint;
        public void SetExperiencePoint(int flatValue) => experiencePoint = flatValue;
    }
}


