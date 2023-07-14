using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region member fields
    public bool Moving { get; private set; } = false;

    public UnitData unitData;
    public Tile unitTile;
    [SerializeField]
    LayerMask GroundLayerMask;
    [SerializeField] private bool isEnemy;
    private HealthComponent healthComponent;
    private ActionPointComponent actionPointComponent;

    private BaseAction[] baseActionArray;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;
    #endregion

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
    /// <summary>
    /// If no starting tile has been manually assigned, we find one beneath us
    /// </summary>
    void FindTileAtStart()
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

    IEnumerator MoveAlongPath(Path path)
    {
        const float MIN_DISTANCE = 0.05f;
        const float TERRAIN_PENALTY = 0.5f;

        int currentStep = 0;
        int pathLength = path.tiles.Length - 1;
        Tile currentTile = path.tiles[0];
        float animationTime = 0f;

        while (currentStep <= pathLength)
        {
            yield return null;

            //Move towards the next step in the path until we are closer than MIN_DIST
            Vector3 nextTilePosition = path.tiles[currentStep].transform.position;
            Debug.Log(unitData.GetStat(0).statName);
            float movementTime = animationTime / (unitData.GetStat(2).value + path.tiles[currentStep].terrainCost * TERRAIN_PENALTY);
            MoveAndRotate(currentTile.transform.position, nextTilePosition, movementTime);
            animationTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextTilePosition) > MIN_DISTANCE)
                continue;

            //Min dist has been reached, look to next step in path
            currentTile = path.tiles[currentStep];
            currentStep++;
            animationTime = 0f;
        }

        FinalizePosition(path.tiles[pathLength]);
    }

    public void StartMove(Path _path)
    {
        Moving = true;
        unitTile.Occupied = false;
        StartCoroutine(MoveAlongPath(_path));
    }

    public void FinalizePosition(Tile tile)
    {
        transform.position = tile.transform.position;
        unitTile = tile;
        Moving = false;
        tile.Occupied = true;
        tile.occupyingCharacter = this;
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.rotation = Quaternion.LookRotation(origin.DirectionTo(destination).Flat(), Vector3.up);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (CanSpendActionPointsToTakeAction(baseAction))
            {
                actionPointComponent.SpendActionPoints(baseAction.GetActionPointsCost(),OnAnyActionPointsChanged);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (actionPointComponent.GetActionPoints() >= baseAction.GetActionPointsCost())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
}