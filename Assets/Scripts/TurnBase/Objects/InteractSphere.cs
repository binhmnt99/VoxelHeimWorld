using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class InteractSphere : MonoBehaviour, IInteractable
    {
        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private MeshRenderer meshRenderer;
        private GridPosition gridPosition;
        private bool isGreen;
        private Action onInteractionComplete;
        private bool isActive;
        private float timer;
        void Start()
        {
            gridPosition = HexLevelGrid.Instance.GetGridPosition(transform.position);
            HexLevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

            SetColorGreen();
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
        private void SetColorGreen()
        {
            isGreen = true;
            meshRenderer.material = greenMaterial;
        }
        private void SetColorRed()
        {
            isGreen = false;
            meshRenderer.material = redMaterial;
        }

        public void Interact(Action onInteractionComplete)
        {
            UnitActionSystem.Instance.GetSelectedUnit().gameObject.TryGetComponent<HealthSystem>(out HealthSystem healthSystem);
            this.onInteractionComplete = onInteractionComplete;
            isActive = true;
            timer = .5f;
            if (isGreen)
            {
                SetColorRed();
                healthSystem.AddHealth(5);
            }
            else
            {
                SetColorGreen();
                healthSystem.AddHealth(3);
            }
        }
    }

}