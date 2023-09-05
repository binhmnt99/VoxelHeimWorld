using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class ActionPoint : MonoBehaviour
    {
        [SerializeField] private int defaultValue;
        [SerializeField] private Stats maxValue;
        [SerializeField] private int currentValue;

        private void Awake()
        {
            maxValue = new Stats(defaultValue);
            currentValue = (int)maxValue.baseValue;
        }

        public int GetMaxValue() => (int)maxValue.baseValue;

        public int GetCurrentValue() => currentValue;

        public void CurrentValue(int amount)
        {
            currentValue -= amount;
            if (currentValue <= 0)
            {
                currentValue = 0;
            }
        }
    }
}
