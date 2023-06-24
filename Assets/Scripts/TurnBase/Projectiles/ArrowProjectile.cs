using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class ArrowProjectile : MonoBehaviour
    {
        private Vector3 targetPosition;
        [SerializeField] private TrailRenderer trailRenderer;

        [SerializeField] private Transform arrowVFXPrefab;
        [SerializeField] private Transform arrowObject;

        void Awake()
        {
            trailRenderer = transform.GetChild(0).GetComponent<TrailRenderer>();
        }
        public void Setup(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        void Update()
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
            float moveSpeed = 200f;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            arrowObject.transform.rotation = Quaternion.Slerp(arrowObject.transform.rotation, targetRotation, 200f * Time.deltaTime);

            float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

            if (distanceBeforeMoving < distanceAfterMoving)
            {
                transform.position = targetPosition;
                trailRenderer.transform.parent = null;
                Destroy(gameObject);

                Instantiate(arrowVFXPrefab, targetPosition, Quaternion.identity);
            }
        }
    }

}
