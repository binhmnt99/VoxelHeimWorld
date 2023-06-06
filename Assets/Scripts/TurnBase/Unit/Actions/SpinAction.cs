using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class SpinAction : BaseAction
    {
        [SerializeField] private float totalSpinAmount;

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            this.onActionComplete = onActionComplete;
            isActive = true;
            totalSpinAmount = 0;
        }
        public override List<GridPosition> GetValidActionGridPositionsList()
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
            float spinAmount = 360f * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, spinAmount, 0);
            totalSpinAmount += spinAmount;
            if (totalSpinAmount >= 360f)
            {
                isActive = false;
                onActionComplete();
            }
        }

        public override string GetActionName()
        {
            return "Spin";
        }

        public override int GetActionPointsCost()
        {
            return 2;
        }
    }
}
