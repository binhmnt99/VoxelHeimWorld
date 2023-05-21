using UnityEngine;

namespace Voxel
{
    public class UndergroundLayerHandler : VoxelLayerHandler
    {
        public VoxelType undergroundVoxelType;
        protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (y < surfaceHeightNoise)
            {
                Vector3Int pos = new Vector3Int(x, y - chunkData.worldPosition.y, z);
                Chunk.SetBlock(chunkData, pos, undergroundVoxelType);
                return true;
            }
            return false;
        }
    }
}

