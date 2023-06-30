using System.Globalization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class HexGridSystem<TGridObject>
    {
        private const float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
        private int width;
        private int height;

        private float cellSize;
        private int x;
        private int z;

        private TGridObject[,] gridObjectArray;

        public HexGridSystem(int width, int height, float cellSize, Func<HexGridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            gridObjectArray = new TGridObject[width, height];
            GridPosition gridPosition;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridPosition = new GridPosition(x,z);
                    gridObjectArray[gridPosition.x, gridPosition.z] = createGridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return
                new Vector3(gridPosition.x, 0, 0) * cellSize +
                new Vector3(0, 0, gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
                (((gridPosition.z % 2) == 1) ? new Vector3(1, 0, 0) * cellSize * .5f : Vector3.zero);

        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            GridPosition roughXZ = new GridPosition(
                        Mathf.RoundToInt(worldPosition.x / cellSize),
                        Mathf.RoundToInt((worldPosition.z / cellSize) / HEX_VERTICAL_OFFSET_MULTIPLIER)
                    );

            bool oddRow = roughXZ.z % 2 == 1;

            List<GridPosition> neighborGridPositionList = new List<GridPosition>
        {
            roughXZ + new GridPosition(-1, 0),
            roughXZ + new GridPosition(+1, 0),

            roughXZ + new GridPosition(0, +1),
            roughXZ + new GridPosition(0, -1),

            roughXZ + new GridPosition(oddRow ? +1 : -1, +1),
            roughXZ + new GridPosition(oddRow ? +1 : -1, -1),
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

            return closestGridPosition;

        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            GameObject deBugGridSystem = new GameObject("DebugGridSystem");
            GridPosition gridPosition;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridPosition = new GridPosition(x, z);
                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, deBugGridSystem.transform);

                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public TGridObject GetGridObject(GridPosition gridPosition)
        {
            return gridObjectArray[gridPosition.x, gridPosition.z];
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.x >= 0 &&
                    gridPosition.z >= 0 &&
                    gridPosition.x < width &&
                    gridPosition.z < height;
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
