using System.Collections;
using System.Collections.Generic;
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

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * cellSize;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(
                Mathf.RoundToInt(worldPosition.x/ cellSize),
                Mathf.RoundToInt(worldPosition.z/cellSize)
            );
        }

        public void CreateDebugObjects(Transform debugPrefab)
        {
            GameObject debugObject = new("DebugObject");
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    GameObject.Instantiate(debugPrefab, GetWorldPosition(x,z), Quaternion.identity, debugObject.transform);
                }
            }
        }
    }
}
