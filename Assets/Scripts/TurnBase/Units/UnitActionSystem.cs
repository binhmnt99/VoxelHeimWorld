using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace TurnBase
{
    public class UnitActionSystem : MonoBehaviour
    {
        public enum UnitActionSystemState
        {
            UnselectedUnit,
            SelectedUnit,
            SelectedUnitAction,
        }
        public static UnitActionSystem Instance { get; private set; }
        private Ray ray;
        private RaycastHit raycastHit;
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private UnitActionSystemState currentState;
        private bool isBusy;
        private BaseAction selectedAction;

        public UnitActionSystemState GetState()
        {
            return currentState;
        }
        public void SetState(UnitActionSystemState _state)
        {
            currentState = _state;
        }
        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            //SetSelectedUnit(selectedUnit);
            currentState = UnitActionSystemState.UnselectedUnit;
        }
        // Update is called once per frame
        void Update()
        {
            //Debug.Log(LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition()));
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
                HandleUnitActionSystemState();
                return;
            }
            if (TryHandleSelectedAction())
            {
                HandleUnitActionSystemState();
                return;
            }
        }

        private void HandleUnitActionSystemState()
        {
            if (currentState == UnitActionSystemState.UnselectedUnit)
            {
                SetSelectedUnit(null);
            }

            if (currentState == UnitActionSystemState.SelectedUnit)
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (selectedUnit == unit)
                    {
                        // Unit is already selected
                        SetSelectedAction(selectedUnit.GetAction<MoveAction>());
                        return;
                    }
                    if (unit.IsEnemy())
                    {
                        // Clicked on an Enemy
                        return;
                    }

                    SetSelectedUnit(unit);
                }
            }

            if (currentState == UnitActionSystemState.SelectedUnitAction)
            {
                GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

                if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
                {
                    return;
                }

                if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
                {
                    return;
                }

                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                OnActionStarted?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool TryHandleSelectedAction()
        {
            if (currentState == UnitActionSystemState.SelectedUnit)
            {
                ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
                if (!Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
                {
                    if (InputManager.Instance.GetLeftMouseButtonDown())
                    {
                        currentState = UnitActionSystemState.SelectedUnitAction;
                        return true;
                    }
                }
            }
            if (currentState == UnitActionSystemState.SelectedUnitAction)
            {
                if (InputManager.Instance.GetLeftMouseButtonDown())
                {
                    currentState = UnitActionSystemState.SelectedUnitAction;
                    return true;
                }
                if (InputManager.Instance.GetRightMouseButtonDown())
                {
                    ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
                    if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
                    {
                        if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                        {
                            if (selectedUnit == unit)
                            {
                                currentState = UnitActionSystemState.SelectedUnit;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        currentState = UnitActionSystemState.UnselectedUnit;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool TryHandleUnitSelection()
        {
            if (currentState == UnitActionSystemState.UnselectedUnit)
            {
                if (InputManager.Instance.GetLeftMouseButtonDown())
                {
                    ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
                    if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
                    {
                        currentState = UnitActionSystemState.SelectedUnit;
                        return true;
                    }
                }
            }
            if (currentState == UnitActionSystemState.SelectedUnit)
            {
                if (InputManager.Instance.GetLeftMouseButtonDown())
                {
                    ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
                    if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
                    {
                        currentState = UnitActionSystemState.SelectedUnit;
                        return true;
                    }
                }
                if (InputManager.Instance.GetRightMouseButtonDown())
                {
                    currentState = UnitActionSystemState.UnselectedUnit;
                    return true;
                }
            }
            return false;
        }

        private void SetBusy()
        {
            isBusy = true;
            OnBusyChanged?.Invoke(this, isBusy);
        }

        private void ClearBusy()
        {
            isBusy = false;
            if (selectedAction == selectedUnit.GetAction<MoveAction>())
            {
                currentState = UnitActionSystemState.SelectedUnit;
            }
            OnBusyChanged?.Invoke(this, isBusy);
        }

        private void SetSelectedUnit(Unit unit)
        {
            selectedUnit = unit;

            if (selectedUnit)
            {
                SetSelectedAction(unit.GetAction<MoveAction>());
            }
            else
            {
                SetSelectedAction(null);
            }

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

