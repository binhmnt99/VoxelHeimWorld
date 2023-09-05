using UnityEngine;

namespace binzuo
{
    public class Unit : MonoBehaviour
    {
        private GridPosition gridPosition;
        private MoveAction moveAction;
        private BaseAction[] baseActionArray;

        private ActionPoint actionPoint;

        private void Awake() {
            moveAction = GetComponent<MoveAction>();
            baseActionArray = GetComponents<BaseAction>();

            actionPoint = GetComponent<ActionPoint>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
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
        }

        public int GetActionPoints()
        {
            return actionPoint.GetCurrentValue();
        }
    }
}

