using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int actionPointMax;
        [SerializeField] private int movePointMax;
        [SerializeField] private bool isEnemy;
        private GridPosition gridPosition;
        private BaseAction[] baseActionArray;
        private int actionPoint;
        private int movePoint;

        private HealthSystem healthSystem;
        private int positionCount;

        public static event EventHandler OnAnyActionPointsChanged;
        public static event EventHandler OnAnyMovePointsChanged;
        public static event EventHandler OnAnyUnitSpawned;
        public static event EventHandler OnAnyUnitDead;

        void Awake()
        {
            actionPoint = actionPointMax;
            movePoint = movePointMax;
            baseActionArray = GetComponents<BaseAction>();
            healthSystem = GetComponent<HealthSystem>();
        }

        void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            healthSystem.OnDead += HealthSystem_OnDead;

            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        }

        // Update is called once per frame
        void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                GridPosition oldGridPosition = gridPosition;
                gridPosition = newGridPosition;
                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);

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

        public bool TrySpendMovePointsToTakeAction(GridPosition mouseGridPosition)
        {
            if (CanSpendMovePointsToTakeAction(mouseGridPosition))
            {
                SpendMovePoints(positionCount);
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

        public bool CanSpendMovePointsToTakeAction(GridPosition mouseGridPosition)
        {
            List<GridPosition> pathGridPositionList = HexPathfinding.Instance.FindPath(GetGridPosition(), mouseGridPosition, out int pathLength);
            positionCount = pathGridPositionList.Count - 1;
            //Debug.Log(" point count " + positionCount + " move point " + movePoint);
            if (movePoint >= positionCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SpendActionPoints(int value)
        {
            actionPoint -= value;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints()
        {
            return actionPoint;
        }

        private void SpendMovePoints(int value)
        {
            movePoint -= value;
            //Debug.Log(movePoint);
            OnAnyMovePointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetMovePoints()
        {
            return movePoint;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if ((IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
            || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
            {
                actionPoint = actionPointMax;
                movePoint = movePointMax;

                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
                OnAnyMovePointsChanged?.Invoke(this, EventArgs.Empty);
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
            LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
            Destroy(gameObject);
            OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        }

        public float GetHealthNormalized()
        {
            return healthSystem.GetHealthNormalize();
        }
    }
}
