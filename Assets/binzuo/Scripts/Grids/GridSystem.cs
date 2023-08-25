using UnityEngine;

namespace binzuo
{
    public class GridSystem
    {
        private int width;
        private int height;
        private float cellSize;
        private GridObject[,] gridObjectArray;

        public GridSystem(int _width, int _height, float _cellSize)
        {
            this.width = _width;
            this.height = _height;
            this.cellSize = _cellSize;

            gridObjectArray = new GridObject[width,height];

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x,z);
                    gridObjectArray[x,z] = new GridObject(this, gridPosition);
                }
            }
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;

        public GridPosition GetGridPosition(Vector3 worldPosition) => new GridPosition(
                Mathf.RoundToInt(worldPosition.x/ cellSize),
                Mathf.RoundToInt(worldPosition.z/cellSize)
            );
        

        public void CreateDebugObjects(Transform debugPrefab)
        {
            GameObject debugObjects = new("DebugObject");
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GridPosition gridPosition = new GridPosition(x,z);
                    Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, debugObjects.transform);
                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(GetGridObject(gridPosition));
                    debugTransform.gameObject.isStatic = true;
                }
            }
            debugObjects.isStatic = true;
        }

        public GridObject GetGridObject(GridPosition gridPosition) => gridObjectArray[gridPosition.x, gridPosition.z];
        

        public bool IsValidGridPosition(GridPosition gridPosition) => gridPosition.x >= 0 &&  gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;

    }
}
