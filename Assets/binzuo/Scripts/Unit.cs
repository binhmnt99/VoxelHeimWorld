using System;
using UnityEngine;

namespace binzuo
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private Sprite unitAvatar;
        [SerializeField] private bool isEnemy;
        private GridPosition gridPosition;
        private MoveAction moveAction;
        private BaseAction[] baseActionArray;

        private ActionPoint actionPoint;
        public static event EventHandler OnAnyActionPointChanged;

        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            baseActionArray = GetComponents<BaseAction>();

            actionPoint = GetComponent<ActionPoint>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
                !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
            {
                actionPoint.ResetCurrentValue();
                OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }

        public Sprite GetAvatar()
        {
            return unitAvatar;
        }

        public bool IsEnemy() => isEnemy;

        public MoveAction GetMoveAction() => moveAction;

        public GridPosition GetGridPosition() => gridPosition;

        public BaseAction[] GetBaseActions() => baseActionArray;

        public bool TrySpendActionPointToTakeAction(BaseAction baseAction)
        {
            if (CanSpendActionPointToTakeAction(baseAction))
            {
                SpendActionPoint(baseAction.GetActionPointCost());
                return true;
            }
            return false;
        }

        public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
        {
            if (actionPoint.GetCurrentValue() >= baseAction.GetActionPointCost())
            {
                return true;
            }
            return false;
        }

        private void SpendActionPoint(int amount)
        {
            actionPoint.CurrentValue(amount);
            OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints()
        {
            return actionPoint.GetCurrentValue();
        }
    }
}

