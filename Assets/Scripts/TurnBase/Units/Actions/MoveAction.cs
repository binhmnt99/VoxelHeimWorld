using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace TurnBase
{
    public class MoveAction : BaseAction
    {
        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;



        [SerializeField] private int maxMoveDistance = 4;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float moveSpeed = 5f;
        [Header("Only LayerMask enemy")]
        [SerializeField] private LayerMask obstaclesLayerMask;

        [SerializeField] private List<Vector3> positionList;
        public List<Vector3> GetPositionList()
        {
            return positionList;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            // Vector3 targetPosition = positionList[currentPositionIndex];
            // Vector3 moveDirection = (targetPosition - transform.position).normalized;

            // transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            // float stoppingDistance = .1f;
            // if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            // {
            //     Debug.Log("Move");

            //     transform.position += moveDirection * moveSpeed * Time.deltaTime;
            // }
            // else
            // {
            //     currentPositionIndex++;
            //     if (currentPositionIndex >= positionList.Count)
            //     {
            //         Debug.Log("EndMove");
            //         OnStopMoving?.Invoke(this, EventArgs.Empty);
            //         transform.position = positionList[positionList.Count - 1];
            //         ActionComplete();
            //     }
            // }
        }


        IEnumerator MoveAlongPath()
        {
            const float MIN_DISTANCE = 0.05f;

            int currentStep = 0;
            Vector3 currentPosition = positionList[0];
            float animationTime = 0f;

            while (currentStep < positionList.Count)
            {
                yield return null;

                //Move towards the next step in the path until we are closer than MIN_DIST
                Vector3 nextPosition = positionList[currentStep];
                Vector3 moveDirection = (nextPosition - transform.position).normalized;

                float movementTime = animationTime * moveSpeed;
                MoveAndRotate(currentPosition, nextPosition, moveDirection, movementTime);
                animationTime += Time.deltaTime;

                if (Vector3.Distance(transform.position, nextPosition) > MIN_DISTANCE)
                    continue;

                //Min dist has been reached, look to next step in path
                currentPosition = positionList[currentStep];
                currentStep++;
                animationTime = 0f;
            }
            if (currentStep >= positionList.Count)
            {
                Debug.Log("EndMove");
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                transform.position = positionList[positionList.Count - 1];
                ActionComplete();
            }
        }

        void MoveAndRotate(Vector3 origin, Vector3 destination, Vector3 moveDirection, float duration)
        {
            transform.position = Vector3.Lerp(origin, destination, duration);
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
            Debug.Log("Move");
        }


        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            Debug.Log("TakeAction");
            List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

            positionList = new List<Vector3>();

            foreach (GridPosition pathGridPosition in pathGridPositionList)
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
            }

            OnStartMoving?.Invoke(this, EventArgs.Empty);

            ActionStart(onActionComplete);

            StartCoroutine(MoveAlongPath());
        }
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            Profiler.BeginSample("GetValidActionGridPositionList");
            Debug.Log("GetValidActionGridPositionList");
            validGridPositionList.Clear();
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
                        // Same Grid Position where the unit is already at
                        continue;
                    }
                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        // Grid Position already occupied with another Unit
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

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (unit.GetGridPosition() == testGridPosition)
                    {
                        continue;
                    }

                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 testWorldPosition = LevelGrid.Instance.GetHexGridSystem().GetWorldPosition(testGridPosition);
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
                        GridPosition testGrid = LevelGrid.Instance.GetGridPosition(hit.collider.transform.position);
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
