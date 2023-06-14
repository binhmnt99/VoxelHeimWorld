using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GrenadeProjectile : MonoBehaviour
    {
        private Vector3 targetPosition;
        private float moveSpeed = 15f;
        private Action onGrenadeBehaviourComplete;

        void Update()
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            float reachedTargetDistance = .05f;
            if (Vector3.Distance(transform.position,targetPosition) < reachedTargetDistance)
            {
                float damageRadius = 4f;
                Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
                foreach (Collider targetCollider in colliderArray)
                {
                    if(targetCollider.TryGetComponent<Unit>(out Unit targetUnit))
                    {
                        targetUnit.Damage(10);
                    }
                }
                Destroy(gameObject);
                onGrenadeBehaviourComplete();
            }
        }
        public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
        {
            this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        }
    }
}