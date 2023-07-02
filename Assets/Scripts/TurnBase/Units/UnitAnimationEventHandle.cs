using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TurnBase
{
    public class UnitAnimationEventHandle : MonoBehaviour
    {
        public UnityEvent OnDamage;
        public void Damage()
        {
            //Debug.Log("Damage");
            OnDamage.Invoke();
        }
    }
}