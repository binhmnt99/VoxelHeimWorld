using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int actionPointMax = 5;
        private GridPosition gridPosition;
        private MoveAction moveAction;
        private SpinAction spinAction;
        private BaseAction[] baseActionArray;
        private int actionPoint;

        public static event EventHandler OnAnyActionPointsChanged;

        void Awake()
        {
            moveAction = GetComponent<MoveAction>();
            spinAction = GetComponent<SpinAction>();
            baseActionArray = GetComponents<BaseAction>();
        }

        void Start()
        {
            actionPoint = actionPointMax;
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        // Update is called once per frame
        void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
                gridPosition = newGridPosition;
            }
        }

        public MoveAction GetMoveAction()
        {
            return moveAction;
        }

        public SpinAction GetSpinAction()
        {
            return spinAction;
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
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

        private void SpendActionPoints(int value)
        {
            actionPoint -= value;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints()
        {
            return actionPoint;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            actionPoint = actionPointMax;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
