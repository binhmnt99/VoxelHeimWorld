using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class BulletProjectile : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Transform hitVFXPrefabs;
        private Vector3 targetPosition;
        private float projectileSpeed = 200f;
        private float distanceBeforeProjectile;
        private float distanceAfterProjectile;

        public void Setup(Vector3 position)
        {
            this.targetPosition = position;
        }

        private void Update()
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            distanceBeforeProjectile = Vector3.Distance(transform.position, targetPosition);

            transform.position += moveDir * projectileSpeed * Time.deltaTime;

            distanceAfterProjectile = Vector3.Distance(transform.position, targetPosition);

            if (distanceBeforeProjectile < distanceAfterProjectile)
            {
                transform.position = targetPosition;
                trailRenderer.transform.parent = null;
                Destroy(gameObject);
                Instantiate(hitVFXPrefabs, targetPosition,Quaternion.identity);
            }
        }
    }

}
