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

        private BaseStats[] baseStatsArray;
        public static event EventHandler OnAnyActionPointChanged;

        private void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            baseStatsArray = GetComponents<BaseStats>();
            baseActionArray = GetComponents<BaseAction>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
            GetStats<HitPoint>().OnDead += HitPoint_OnDead;
            GetStats<ExperiencePoint>().OnLevelUp += ExperiencePoint_OnLevelUp;
        }

        private void ExperiencePoint_OnLevelUp(object sender, EventArgs e)
        {
            GetStats<LevelPoint>().LevelUp();
        }

        private void HitPoint_OnDead(object sender, EventArgs e)
        {
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
            Destroy(gameObject);
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
                !IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
            {
                GetStats<ActionPoint>().ResetValue();
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

        public Vector3 GetWorldPosition() => transform.position;

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
            if (GetStats<ActionPoint>().GetValue() >= baseAction.GetActionPointCost())
            {
                return true;
            }
            return false;
        }

        private void SpendActionPoint(int amount)
        {
            GetStats<ActionPoint>().CalculateValue(amount);
            OnAnyActionPointChanged?.Invoke(this, EventArgs.Empty);
        }

        public void TakeDamage(float amount)
        {
            GetStats<HitPoint>().CalculateValue(amount);
        }

        public T GetStats<T>() where T : BaseStats
        {
            foreach (BaseStats baseStat in baseStatsArray)
            {
                if (baseStat is T)
                {
                    return (T)baseStat;
                }
            }
            return null;
        }
    }
}

