using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public class WeaponDamageDealer : MonoBehaviour
    {
        private bool canDealDamage = false;

        List<GameObject> hasDealtDamage;

        [SerializeField] float weaponLength;
        [SerializeField] float weaponDamage = 0;
        [SerializeField] private LayerMask layerMask;

        public bool IsDealDamage()
        {
            return canDealDamage;
        }

        void Start()
        {
            canDealDamage = false;
            hasDealtDamage = new List<GameObject>();


        }

        private IEnumerator DealDamageCoroutine()
        {
            while (canDealDamage)
            {
                DealDamage();
                yield return null;
            }
        }
        private void DealDamage()
        {
            RaycastHit hit;
            Debug.DrawRay(transform.position, -transform.up * weaponLength, Color.red, 5f);
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
            {
                if (hit.transform.TryGetComponent(out Enemy enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    //Debug.Log("Dealing damage...");
                    enemy.TakeDamage(weaponDamage);
                    //enemy.HitVFX(hit.point);
                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }

        public void StartDealDamage()
        {

            canDealDamage = true;
            hasDealtDamage.Clear();
            StartCoroutine(DealDamageCoroutine());
        }
        public void EndDealDamage()
        {
            canDealDamage = false;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
        }
    }

}
