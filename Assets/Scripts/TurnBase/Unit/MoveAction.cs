using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MoveAction : MonoBehaviour
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private int maxMoveDistance = 5;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float stopDistance = .1f;
        private Vector3 moveDirection;
        private Animator animator;
        private Unit unit;

        // Start is called before the first frame update
        void Awake()
        {
            targetPosition = transform.position;
            animator = GetComponentInChildren<Animator>();
            unit = GetComponent<Unit>();
        }
        // Update is called once per frame
        void Update()
        {
            MoveDirection();
        }

        private void MoveDirection()
        {
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * Time.deltaTime * moveSpeed;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        public void Move(GridPosition gridPosition)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        }

        public List<GridPosition> GetValidGridPositionsList()
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

        public bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = GetValidGridPositionsList();
            return validGridPositionList.Contains(gridPosition);
        }
    }
}
