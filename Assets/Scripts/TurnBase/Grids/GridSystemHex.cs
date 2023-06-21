using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GridSystemHex<TGridObject>
    {
        private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
        private int width;
        private int height;
        private float cellSize;
        private int floor;
        private float floorHeight;

        private TGridObject[,] gridObjects;
        private GridPosition gridPosition;
        private GameObject deBugGridSystem;

        public GridSystemHex(int width, int height, float cellSize, int floor, float floorHeight, Func<GridSystemHex<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.floor = floor;
            this.floorHeight = floorHeight;

            gridObjects = new TGridObject[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridPosition = new GridPosition(x, z, floor);
                    gridObjects[x, z] = createGridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x, 0, 0) * cellSize +
            new Vector3(0, 0, gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
            new Vector3(0, gridPosition.floor, 0) * floorHeight +
            (((gridPosition.z % 2) == 1) ? new Vector3(1, 0, 0) * cellSize * .5f : Vector3.zero);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            //Debug.Log("1) " + worldPosition);
            GridPosition roughXZ = new GridPosition(
                Mathf.RoundToInt(worldPosition.x / cellSize),
                Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER),
                floor
            );
            //Debug.Log("2) " + Mathf.RoundToInt(worldPosition.x / cellSize) + " " + Mathf.RoundToInt(worldPosition.z / cellSize / HEX_VERTICAL_OFFSET_MULTIPLIER));
            bool oddRow = roughXZ.z % 2 == 1;

            List<GridPosition> neighborGridPositionList = new List<GridPosition>
            {
                roughXZ + new GridPosition(-1, 0, 0),
                roughXZ + new GridPosition(+1, 0, 0),

                roughXZ + new GridPosition(0, +1, 0),
                roughXZ + new GridPosition(0, -1, 0),

                roughXZ + new GridPosition(oddRow ? +1 : -1, +1, 0),
                roughXZ + new GridPosition(oddRow ? +1 : -1, -1, 0),
            };
            GridPosition closestGridPosition = roughXZ;

            foreach (GridPosition neighborGridPosition in neighborGridPositionList)
            {
                if (Vector3.Distance(worldPosition, GetWorldPosition(neighborGridPosition)) <
                    Vector3.Distance(worldPosition, GetWorldPosition(closestGridPosition)))
                {
                    // Closer than the Closest
                    closestGridPosition = neighborGridPosition;
                }
            }
            //Debug.Log("3) " + closestGridPosition.x + " " + closestGridPosition.z);
            return closestGridPosition;

        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            deBugGridSystem = new GameObject("DebugGridSystem");
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, deBugGridSystem.transform);

                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public TGridObject GetGridObject(GridPosition gridPosition)
        {
            return gridObjects[gridPosition.x, gridPosition.z];
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.x >= 0 &&
                    gridPosition.z >= 0 &&
                    gridPosition.x < width &&
                    gridPosition.z < height &&
                    gridPosition.floor == floor;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }
    }
}

