using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnBase
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }

        private Ray ray;
        private RaycastHit raycastHit;
        private UnitInput unitInput;
        public event EventHandler OnSelectedUnitChanged;
        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask layerMask;
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
            unitInput = GetComponent<UnitInput>();
        }

        // Update is called once per frame
        void Update()
        {

            if (unitInput.isLeftClicked)
            {
                if (TryHandleUnitSelection())
                {
                    return;
                }
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
                if (selectedUnit)
                {
                    if (selectedUnit.GetMoveAction().IsValidActionGridPosition(mouseGridPosition))
                    {
                        selectedUnit.GetMoveAction().Move(mouseGridPosition);
                    }
                }
            }
        }

        private bool TryHandleUnitSelection()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    SetSelectedUnit(unit);
                    return true;
                }

            }
            return false;
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }
    }
}

