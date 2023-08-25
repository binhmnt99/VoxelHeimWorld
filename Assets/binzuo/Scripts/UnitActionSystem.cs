using System;
using UnityEngine;

namespace binzuo
{
    public class UnitActionSystem : Singleton<UnitActionSystem>
    {
        [SerializeField] private LayerMask unitLayerMask;
        [SerializeField] private Unit selectedUnit;

        public event EventHandler OnSelectedUnitChanged;

        private void Update() {
            if (Input.GetMouseButtonDown(0))
            {
                if (TryHandleUnitSelection()) return;

                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MousePosition.GetPoint());
                if (selectedUnit.GetMoveAction().IsValidAvtionGridPosition(mouseGridPosition))
                {
                    selectedUnit.GetMoveAction().Move(mouseGridPosition);
                }
            }

        }

        private bool TryHandleUnitSelection()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out Unit unit))
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

        public Unit GetSelectedUnit() => selectedUnit;
        
    }
}
