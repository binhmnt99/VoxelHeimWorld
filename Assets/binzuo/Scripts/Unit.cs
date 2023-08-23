using UnityEngine;

namespace binzuo
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float stopDistance = .1f;
        private GridPosition gridPosition;

        private void Start() {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.SetUnitAtGridPosition(gridPosition, this);
        }
        private void Update()
        {
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            }

            if (Input.GetMouseButtonDown(0))
            {
                Move(MousePosition.GetPoint());
            }

            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }
        private void Move(Vector3 _targetPosition)
        {
            this.targetPosition = _targetPosition;
        }
    }
}

