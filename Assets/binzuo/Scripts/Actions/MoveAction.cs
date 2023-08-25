using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class MoveAction : MonoBehaviour
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float stopDistance = .1f;
        [SerializeField] private int maxMoveDistance = 5;
        private Unit unit;

        private void Awake()
        {
            unit = GetComponent<Unit>();
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            }

        }

        public void Move(GridPosition gridPosition)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        }

        public bool IsValidAvtionGridPosition(GridPosition gridPosition) => GetValidActionGridPositionList().Contains(gridPosition);

        List<GridPosition> validGridPositionList = new();
        public List<GridPosition> GetValidActionGridPositionList()
        {
            validGridPositionList.Clear();
            GridPosition unitGridPosition = unit.GetGridPosition();
            for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            {
                for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x,z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue; // inside grid position range

                    if(unitGridPosition == testGridPosition) continue; // same grid position where unit is already at

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue; // grid position already occupied with another unit;
                    
                    validGridPositionList.Add(testGridPosition);
                }
            }
            return validGridPositionList;
        }
    }

}