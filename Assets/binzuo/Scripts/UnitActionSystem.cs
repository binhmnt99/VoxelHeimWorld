using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace binzuo
{
    public class UnitActionSystem : Singleton<UnitActionSystem>
    {

        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnActingChanged;

        public event EventHandler OnActionStarted;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private BaseAction selectedAction;
        [SerializeField] private LayerMask unitLayerMask;

        private bool acting;

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
        }

        private void Update()
        {
            if (acting) return;

            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (TryHandleUnitSelection()) return;

            HandleSelectedAction();
        }

        private void SetActing()
        {
            acting = true;
            OnActingChanged?.Invoke(this, acting);
        }

        private void ClearActing()
        {
            acting = false;
            OnActingChanged?.Invoke(this, acting);
        }

        private bool TryHandleUnitSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                    {
                        if (unit.IsEnemy())
                        {
                            return false;
                        }
                        if (unit == selectedUnit)
                        {
                            return false;
                        }
                        SetSelectedUnit(unit);
                        return true;
                    }
                }
            }
            return false;
        }

        private void HandleSelectedAction()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MousePosition.GetPoint());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;


                if (!selectedUnit.TrySpendActionPointToTakeAction(selectedAction)) return;

                SetActing();
                selectedAction.TakeAction(mouseGridPosition, ClearActing);
                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;
            SetSelectedAction(unit.GetMoveAction());

            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetSelectedAction(BaseAction baseAction)
        {
            selectedAction = baseAction;
            OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
        }

        public Unit GetSelectedUnit()
        {
            return selectedUnit;
        }

        public BaseAction GetSelectedAction()
        {
            return selectedAction;
        }

    }
}
