using UnityEngine;

namespace Voxel
{
    public class AirLayerHandler : VoxelLayerHandler
    {
        protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (y > surfaceHeightNoise)
            {
                Vector3Int pos = new Vector3Int(x, y, z);
                Chunk.SetBlock(chunkData, pos, VoxelType.AIR);
                return true;
            }
            return false;
        }
    }
}

