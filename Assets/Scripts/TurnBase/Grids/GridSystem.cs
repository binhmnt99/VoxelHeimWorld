using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GridSystem<TGridObject>
    {
        private int width;
        private int height;
        private float cellSize;

        private TGridObject[,] gridObjects;
        private GridPosition gridPosition;
        private GameObject deBugGridSystem;

        public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;

            gridObjects = new TGridObject[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    gridPosition = new GridPosition(x, z);
                    gridObjects[x, z] = createGridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            deBugGridSystem = new GameObject("DebugGridSystem");
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);
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

