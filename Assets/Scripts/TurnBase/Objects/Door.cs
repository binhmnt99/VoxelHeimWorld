using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Door : MonoBehaviour,IInteractable
    {
        public static event EventHandler OnAnyDoorOpened;
        public event EventHandler OnDoorOpened;
        [SerializeField] private bool isOpen;
        private GridPosition gridPosition;
        private Animator animator;
        private Action onInteractionComplete;
        private bool isActive;
        private float timer;
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        void Start()
        {
            gridPosition = HexLevelGrid.Instance.GetGridPosition(transform.position);
            HexLevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
            // if (isOpen)
            // {
            //     OpenDoor();
            // }
            // else
            // {
            //     CloseDoor();
            // }
        }

        void Update()
        {
            if (!isActive)
            {
                return;
            }
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                isActive = false;
                onInteractionComplete();
            }
        }
        public void Interact(Action onInteractionComplete)
        {
            this.onInteractionComplete = onInteractionComplete;
            isActive = true;
            timer = .5f;
            if (isOpen)
            {
                CloseDoor();
            }
            else
            {
                OpenDoor();
            }
        }
        private void OpenDoor()
        {
            isOpen = true;
            animator.SetBool("isOpen", isOpen);
            HexPathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);

            OnDoorOpened?.Invoke(this,EventArgs.Empty);
            OnAnyDoorOpened?.Invoke(this,EventArgs.Empty);
        }
        private void CloseDoor()
        {
            isOpen = false;
            animator.SetBool("isOpen", isOpen);
            HexPathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        }
    }

}