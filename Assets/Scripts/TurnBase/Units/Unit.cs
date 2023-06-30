using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int actionPointMax;
        private int actionPoint;


        public static event EventHandler OnAnyActionPointsChanged;
        public static event EventHandler OnAnyUnitSpawned;
        public static event EventHandler OnAnyUnitDead;


        [SerializeField] private bool isEnemy;


        private GridPosition gridPosition;
        private HealthSystem healthSystem;
        private BaseAction[] baseActionArray;

        private void Awake()
        {
            actionPoint = actionPointMax;
            healthSystem = GetComponent<HealthSystem>();
            baseActionArray = GetComponents<BaseAction>();
        }

        private void Start()
        {
            gridPosition = HexLevelGrid.Instance.GetGridPosition(transform.position);
            HexLevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            healthSystem.OnDead += HealthSystem_OnDead;

            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            GridPosition newGridPosition = HexLevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                // Unit changed Grid Position
                GridPosition oldGridPosition = gridPosition;
                gridPosition = newGridPosition;

                HexLevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }

        public T GetAction<T>() where T : BaseAction
        {
            foreach (BaseAction baseAction in baseActionArray)
            {
                if (baseAction is T)
                {
                    return (T)baseAction;
                }
            }
            return null;
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public BaseAction[] GetBaseActionArray()
        {
            return baseActionArray;
        }

        public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (CanSpendActionPointsToTakeAction(baseAction))
            {
                SpendActionPoints(baseAction.GetActionPointsCost());
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (actionPoint >= baseAction.GetActionPointsCost())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SpendActionPoints(int amount)
        {
            actionPoint -= amount;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints()
        {
            return actionPoint;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
                (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
            {
                actionPoint = actionPointMax;

                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsEnemy()
        {
            return isEnemy;
        }

        public void Damage(int damageAmount)
        {
            healthSystem.Damage(damageAmount);
        }

        private void HealthSystem_OnDead(object sender, EventArgs e)
        {
            HexLevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

            Destroy(gameObject);

            OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        }

        public float GetHealthNormalized()
        {
            return healthSystem.GetHealthNormalized();
        }

    }
}
