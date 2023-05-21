using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    [CreateAssetMenu(fileName = "VoxelDataSO", menuName = "Voxel/VoxelDataSO", order = 0)]
    public class VoxelDataSO : ScriptableObject
    {
        private int ceilSqrt = 0;
        private int numberOfColumn = 0;
        private int numberOfRow = 0;

        public float textureSizeX = 0;
        public float textureSizeY = 0;

        public List<TexturesData> texturesData;

        public void CalculateColumnAndRow()
        {
            if (texturesData.Count > 0)
            {
                ceilSqrt = (int)Mathf.Ceil(Mathf.Sqrt(texturesData.Count));
                numberOfColumn = ceilSqrt;
                numberOfRow = ceilSqrt;
                while (numberOfColumn * numberOfRow < texturesData.Count || numberOfColumn * numberOfRow % ceilSqrt != 0)
                {
                    numberOfColumn++;
                    if (numberOfColumn * numberOfRow < texturesData.Count || numberOfColumn * numberOfRow % ceilSqrt != 0)
                    {
                        numberOfRow++;
                    }
                }

                textureSizeX = 1f / (float)numberOfColumn;
                textureSizeY = 1f / (float)numberOfRow;
            }
        }
    }

    [Serializable]
    public class TexturesData
    {
        public VoxelType voxelType;
        public bool isSolid = true;
        public bool isCollider = true;
        public Vector2Int side, up, down;
    }
}
