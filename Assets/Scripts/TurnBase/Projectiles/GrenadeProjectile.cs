using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GrenadeProjectile : MonoBehaviour
    {
        public static event EventHandler OnAnyGrenadeExploded;

        [SerializeField] private Transform grenadeExploreVfxPrefab;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private AnimationCurve arcYAnimationCurve;
        [SerializeField] private float maxHeight = 3f;
        private float totalDistance;
        private Vector3 positionXZ;
        private Vector3 targetPosition;
        private float moveSpeed = 15f;
        private Action onGrenadeBehaviourComplete;
        private bool isEntityAction = false;

        void Update()
        {
            Vector3 moveDirection = (targetPosition - positionXZ).normalized;

            positionXZ += moveDirection * moveSpeed * Time.deltaTime;
            float distance = Vector3.Distance(positionXZ, targetPosition);
            float distanceNormalized = 1 - distance / totalDistance;
            float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
            transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
            float reachedTargetDistance = .05f;
            if (Vector3.Distance(positionXZ, targetPosition) < reachedTargetDistance)
            {
                float damageRadius = 4f;
                Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
                foreach (Collider targetCollider in colliderArray)
                {
                    if (targetCollider.TryGetComponent<Unit>(out Unit targetUnit))
                    {
                        if (targetUnit.IsEnemy() != isEntityAction)
                        {
                            targetUnit.Damage(3);
                        }
                    }
                    if (targetCollider.TryGetComponent<DestructibleCrate>(out DestructibleCrate targetCrate))
                    {
                        targetCrate.Damage();
                    }
                }
                OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
                trailRenderer.transform.parent = null;
                Instantiate(grenadeExploreVfxPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
                Destroy(gameObject);
                onGrenadeBehaviourComplete();
            }
        }
        public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete, bool isEnemyAction)
        {
            this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
            targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
            this.isEntityAction = isEnemyAction;
            positionXZ = transform.position;
            positionXZ.y = 0;
            totalDistance = Vector3.Distance(positionXZ, targetPosition);
        }
    }
}