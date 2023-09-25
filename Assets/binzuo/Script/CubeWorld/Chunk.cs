using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace binzuo
{
    public class Chunk : MonoBehaviour
    {
        private int vertexStart;
        private int triStart;
        private int meshCount;
        private int m;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private List<Mesh> inputMeshes;
        private ProcessMeshDataJob jobs;
        private Block[,,] blocks;

        private Vector3 location;


        [SerializeField] private Material atlasMaterial;
        private int width;
        private int height;
        private int depth;

        [Header("Perlin Settings")]
        [SerializeField] private float heightOffset = -33f;
        [SerializeField] private float heightScale = 10f;
        [SerializeField] private float scale = 0.001f;
        [SerializeField] private int octaves = 8;
        [SerializeField] private MeshUtils.BlockType[] chunkData;

        public MeshUtils.BlockType[] GetChunkData() => chunkData;
        public int GetWidth() => width;
        public int GetHeight() => height;
        public int GetDepth() => depth;
        public Vector3 GetLocation() => location;

        public float GetHeightOffset() => heightOffset;
        public float GetHeightScale() => heightScale;
        public float GetScale() => scale;
        public int GetOctaves() => octaves;

        private void Awake()
        {
        }
        // Start is called before the first frame update
        void Start()
        {
            //CreateChunk();
        }

        public void CreateChunk(Vector3 dimensions, Vector3 position)
        {
            location = position;
            width = (int)dimensions.x;
            height = (int)dimensions.y;
            depth = (int)dimensions.z;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = atlasMaterial;
            blocks = new Block[width, height, depth];

            inputMeshes = new();
            vertexStart = 0;
            triStart = 0;
            meshCount = width * height * depth;
            m = 0;

            jobs = new ProcessMeshDataJob();
            jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            jobs.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            BuildChunk();
            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        blocks[x, y, z] = new Block(new Vector3(x, y, z) + location, chunkData[x + width * (y + depth * z)], this);
                        if (blocks[x, y, z].mesh != null)
                        {
                            inputMeshes.Add(blocks[x, y, z].mesh);
                            var vCount = blocks[x, y, z].mesh.vertexCount;
                            var iCount = (int)blocks[x, y, z].mesh.GetIndexCount(0);
                            jobs.vertexStart[m] = vertexStart;
                            jobs.triStart[m] = triStart;
                            vertexStart += vCount;
                            triStart += iCount;
                            m++;
                        }
                    }
                }
            }

            jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
            var outputMeshData = Mesh.AllocateWritableMeshData(1);
            jobs.outputMesh = outputMeshData[0];
            jobs.outputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
            jobs.outputMesh.SetVertexBufferParams(vertexStart,
                new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

            var handle = jobs.Schedule(inputMeshes.Count, 4);
            var newMesh = new Mesh();
            newMesh.name = "Chunk_" + location.x + "_" + location.y + "_" + location.z;
            var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
            sm.firstVertex = 0;
            sm.vertexCount = vertexStart;

            handle.Complete();

            jobs.outputMesh.subMeshCount = 1;
            jobs.outputMesh.SetSubMesh(0, sm);
            Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });
            jobs.meshData.Dispose();
            jobs.vertexStart.Dispose();
            jobs.triStart.Dispose();
            newMesh.RecalculateBounds();

            meshFilter.mesh = newMesh;
        }

        public void BuildChunk()
        {
            int blockCount = width * depth * height;
            chunkData = new MeshUtils.BlockType[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                int x = i % width + (int)location.x;
                int y = (i / width) % height + (int)location.y;
                int z = i / (width * height) + (int)location.z;
                if (MeshUtils.FractalBrownianMotion(x, z, octaves, scale, heightScale, heightOffset) > y)
                {
                    chunkData[i] = MeshUtils.BlockType.DIRT;
                }
                else
                {
                    chunkData[i] = MeshUtils.BlockType.AIR;
                }
            }
        }

        [BurstCompile]
        struct ProcessMeshDataJob : IJobParallelFor
        {
            [ReadOnly] public Mesh.MeshDataArray meshData;
            public Mesh.MeshData outputMesh;
            public NativeArray<int> vertexStart;
            public NativeArray<int> triStart;

            public void Execute(int index)
            {
                var data = meshData[index];
                var vCount = data.vertexCount;
                var vStart = vertexStart[index];

                var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                data.GetVertices(verts.Reinterpret<Vector3>());

                var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                data.GetNormals(normals.Reinterpret<Vector3>());

                var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                data.GetUVs(0, uvs.Reinterpret<Vector3>());

                var outputVerts = outputMesh.GetVertexData<Vector3>();
                var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
                var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

                for (int i = 0; i < vCount; i++)
                {
                    outputVerts[i + vStart] = verts[i];
                    outputNormals[i + vStart] = normals[i];
                    outputUVs[i + vStart] = uvs[i];
                }

                verts.Dispose();
                normals.Dispose();
                uvs.Dispose();

                var tStart = triStart[index];
                var tCount = data.GetSubMesh(0).indexCount;
                var outputTris = outputMesh.GetIndexData<int>();
                if (data.indexFormat == IndexFormat.UInt16)
                {
                    var tris = data.GetIndexData<ushort>();
                    for (int i = 0; i < tCount; ++i)
                    {
                        int idx = tris[i];
                        outputTris[i + tStart] = vStart + idx;
                    }
                }
                else
                {
                    var tris = data.GetIndexData<int>();
                    for (int i = 0; i < tCount; ++i)
                    {
                        int idx = tris[i];
                        outputTris[i + tStart] = vStart + idx;
                    }
                }
            }
        }

    }
}