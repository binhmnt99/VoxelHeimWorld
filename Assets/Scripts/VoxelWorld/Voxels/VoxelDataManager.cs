using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public class VoxelDataManager : MonoBehaviour
    {
        public static float textureOffset = 0.001f;
        public static float tileSizeX, tileSizeY;
        public static Dictionary<VoxelType, TexturesData> blockTextureDataDictionary = new Dictionary<VoxelType, TexturesData>();
        public VoxelDataSO voxelData;

        private void Awake()
        {
            foreach (var item in voxelData.texturesData)
            {
                if (blockTextureDataDictionary.ContainsKey(item.voxelType) == false)
                {
                    blockTextureDataDictionary.Add(item.voxelType, item);
                };
            }
            tileSizeX = voxelData.textureSizeX;
            tileSizeY = voxelData.textureSizeY;
        }
    }
}
