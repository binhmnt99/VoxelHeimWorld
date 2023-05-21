using UnityEngine;

namespace Voxel
{
    public class WaterLayerHandler : VoxelLayerHandler
    {
        public int waterLevel = 3;
        protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (y > surfaceHeightNoise && y <= waterLevel)
            {
                Vector3Int pos = new Vector3Int(x, y, z);
                Chunk.SetBlock(chunkData, pos, VoxelType.WATER);
                if (y == surfaceHeightNoise + 1)
                {
                    pos.y = surfaceHeightNoise;
                    Chunk.SetBlock(chunkData, pos, VoxelType.SAND);
                }
                return true;
            }
            return false;
        }
    }
}

