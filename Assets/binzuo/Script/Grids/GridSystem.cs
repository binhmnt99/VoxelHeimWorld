using System;
using UnityEngine;

namespace binzuo
{
    public class GridSystem<TGridObject>
    {
        private int width;
        private int height;
        private float cellSize;
        private int floor;
        private float floorHeight;
        private TGridObject[,] gridObjectArray;

        public GridSystem(int width, int height, float cellSize, int floor,float floorHeight, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.floor = floor;
            this.floorHeight = floorHeight;

            gridObjectArray = new TGridObject[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);
                    gridObjectArray[x, z] = createGridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + new Vector3(0f, gridPosition.floor, 0f) * floorHeight;

        public GridPosition GetGridPosition(Vector3 worldPosition) => new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize), floor);

        public void CreateDebugObjects(Transform debugPrefab)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z, floor);

                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, LevelGrid.Instance.transform);
                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                }
            }
        }

        public TGridObject GetGridObject(GridPosition gridPosition) => gridObjectArray[gridPosition.x, gridPosition.z];

        public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height && gridPosition.floor == floor;

        public int GetWidth() => width;

        public int GetHeight() => height;

    }
}
