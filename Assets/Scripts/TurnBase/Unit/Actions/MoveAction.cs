using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private int maxMoveDistance = 5;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 50f;
        [SerializeField] private float stopDistance = .1f;
        private Vector3 moveDirection;
        private Animator animator;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            targetPosition = transform.position;
            animator = GetComponentInChildren<Animator>();
        }
        // Update is called once per frame
        void Update()
        {
            MoveDirection();
        }

        private void MoveDirection()
        {
            if (!isActive)
            {
                return;
            }
            moveDirection = (targetPosition - transform.position).normalized;
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                transform.position += moveDirection * Time.deltaTime * moveSpeed;
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
                isActive = false;
                onActionComplete();
            }
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            this.onActionComplete = onActionComplete;
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            isActive = true;
        }

        public override List<GridPosition> GetValidActionGridPositionsList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    if (unitGridPosition == testGridPosition)
                    {
                        continue;
                    }
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);
                    //Debug.Log(testGridPosition);
                }
            }
            return validGridPositionList;
        }

        public override bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidActionGridPositionsList();
            return validGridPositionList.Contains(gridPosition);
        }

        public override string GetActionName()
        {
            return "Move";
        }

    }
}
