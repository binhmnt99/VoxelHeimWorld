using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShoot : BaseAction
{
    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<Tile> GetValidActionTilePositionList()
    {
        return validTiles;
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
        throw new NotImplementedException();
    }
}
