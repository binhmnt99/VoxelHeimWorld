using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Block
    {

        private List<Quad> quadList;
        private Mesh[] sideMeshes;
        public Mesh mesh;
        private Chunk parentChunk;

        public Block(Vector3 offset, MeshUtils.BlockType blockType, Chunk chunk)
        {
            parentChunk = chunk;
            Vector3 blockLocalPos = offset - chunk.GetLocation();
            if (blockType != MeshUtils.BlockType.AIR)
            {
                quadList = new();
                if (!HasSolidNeightbour((int)blockLocalPos.x, (int)blockLocalPos.y - 1, (int)blockLocalPos.z))
                {
                    quadList.Add(new(MeshUtils.BlockSide.BOTTOM, offset, blockType));
                }
                if (!HasSolidNeightbour((int)blockLocalPos.x, (int)blockLocalPos.y + 1, (int)blockLocalPos.z))
                {
                    quadList.Add(new(MeshUtils.BlockSide.TOP, offset, blockType));
                }
                if (!HasSolidNeightbour((int)blockLocalPos.x - 1, (int)blockLocalPos.y, (int)blockLocalPos.z))
                {
                    quadList.Add(new(MeshUtils.BlockSide.LEFT, offset, blockType));
                }
                if (!HasSolidNeightbour((int)blockLocalPos.x + 1, (int)blockLocalPos.y, (int)blockLocalPos.z))
                {
                    quadList.Add(new(MeshUtils.BlockSide.RIGHT, offset, blockType));
                }
                if (!HasSolidNeightbour((int)blockLocalPos.x, (int)blockLocalPos.y, (int)blockLocalPos.z + 1))
                {
                    quadList.Add(new(MeshUtils.BlockSide.FRONT, offset, blockType));
                }
                if (!HasSolidNeightbour((int)blockLocalPos.x, (int)blockLocalPos.y, (int)blockLocalPos.z - 1))
                {
                    quadList.Add(new(MeshUtils.BlockSide.BACK, offset, blockType));
                }

                if (quadList.Count == 0)
                {
                    return;
                }

                sideMeshes = new Mesh[quadList.Count];

                int m = 0;
                foreach (Quad quad in quadList)
                {
                    sideMeshes[m] = quad.GetMesh();
                    m++;
                }

                mesh = MeshUtils.MergeMeshes(sideMeshes);
                mesh.name = "Block_0_0_0";
            }

        }

        public bool HasSolidNeightbour(int x, int y, int z)
        {
            if (x < 0 || x >= parentChunk.GetWidth() ||
                y < 0 || y >= parentChunk.GetHeight() ||
                z < 0 || z >= parentChunk.GetDepth())
            {
                return false;
            }
            if (parentChunk.GetChunkData()[x + parentChunk.GetWidth() * (y + parentChunk.GetDepth() * z)] == MeshUtils.BlockType.AIR ||
                parentChunk.GetChunkData()[x + parentChunk.GetWidth() * (y + parentChunk.GetDepth() * z)] == MeshUtils.BlockType.WATER)
            {
                return false;
            }
            return true;
        }

    }
}
