using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public abstract class BaseStats : MonoBehaviour
    { 
        protected Unit unit;

        [SerializeField] protected float defaultValue = 0;
        [SerializeField] protected Stats maxValue;
        [SerializeField] protected float value; 
        
        protected virtual void Awake() {
            unit = GetComponent<Unit>();
        }

        public virtual float GetBaseMaxValue() => maxValue.value;

        public virtual Stats GetStats() => maxValue;

        public virtual float GetValue() => value;

        public virtual void SetValue(float _value) => value = _value;

        public abstract void ResetValue();

        public abstract void CalculateValue(float _value);

    }
}

