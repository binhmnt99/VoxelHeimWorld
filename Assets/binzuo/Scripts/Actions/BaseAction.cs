using System.Globalization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public abstract class BaseAction : MonoBehaviour
    {
        protected Unit unit;
        protected bool isActive;

        protected Action onActionComplete;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
        }

        public abstract string GetActionName();

        public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

        public virtual bool IsValidActionGridPosition(GridPosition gridPosition) => GetValidActionGridPositionList().Contains(gridPosition);

        public abstract List<GridPosition> GetValidActionGridPositionList();

        public virtual int GetActionPointCost()
        {
            return 1;
        }

        public void ActionStart(Action onActionComplete)
        {
            isActive = true;
            this.onActionComplete = onActionComplete;
        }

        public void ActionComplete()
        {
            isActive = false;
            onActionComplete();
        }
    }
}

