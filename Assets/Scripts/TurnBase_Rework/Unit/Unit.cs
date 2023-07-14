using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitData unitData;
    public Tile unitTile;
    [SerializeField] private LayerMask GroundLayerMask;
    [SerializeField] private bool isEnemy;

    private HealthComponent healthComponent;
    private ActionPointComponent actionPointComponent;
    private BaseAction[] baseActionArray;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
        actionPointComponent = GetComponent<ActionPointComponent>();
        baseActionArray = GetComponents<BaseAction>();
        FindTileAtStart();
    }

    private void Start()
    {
        healthComponent.Setup(unitData.GetStat(0).value);
        actionPointComponent.Setup((int)unitData.GetStat(1).value);
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public T GetAction<T>() where T : BaseAction
    {
        foreach (BaseAction baseAction in baseActionArray)
        {
            if (baseAction is T)
            {
                return (T)baseAction;
            }
        }
        return null;
    }

    private void FindTileAtStart()
    {
        if (unitTile != null)
        {
            FinalizePosition(unitTile);
            return;
        }

        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 50f, GroundLayerMask))
        {
            FinalizePosition(hit.transform.GetComponent<Tile>());
            return;
        }

        Debug.Log("Unable to find a start position");
    }

    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        unitTile = tile;
        tile.Occupied = true;
        tile.occupyingCharacter = this;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeAction(baseAction))
        {
            actionPointComponent.SpendActionPoints(baseAction.GetActionPointsCost(), OnAnyActionPointsChanged);
            return true;
        }

        return false;
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        return actionPointComponent.GetActionPoints() >= baseAction.GetActionPointsCost();
    }
}
