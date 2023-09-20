using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform projectilePrefabs;
        [SerializeField] private Transform projectilePointTransform;

        private void Awake() {
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
            animator.SetTrigger("isAttack");
            Transform bulletTransform = Instantiate(projectilePrefabs, projectilePointTransform.position, Quaternion.identity);
            BulletProjectile projectile = bulletTransform.GetComponent<BulletProjectile>();
            Vector3 targetPosition = e.targetUnit.GetWorldPosition();
            targetPosition.y = projectilePointTransform.position.y;
            projectile.Setup(targetPosition);
        }

        private void MoveAction_OnStopMoving(object sender, EventArgs e)
        {
            animator.SetBool("isMove", false);
        }

        private void MoveAction_OnStartMoving(object sender, EventArgs e)
        {
            animator.SetBool("isMove", true);
        }
    }
}

