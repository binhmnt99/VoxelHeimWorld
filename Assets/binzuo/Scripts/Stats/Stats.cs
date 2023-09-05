using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace binzuo
{
    [Serializable]
    public class Stats
    {
        public float baseValue;

        public virtual float value
        {
            get
            {
                if (isDirty || baseValue != lastBaseValue)
                {
                    lastBaseValue = baseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }

        protected bool isDirty = true;

        protected float _value;
        protected float lastBaseValue = float.MinValue;

        protected readonly List<StatsModifier> statModifiers;
        public readonly ReadOnlyCollection<StatsModifier> readOnlyStatModifiers;
        public Stats()
        {
            statModifiers = new();
            readOnlyStatModifiers = statModifiers.AsReadOnly();
        }

        public Stats(float baseValue) : this()
        {
            this.baseValue = baseValue;
        }

        public virtual void AddModifier(StatsModifier statModifier)
        {
            isDirty = true;
            statModifiers.Add(statModifier);
            statModifiers.Sort(CompareModifierOrder);
        }

        public virtual bool RemoveModifier(StatsModifier statModifier)
        {
            if (statModifiers.Remove(statModifier))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifierFromSource(object source)
        {
            bool remove = false;
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].source == source)
                {
                    isDirty = true;
                    remove = true;
                    statModifiers.RemoveAt(i);
                }
            }
            return remove;
        }

        protected virtual int CompareModifierOrder(StatsModifier a, StatsModifier b)
        {
            if (a.order < b.order)
            {
                return -1;
            }
            if (a.order > b.order)
            {
                return 1;
            }
            return 0;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;
            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatsModifier modifier = statModifiers[i];
                if (modifier.type == StatsModifierType.Flat)
                {
                    finalValue += modifier.value;
                }
                if (modifier.type == StatsModifierType.PercentAdd)
                {
                    sumPercentAdd += modifier.value;
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatsModifierType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                if (modifier.type == StatsModifierType.PercentMulti)
                {
                    finalValue *= 1 + modifier.value;
                }
            }
            return (float)Math.Round(finalValue, 4);
        }
    }
}
