using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int moveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;


    private List<Tile> allRangeTile;
    private List<Tile> validTile;

    protected override void Awake()
    {
        base.Awake();
        moveDistance = (int)unit.unitData.GetAbility(0).GetStat(0).value;
        moveSpeed = unit.unitData.GetAbility(0).GetStat(1).value;
        rotateSpeed = unit.unitData.GetAbility(0).GetStat(2).value;
        allRangeTile = new();
        validTile = new();
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override void TakeAction(Path path, Tile targetTile, Action onActionComplete)
    {
        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
        unit.unitTile.Occupied = false;
        StartCoroutine(MoveAlongPath(path));
    }

    IEnumerator MoveAlongPath(Path path)
    {
        const float MIN_DISTANCE = 0.05f;

        int currentStep = 0;
        Tile currentTile = path.tiles[0];
        float animationTime = 0f;

        while (currentStep < path.tiles.Length)
        {
            yield return null;

            //Move towards the next step in the path until we are closer than MIN_DIST
            Vector3 nextPosition = path.tiles[currentStep].transform.position;
            Vector3 moveDirection = (nextPosition - transform.position).normalized;

            float movementTime = animationTime * moveSpeed;
            MoveAndRotate(currentTile.transform.position, nextPosition, moveDirection, movementTime);
            animationTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, nextPosition) > MIN_DISTANCE)
                continue;

            //Min dist has been reached, look to next step in path
            currentTile = path.tiles[currentStep];
            currentStep++;
            animationTime = 0f;
        }
        if (currentStep >= path.tiles.Length)
        {
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            transform.position = path.tiles[path.tiles.Length - 1].transform.position;
            unit.FinalizePosition(path.tiles[path.tiles.Length - 1]);
            Pathfinder.Instance.ResetPathfinder();
            ActionComplete();
        }
    }

    void MoveAndRotate(Vector3 origin, Vector3 destination, Vector3 moveDirection, float duration)
    {
        transform.position = Vector3.Lerp(origin, destination, duration);
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    public override List<Tile> GetValidActionTilePositionList()
    {
        return validTile;
    }

    public override void ShowValidTile()
    {
        validTile.Clear();
        allRangeTile = Rangefinder.Instance.GetTilesInRange(unit.unitTile, moveDistance);

        foreach (Tile tile in allRangeTile)
        {
            if (!tile.Occupied)
            {
                tile.SetMaterial(Tile.TileVisualType.Green);
                validTile.Add(tile);
            }
        }
        allRangeTile.Clear();
    }

    public override void HideValidTile()
    {
        if (validTile.Count != 0)
        {
            foreach (Tile tile in validTile)
            {
                tile.SetMaterial(Tile.TileVisualType.Default);
            }
        }
        validTile.Clear();
    }
}
