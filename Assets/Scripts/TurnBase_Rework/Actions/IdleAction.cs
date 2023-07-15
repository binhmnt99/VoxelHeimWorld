using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : BaseAction
{
    public override string GetActionName()
    {
        return "Idle";
    }

    public override List<Tile> GetValidActionTilePositionList()
    {
        throw new NotImplementedException();
    }

    public override void HideValidTile()
    {
        throw new NotImplementedException();
    }

    public override void ShowValidTile()
    {
        throw new NotImplementedException();
    }

    public override void TakeAction(Path path, Tile targetTile, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        ActionComplete();
    }

    public override int GetActionPointsCost()
    {
        return (int)unit.unitData.GetStat(0).value;
    }
}
