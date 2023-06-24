using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class UnitAnimator : MonoBehaviour
    {
        public event EventHandler OnShootAnim;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform arrowProjectilePrefab;
        [SerializeField] private Transform shootPoint;
        private Vector3 targetUnitShootAtPosition;
        private Unit targetUnit;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (TryGetComponent<MoveAction>(out MoveAction moveAction))
            {
                moveAction.OnStartMoving += MoveAction_OnStartMoving;
                moveAction.OnStopMoving += MoveAction_OnStopMoving;
            }
            if (TryGetComponent<ShootAction>(out ShootAction shootAction))
            {
                shootAction.OnShoot += ShootAction_OnShoot;
            }
        }

        private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            animator.SetTrigger("isBowShoot");
            targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
        }

        public void BowShoot()
        {
            Transform arrowTransform = Instantiate(arrowProjectilePrefab, shootPoint.position, Quaternion.identity);
            ArrowProjectile arrowProjectile = arrowTransform.GetComponent<ArrowProjectile>();
            targetUnitShootAtPosition.y = shootPoint.position.y;
            arrowProjectile.Setup(targetUnitShootAtPosition);
            OnShootAnim?.Invoke(this,EventArgs.Empty);
        }

        private void MoveAction_OnStopMoving(object sender, EventArgs e)
        {
            animator.SetBool("isWalking", false);
        }

        private void MoveAction_OnStartMoving(object sender, EventArgs e)
        {
            animator.SetBool("isWalking", true);
        }
    }
}
