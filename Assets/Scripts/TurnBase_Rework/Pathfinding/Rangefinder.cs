using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rangefinder : Singleton<Rangefinder>
{
    [SerializeField] private LayerMask tileMask;

    private List<Tile> validTiles = new List<Tile>();
    private List<Tile> tilesForPreviousStep = new List<Tile>();
    private List<Tile> surroundingTiles = new List<Tile>();

    private const float HexagonalOffset = 1.75f;
    private const float RayLength = 50f;
    private const float RayHeightOffset = 1f;
    private const int RotationStep = 60;

    private List<Tile> GetNeighborTiles(Tile startTile)
    {
        List<Tile> tiles = new List<Tile>();
        Vector3 direction = Vector3.forward * (startTile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * HexagonalOffset);

        // Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, RotationStep, 0f) * direction;
            Vector3 aboveTilePos = (startTile.transform.position + direction).With(y: startTile.transform.position.y + RayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, RayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                tiles.Add(hitTile);
            }
        }

        if (startTile.connectedTile != null)
            tiles.Add(startTile.connectedTile);

        return tiles;
    }

    public List<Tile> GetTilesInRange(Tile tile, int range)
    {
        validTiles.Clear();
        tilesForPreviousStep.Clear();
        surroundingTiles.Clear();
        tilesForPreviousStep.Add(tile);

        for (int stepCount = 0; stepCount < range; stepCount++)
        {
            foreach (var item in tilesForPreviousStep)
            {
                surroundingTiles.AddRange(GetNeighborTiles(item));
            }

            validTiles.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
        }

        return validTiles.Distinct().ToList();
    }
}
