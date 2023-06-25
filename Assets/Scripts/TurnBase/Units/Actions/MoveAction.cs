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
        [SerializeField] private List<Vector3> positionList;
        private int currentPositionIndex;
        [SerializeField] private int maxMoveDistance = 3;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 50f;
        [SerializeField] private float stopDistance = .01f;
        private Vector3 moveDirection;

        // Update is called once per frame

        void Update()
        {
            MoveDirection();
        }

        public int GetPositionListCount()
        {
            return positionList.Count;
        }

        private void MoveDirection()
        {
            if (!isActive)
            {
                return;
            }

            Vector3 targetPosition = positionList[currentPositionIndex];
            moveDirection = (targetPosition - transform.position).normalized;
            float differentAngle = Vector3.Angle(moveDirection, transform.forward);
            if (differentAngle > 0f)
            {
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                return;
            }
            //transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                transform.position += moveDirection * Time.deltaTime * moveSpeed;

                OnStartMoving?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count)
                {
                    Pathfinding.Instance.SetIsWalkableGridPosition(LevelGrid.Instance.GetGridPosition(positionList[positionList.Count - 1]), false);
                    transform.position = LevelGrid.Instance.GetWorldPosition(LevelGrid.Instance.GetGridPosition(positionList[positionList.Count - 1]));
                    OnStopMoving?.Invoke(this, EventArgs.Empty);
                    //positionList.Clear();
                    ActionComplete();
                }
            }
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
            //Debug.Log(pathGridPositionList);
            currentPositionIndex = 0;
            positionList = new List<Vector3>();
            foreach (GridPosition pathGridPosition in pathGridPositionList)
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }
            Pathfinding.Instance.SetIsWalkableGridPosition(LevelGrid.Instance.GetGridPosition(positionList[0]), true);
            OnStartMoving?.Invoke(this, EventArgs.Empty);
            ActionStart(onActionComplete);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();
            maxMoveDistance = unit.GetMovePoints()*2;
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z, 0);
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
                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    List<GridPosition> path =
                        Pathfinding.Instance.FindPath(unitGridPosition, testGridPosition, out int pathLength);
                    if (path == null)
                    {
                        continue;
                    }
                    int pathfindingDistanceMultiplier = 10;
                    if (pathLength > maxMoveDistance * pathfindingDistanceMultiplier)
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

        public override int GetActionPointsCost()
        {
            return 0;
        }

        public override int GetMovePointsCost()
        {
            return base.GetMovePointsCost();
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
