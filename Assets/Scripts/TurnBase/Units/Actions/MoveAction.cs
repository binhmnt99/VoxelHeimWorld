using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace TurnBase
{
    public class MoveAction : BaseAction
    {
        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;



        [SerializeField] private int maxMoveDistance = 4;
        [Header("Only LayerMask enemy")]
        [SerializeField] private LayerMask obstaclesLayerMask;

        [SerializeField] private List<Vector3> positionList;
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
            Profiler.BeginSample("GetValidActionGridPositionList");
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
                    List<GridPosition> path =
                        HexPathfinding.Instance.FindPath(unitGridPosition, testGridPosition, out int pathLength);
                    if (path == null)
                    {
                        continue;
                    }
                    int pathfindingDistanceMultiplier = 10;
                    if (pathLength > maxMoveDistance * pathfindingDistanceMultiplier)
                    {
                        // Path length is too long
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);
                }
            }
            Profiler.EndSample();
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
                    Vector3 testWorldPosition = HexLevelGrid.Instance.GetHexGridSystem().GetWorldPosition(testGridPosition);
                    float distance = Vector3.Distance(unitWorldPosition, testWorldPosition);
                    if (distance > maxMoveDistance)
                    {
                        continue;
                    }

                    Vector3 testDirection = (testWorldPosition - unit.GetWorldPosition()).normalized;
                    distance = Vector3.Distance(unit.GetWorldPosition(), testWorldPosition);
                    RaycastHit[] hits = Physics.RaycastAll(unit.GetWorldPosition(), testDirection, distance, obstaclesLayerMask);
                    foreach (var hit in hits)
                    {
                        GridPosition testGrid = HexLevelGrid.Instance.GetGridPosition(hit.collider.transform.position);
                        //Debug.Log("When " + validGrid + " and distance " + distance + " Hit Grit " + testGrid);

                        if (testGrid != testGridPosition && !blockGridList.Contains(testGridPosition))
                        {
                            //Debug.Log("Remove blocked Grid " + validGrid);
                            blockGridList.Add(testGridPosition);
                        }
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
