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

        public GridSystem(int _width, int _height, float _cellSize)
        {
            this.width = _width;
            this.height = _height;
            this.cellSize = _cellSize;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Debug.DrawLine(GetWorldPosition(x,z), GetWorldPosition(x,z) + Vector3.right * .2f, Color.white, 100f);

                }
            }
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * cellSize;
        }

        // public GridPosition GetGridPosition(Vector3 worldPosition)
        // {
        //     return new GridPosition(
        //         Mathf.RoundToInt(worldPosition.x / cellSize,
        //         Mathf.RoundToInt(worldPosition.z / cellSize))
        //     );
        // }

    }
}
