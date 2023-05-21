using UnityEngine;

namespace Voxel
{
    public class SurfaceLayerHandler : VoxelLayerHandler
    {
        public VoxelType surfaceVoxelType;
        protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (y == surfaceHeightNoise)
            {
                Vector3Int pos = new Vector3Int(x, y, z);
                Chunk.SetBlock(chunkData, pos, surfaceVoxelType);
                return true;
            }
            return false;
        }
    }
}

