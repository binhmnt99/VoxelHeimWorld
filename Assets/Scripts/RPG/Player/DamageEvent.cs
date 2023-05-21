using UnityEngine;
using UnityEngine.Events;

namespace Voxel
{
    public class DamageEvent : MonoBehaviour
    {
        public UnityEvent OnStartDealDamage;
        public UnityEvent OnEndDealDamage;

        public void StartDealDamage()
        {
            OnStartDealDamage?.Invoke();
        }

        public void EndDealDamage()
        {
            OnEndDealDamage?.Invoke();
        }
    }
}

