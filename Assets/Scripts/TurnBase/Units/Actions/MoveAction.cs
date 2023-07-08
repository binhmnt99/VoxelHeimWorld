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



        [SerializeField] private int maxMoveDistance = 4;
        [Header("Only LayerMask enemy")]
        [SerializeField] private LayerMask obstaclesLayerMask;

        private List<Vector3> positionList;
        private int currentPositionIndex;

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            Vector3 targetPosition = positionList[currentPositionIndex];
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            float stoppingDistance = .1f;
            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                float moveSpeed = 4f;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count)
                {
                    OnStopMoving?.Invoke(this, EventArgs.Empty);
                    transform.position = positionList[positionList.Count - 1];
                    ActionComplete();
                }
            }
        }


        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            List<GridPosition> pathGridPositionList = HexPathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

            currentPositionIndex = 0;
            positionList = new List<Vector3>();

            foreach (GridPosition pathGridPosition in pathGridPositionList)
            {
                positionList.Add(HexLevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }

            OnStartMoving?.Invoke(this, EventArgs.Empty);

            ActionStart(onActionComplete);
        }
        List<GridPosition> validGridPositionList = new List<GridPosition>();
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
                    if (!HexLevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    if (unitGridPosition == testGridPosition)
                    {
                        // Same Grid Position where the unit is already at
                        continue;
                    }
                    if (HexLevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        // Grid Position already occupied with another Unit
                        continue;
                    }
                    if (!HexPathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                    {
                        continue;
                    }
                    if (!HexPathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    {
                        continue;
                    }
                    int pathfindingDistanceMultiplier = 10;
                    if (HexPathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                    {
                        // Path length is too long
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);
                }
            }

            return validGridPositionList;
        }

        List<GridPosition> blockGridList = new List<GridPosition>();
        public List<GridPosition> GetValidEnemyActionGridPositionList(GridPosition unitGridPosition)
        {
            validGridPositionList.Clear();
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!HexLevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxMoveDistance)
                    {
                        continue;
                    }

                    if (!HexLevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    Unit targetUnit = HexLevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (unit.GetGridPosition() == testGridPosition)
                    {
                        continue;
                    }

                    Vector3 unitWorldPosition = HexLevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float unitShoulderHeight = 1.1f;
                    if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDirection,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask
                    ))
                    {
                        //Block by Obstacles
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);

                }
            }
            foreach (GridPosition gridPosition in blockGridList)
            {
                //Debug.Log("Remove blocked Grid " + gridPosition);
                validGridPositionList.Remove(gridPosition);
            }
            blockGridList.Clear();
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
