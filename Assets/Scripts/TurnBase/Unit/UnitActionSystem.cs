using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace TurnBase
{
    public class UnitActionSystem : MonoBehaviour
    {
        public static UnitActionSystem Instance { get; private set; }

        private Ray ray;
        private RaycastHit raycastHit;
        private UnitInput unitInput;
        public event EventHandler OnSelectedUnitChanged;
        public event EventHandler OnSelectedActionChanged;
        public event EventHandler<bool> OnBusyChanged;
        public event EventHandler OnActionStarted;

        [SerializeField] private Unit selectedUnit;
        [SerializeField] private LayerMask layerMask;
        private bool isBusy;
        private BaseAction selectedAction;
        // Start is called before the first frame update
        void Start()
        {
            SetSelectedUnit(selectedUnit);
        }
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

        private void HandleSelectedAction()
        {
            if (unitInput.isMoveClicked)
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
        // Update is called once per frame
        void Update()
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

        private bool TryHandleUnitSelection()
        {
            if (unitInput.isMoveClicked)
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask))
                {
                    if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                    {
                        if (unit == selectedUnit)
                        {
                            return false;
                        }
                        if (unit.IsEnemy())
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

