using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class Tile : MonoBehaviour
{
    #region member fields
    public Tile parent;
    public Tile connectedTile;
    public Unit occupyingCharacter;

    public int cost;

    public float costFromOrigin = 0;
    public float costToDestination = 0;
    public int terrainCost = 0;
    public float TotalCost { get { return costFromOrigin + costToDestination + terrainCost; } }
    public bool Occupied { get; set; } = false;
    [SerializeField]
    TMP_Text costText;
    [Serializable]
    public struct TileVisualTypeMaterial
    {
        public TileVisualType tileVisualType;
        public Material material;
    }

    public enum TileVisualType
    {
        Default,
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
        Green
    }

    public TileVisualType selectTileVisualType;

    [SerializeField] private List<TileVisualTypeMaterial> tileVisualTypeMaterialList;
    #endregion

    /// <summary>
    /// Changes color of the tile by activating child-objects of different colors
    /// </summary>
    /// <param name="col"></param>
    public void Highlight()
    {
        SetMaterial(TileVisualType.White);
    }

    public void ClearHighlight()
    {
        SetMaterial(TileVisualType.Default);
    }

    /// <summary>
    /// This is called when right clicking a tile to increase its cost
    /// </summary>
    /// <param name="value"></param>
    // public void ModifyCost()
    // {
    //     terrainCost++;
    //     if (terrainCost > costMap.Count - 1)
    //         terrainCost = 0;

    //     if (costMap.TryGetValue(terrainCost, out Color col))
    //     {
    //         SetMaterial(col);
    //     }
    //     else
    //     {
    //         Debug.Log("Invalid terraincost or mapping" + terrainCost);
    //     }
    // }

    public void SetMaterial(TileVisualType tileVisualType)
    {
        GetComponent<MeshRenderer>().sharedMaterial = GetTileVisualTypeMaterial(tileVisualType);
        this.selectTileVisualType = tileVisualType;
    }

    public void DebugCostText()
    {
        costText.text = TotalCost.ToString("F1");
    }

    public void ClearText()
    {
        costText.text = "";
    }

    private Material GetTileVisualTypeMaterial(TileVisualType tileVisualType)
    {
        foreach (TileVisualTypeMaterial gridVisualTypeMaterial in tileVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.tileVisualType == tileVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + tileVisualType);
        return null;
    }
}
