using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class MoveAction : BaseAction
    {
        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;
        private List<Vector3> positionList;
        private List<GridPosition> pathGridPositionList;
        private int currentPositionIndex;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 50f;
        [SerializeField] private float stopDistance = .05f;
        [SerializeField] private int maxMoveDistance = 5;

        protected override void Awake()
        {
            base.Awake();
            unit = GetComponent<Unit>();
            positionList = new List<Vector3>();
        }

        private void Start() {
            OnStartMoving += MoveAction_OnStartMoving;
            OnStopMoving += MoveAction_OnStopMoving;
        }

        private void MoveAction_OnStopMoving(object sender, EventArgs e)
        {
            Pathfinding.Instance.SetWalkableGridPosition(pathGridPositionList[pathGridPositionList.Count - 1], true);
            unit.FinalizePosition(pathGridPositionList[pathGridPositionList.Count - 1]);
        }

        private void MoveAction_OnStartMoving(object sender, EventArgs e)
        {
            Pathfinding.Instance.SetWalkableGridPosition(pathGridPositionList[0], true);
        }

        private void Update()
        {
            if (!isActive) return;
            Vector3 targetPosition = positionList[currentPositionIndex];
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.GetChild(0).forward = Vector3.Lerp(transform.GetChild(0).forward, moveDirection, Time.deltaTime * rotateSpeed);
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {

                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                currentPositionIndex++;
                if (currentPositionIndex >= positionList.Count)
                {
                    transform.position = targetPosition;
                    OnStopMoving?.Invoke(this,EventArgs.Empty);
                    ActionComplete();
                }
            }

        }

        public override void TakeAction(GridPosition gridPosition, Action action)
        {
            positionList.Clear();
            pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
            
            foreach (GridPosition pathGrid in pathGridPositionList)
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGrid));
            }
            currentPositionIndex = 0;
            OnStartMoving?.Invoke(this, EventArgs.Empty);
            ActionStart(action);

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

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                    if (unitGridPosition == testGridPosition) continue;

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) continue;

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxMoveDistance)
                    {
                        continue;
                    }

                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                    {
                        continue;
                    }

                    int pathDistanceMultiplier = 10;
                    if (Pathfinding.Instance.GetPathLength(unitGridPosition, testGridPosition) > maxMoveDistance * pathDistanceMultiplier)
                    {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }

        public override string GetActionName() => "Move";

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = targetCountAtGridPosition * 10
            };
        }
    }

}