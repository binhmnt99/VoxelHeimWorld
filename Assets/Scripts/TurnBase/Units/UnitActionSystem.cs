using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace TurnBase
{
    public class UnitActionSystem : Singleton<UnitActionSystem>
    {
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;


        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask unitLayerMask;

        private BaseAction selectedAction;
        private bool isBusy;

        private void Start()
        {
            SetSelectedUnit(selectedUnit);
        }

        private void Update()
        {
            if (isBusy)
            {
                return;
            }

            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (TryHandleUnitSelection())
            {
                return;
            }

            HandleSelectedAction();
        }

        private void HandleSelectedAction()
        {
            if (InputManager.Instance.GetLeftMouseButtonDown())
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MousePosition.Instance.GetPosition());

                if (!selectedAction.IsValidActionGridPosition(GridSystemVisual.Instance.GetGridPositionSelected()))
                {
                    return;
                }

                if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    return;
                }

                SetBusy();
                selectedAction.TakeAction(GridSystemVisual.Instance.GetGridPositionSelected(), ClearBusy);
                Debug.Log("HandleAction");
                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }


        private void SetBusy()
        {
            isBusy = true;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private void ClearBusy()
        {
            isBusy = false;

            OnBusyChanged?.Invoke(this, isBusy);
        }

        private bool TryHandleUnitSelection()
        {
            if (InputManager.Instance.GetLeftMouseButtonDown())
            {
                Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
                if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                    {
                        if (unit == selectedUnit)
                        {
                            // Unit is already selected
                            return false;
                        }

                        if (unit.IsEnemy())
                        {
                            // Clicked on an Enemy
                            return false;
                        }

                        SetSelectedUnit(unit);
                        return true;
                    }
                }
            }

            return false;
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            SetSelectedAction(unit.GetAction<MoveAction>());

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

