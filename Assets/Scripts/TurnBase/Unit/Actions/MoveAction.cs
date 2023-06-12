using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TurnBase
{
    public class MoveAction : BaseAction
    {
        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField][Range(0, 100)] private int maxMoveDistance = 5;
        private float roundedEnd = 0.5f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 50f;
        [SerializeField] private float stopDistance = .1f;
        private Vector3 moveDirection;
        //private Animator animator;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            targetPosition = transform.position;
            //animator = GetComponentInChildren<Animator>();
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
            float differentAngle = Vector3.Angle(moveDirection, transform.forward);
            if (differentAngle > 0f)
            {
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                return;
            }

            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                transform.position += moveDirection * Time.deltaTime * moveSpeed;

                OnStartMoving?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            }
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            OnStartMoving?.Invoke(this, EventArgs.Empty);
            ActionStart(onActionComplete);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    float distance = (new Vector2(x, z)).magnitude;
                    if (distance > maxMoveDistance + roundedEnd) continue;
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
                }
            }
            return validGridPositionList;
        }

        public override string GetActionName()
        {
            return "Move";
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10,
            };
        }

    }
}
