using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Units : MonoBehaviour
    {
        [SerializeField] private Vector3 targetPosition;

        private GridPosition gridPosition;
        [SerializeField] private float speed = 4f;
        [SerializeField] private float rotateSpeed = 10f;
        [SerializeField] private float stopDistance = .1f;
        private Vector3 moveDirection;

        private Animator animator;

        // Start is called before the first frame update
        void Awake()
        {
            targetPosition = transform.position;
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(transform.position, targetPosition) > stopDistance)
            {
                moveDirection = (targetPosition - transform.position).normalized;
                transform.position += moveDirection * Time.deltaTime * speed;
                transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition,newGridPosition);
                gridPosition = newGridPosition;
            }
        }

        public void Move(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }
    }
}
