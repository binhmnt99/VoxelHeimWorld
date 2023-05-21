using UnityEngine;

namespace Voxel
{
    public class ChunkData
    {
    public VoxelType[] blocks;
    public int chunkSize = 16;
    public int chunkHeight = 100;
    public World worldReference;
    public Vector3Int worldPosition;

    public bool modifiedByThePlayer = false;
    public TreeData treeData;

    public ChunkData(int chunkSize, int chunkHeight, World world, Vector3Int worldPosition)
    {
        this.chunkHeight = chunkHeight;
        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        blocks = new VoxelType[chunkSize * chunkHeight * chunkSize];
    }
    }
}

