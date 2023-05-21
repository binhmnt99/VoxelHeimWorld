using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public abstract class VoxelLayerHandler : MonoBehaviour
    {
        [SerializeField]
        private VoxelLayerHandler Next;

        public bool Handle(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
        {
            if (TryHandling(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset))
                return true;
            if (Next != null)
                return Next.Handle(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset);
            return false;
        }

        protected abstract bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset);
    }
}

