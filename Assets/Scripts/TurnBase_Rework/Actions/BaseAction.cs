using System;
using System.Collections.Generic;
using UnityEngine;

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

    public abstract void TakeAction(Path path, Tile targetTile, Action onActionComplete);

    public virtual bool IsValidActionGridPosition(Tile tile)
    {
        List<Tile> validGridPositionList = GetValidActionTilePositionList();
        return validGridPositionList.Contains(tile);
    }

    public abstract List<Tile> GetValidActionTilePositionList();

    public abstract void ShowValidTile();
    public abstract void HideValidTile();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        HideValidTile();
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
        ShowValidTile();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }
}
