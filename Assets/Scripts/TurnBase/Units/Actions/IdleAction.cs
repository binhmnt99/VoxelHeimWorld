using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class IdleAction : BaseAction
    {
        [SerializeField] private bool onAction = false;

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);
            onAction = true;
        }
        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            GridPosition unitGridPosition = unit.GetGridPosition();

            return new List<GridPosition>
            { unitGridPosition};
        }
        void Update()
        {
            if (!isActive)
            {
                return;
            }
            if (onAction)
            {
                ActionComplete();
            }
        }

        public override string GetActionName()
        {
            return "Idle";
        }

        public override int GetActionPointsCost()
        {
            return 2;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            //Debug.Log("Idle grid position " + gridPosition + "\n Idle value: " + 0);
            return new EnemyAIAction{ gridPosition = gridPosition, actionValue = 0};
        }
    }
}

