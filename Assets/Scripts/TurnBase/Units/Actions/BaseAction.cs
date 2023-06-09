using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public abstract class BaseAction : MonoBehaviour
    {
        public static event EventHandler OnAnyActionStarted;
        public static event EventHandler OnAnyActionCompleted;


        protected Unit unit;
        protected bool isActive;
        protected Action onActionComplete;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        public abstract string GetActionName();

        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

        public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
        {
            Debug.Log("IsValidActionGridPosition");
            List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
            return validGridPositionList.Contains(gridPosition);
        }

        public abstract List<GridPosition> GetValidActionGridPositionList();

        public virtual int GetActionPointsCost()
        {
            return 1;
        }

        protected void ActionStart(Action onActionComplete)
        {
            isActive = true;
            this.onActionComplete = onActionComplete;
            Debug.Log("Action Start");
            GridSystemVisual.Instance.HideAllGridPosition();
            OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
        }

        protected void ActionComplete()
        {
            isActive = false;
            onActionComplete();
            Debug.Log("Action End");
            OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        }

        public Unit GetUnit()
        {
            return unit;
        }

        public EnemyAIAction GetBestEnemyAIAction()
        {
            List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

            List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

            foreach (GridPosition gridPosition in validActionGridPositionList)
            {
                EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
                enemyAIActionList.Add(enemyAIAction);
            }
            if (enemyAIActionList.Count > 0)
            {
                enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
                return enemyAIActionList[0];
            }
            else
            {
                //Không có hành động AI của kẻ thù có thể xảy ra.
                return null;
            }

        }

        public abstract EnemyAIAction GetEnemyAIAction(GridPosition validActionGridPosition);

    }

}