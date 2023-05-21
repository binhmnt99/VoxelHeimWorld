using UnityEngine;

namespace Voxel
{
    public static class VoxelHelper
    {
        private static Direction[] directions =
        {
            Direction.UP,
            Direction.DOWN,
            Direction.LEFT,
            Direction.RIGHT,
            Direction.FRONT,
            Direction.BACK
        };

        public static MeshData GetMeshData
        (ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            if (voxelType == VoxelType.AIR || voxelType == VoxelType.NOTHING)
                return meshData;

            foreach (Direction direction in directions)
            {
                var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
                var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);

                if (neighbourBlockType != VoxelType.NOTHING && VoxelDataManager.blockTextureDataDictionary[neighbourBlockType].isSolid == false)
                {

                    if (voxelType == VoxelType.WATER)
                    {
                        if (neighbourBlockType == VoxelType.AIR)
                            meshData.waterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.waterMesh, voxelType);
                    }
                    else
                    {
                        meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, voxelType);
                    }

                }
            }

            return meshData;
        }

        public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            GetFaceVertices(direction, x, y, z, meshData, voxelType);
            meshData.AddQuadTriangles(VoxelDataManager.blockTextureDataDictionary[voxelType].isCollider);
            meshData.uv.AddRange(FaceUVs(direction, voxelType));


            return meshData;
        }

        public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelType voxelType)
        {
            var isCollider = VoxelDataManager.blockTextureDataDictionary[voxelType].isCollider;
            //order of vertices matters for the normals and how we render the mesh
            switch (direction)
            {
                case Direction.BACK:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    break;
                case Direction.FRONT:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    break;
                case Direction.LEFT:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    break;

                case Direction.RIGHT:
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    break;
                case Direction.DOWN:
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), isCollider);
                    break;
                case Direction.UP:
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), isCollider);
                    break;
                default:
                    break;
            }
        }

        public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
        {
            Vector2[] UVs = new Vector2[4];
            var tilePos = TexturePosition(direction, voxelType);

            UVs[0] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

            UVs[1] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

            UVs[2] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

            UVs[3] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
                VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

            return UVs;
        }

        public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
        {
            return direction switch
            {
                Direction.UP => VoxelDataManager.blockTextureDataDictionary[voxelType].up,
                Direction.DOWN => VoxelDataManager.blockTextureDataDictionary[voxelType].down,
                _ => VoxelDataManager.blockTextureDataDictionary[voxelType].side
            };
        }
    }
}
