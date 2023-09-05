using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class MoveAction : BaseAction
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 50f;
        [SerializeField] private float stopDistance = .05f;
        [SerializeField] private int maxMoveDistance = 5;

        protected override void Awake()
        {
            base.Awake();
            unit = GetComponent<Unit>();
            targetPosition = transform.position;
        }

        private void Update()
        {
            if(!isActive) return;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {

                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, moveDirection, Time.deltaTime * rotateSpeed);
            }
            else
            {
                transform.position = targetPosition;
                isActive = false;
                this.onActionComplete();
            }

        }

        public override void TakeAction(GridPosition gridPosition, Action action)
        {
            this.onActionComplete = action;
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            isActive = true;
        }

        List<GridPosition> validGridPositionList = new();
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            validGridPositionList.Clear();
            GridPosition unitGridPosition = unit.GetGridPosition();
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue; // inside grid position range

                    if (unitGridPosition == testGridPosition) continue; // same grid position where unit is already at

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // grid position already occupied with another unit;

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }

        public override string GetActionName() => "Move";
    }

}