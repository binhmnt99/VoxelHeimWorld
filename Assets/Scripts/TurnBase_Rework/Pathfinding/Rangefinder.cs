using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Rangefinder : Singleton<Rangefinder>
{
    [SerializeField] private LayerMask tileMask;
    private List<Tile> validTile = new();

    private List<Tile> GetTilesNeighbor(Tile startTile)
    {
        List<Tile> tiles = new List<Tile>();
        const float HEXAGONAL_OFFSET = 1.75f;

        Vector3 direction = Vector3.forward * (startTile.GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * HEXAGONAL_OFFSET);
        float rayLength = 50f;
        float rayHeightOffset = 1f;

        //Rotate a raycast in 60 degree steps and find all adjacent tiles
        for (int i = 0; i < 6; i++)
        {
            direction = Quaternion.Euler(0f, 60f, 0f) * direction;

            Vector3 aboveTilePos = (startTile.transform.position + direction).With(y: startTile.transform.position.y + rayHeightOffset);

            if (Physics.Raycast(aboveTilePos, Vector3.down, out RaycastHit hit, rayLength, tileMask))
            {
                Tile hitTile = hit.transform.GetComponent<Tile>();
                // if (hitTile.Occupied == false)
                // {
                    
                // }
                tiles.Add(hitTile);
            }
        }

        if (startTile.connectedTile != null)
            tiles.Add(startTile.connectedTile);

        //Debug.Log(tiles.Count);
        return tiles;
    }

    List<Tile> tilesForPreviousStep = new();
    List<Tile> surroundingTiles = new();
    public List<Tile> GetTilesInRange(Tile tile, int range)
    {
        validTile.Clear();
        tilesForPreviousStep.Clear();
        surroundingTiles.Clear();
        int stepCount = 0;
        tilesForPreviousStep.Add(tile);
        while (stepCount < range)
        {
            foreach (var item in tilesForPreviousStep)
            {
                surroundingTiles.AddRange(GetTilesNeighbor(item));
            }
            validTile.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }
        return validTile.Distinct().ToList();
    }
}
