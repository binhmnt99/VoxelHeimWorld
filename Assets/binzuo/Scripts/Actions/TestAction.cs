using System;
using System.Collections.Generic;


namespace binzuo
{
    public class TestAction : BaseAction
    {
        public override string GetActionName()
        {
            return "Testing";
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            return null;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            throw new NotImplementedException();
        }
    }

}