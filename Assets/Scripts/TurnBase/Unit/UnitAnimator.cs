using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform arrowProjectilePrefab;
        [SerializeField] private Transform shootPoint;

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
            Transform arrowTransform = Instantiate(arrowProjectilePrefab, shootPoint.position, Quaternion.identity);
            ArrowProjectile arrowProjectile = arrowTransform.GetComponent<ArrowProjectile>();
            Vector3 targetUnitShootAtPosition = e.targetUnit.GetWorldPosition();
            targetUnitShootAtPosition.y = shootPoint.position.y;
            arrowProjectile.Setup(targetUnitShootAtPosition);
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
