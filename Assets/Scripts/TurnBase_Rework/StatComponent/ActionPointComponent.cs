using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPointComponent : MonoBehaviour
{
    private int maxActionPoint;
    [SerializeField] private int actionPoint;

    public void Setup(int value)
    {
        actionPoint = value;
        maxActionPoint = actionPoint;
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

    public void SpendActionPoints(int amount, EventHandler OnAnyActionPointsChanged)
    {
        actionPoint -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoint;
    }
}
