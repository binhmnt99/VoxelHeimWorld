using System;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

namespace binzuo
{
    public static class MeshUtils
    {
        public enum BlockSide
        {
            BOTTOM,
            TOP,
            LEFT,
            RIGHT,
            FRONT,
            BACK
        }
        public enum BlockType
        {
            GRASSTOP,
            GRASSSIDE,
            DIRT,
            WATER,
            STONE,
            SAND,
            AIR
        }

        public static Vector2[,] blockUVs =
        {
            /*GRASSTOP*/ {new Vector2(0.125f*0, 0.125f*5),
                          new Vector2(0.125f*1, 0.125f*5),
                          new Vector2(0.125f*0, 0.125f*6),
                          new Vector2(0.125f*1, 0.125f*6)},

            /*GRASSSIDE*/ {new Vector2(0.125f*3, 0.125f*5),
                           new Vector2(0.125f*4, 0.125f*5),
                           new Vector2(0.125f*3, 0.125f*6),
                           new Vector2(0.125f*4, 0.125f*6)},

            /*DIRT*/ {new Vector2(0.125f*0, 0.125f*6),
                      new Vector2(0.125f*1, 0.125f*6),
                      new Vector2(0.125f*0, 0.125f*7),
                      new Vector2(0.125f*1, 0.125f*7)},

            /*WATER*/ {new Vector2(0.125f*5, 0.125f*3),
                       new Vector2(0.125f*6, 0.125f*3),
                       new Vector2(0.125f*5, 0.125f*4),
                       new Vector2(0.125f*6, 0.125f*4)},

            /*STONE*/ {new Vector2(0.125f*0, 0.125f*7),
                       new Vector2(0.125f*1, 0.125f*7),
                       new Vector2(0.125f*0, 0.125f*8),
                       new Vector2(0.125f*1, 0.125f*8)}, 

            /*SAND*/ {new Vector2(0.125f*6, 0.125f*3),
                      new Vector2(0.125f*7, 0.125f*3),
                      new Vector2(0.125f*6, 0.125f*4),
                      new Vector2(0.125f*7, 0.125f*4)},
        };
        public static Mesh MergeMeshes(Mesh[] meshes)
        {
            Mesh mesh = new Mesh();

            Dictionary<VertexData, int> pointsOrder = new();
            HashSet<VertexData> pointsHash = new();
            List<int> tris = new();

            int pIndex = 0;
            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] == null) continue;
                for (int j = 0; j < meshes[i].vertices.Length; j++)
                {
                    Vector3 v = meshes[i].vertices[j];
                    Vector3 n = meshes[i].normals[j];
                    Vector2 u = meshes[i].uv[j];
                    VertexData p = new VertexData(v, n, u);
                    if (!pointsHash.Contains(p))
                    {
                        pointsOrder.Add(p, pIndex);
                        pointsHash.Add(p);

                        pIndex++;
                    }

                }

                for (int t = 0; t < meshes[i].triangles.Length; t++)
                {
                    int triPoint = meshes[i].triangles[t];
                    Vector3 v = meshes[i].vertices[triPoint];
                    Vector3 n = meshes[i].normals[triPoint];
                    Vector2 u = meshes[i].uv[triPoint];
                    VertexData p = new VertexData(v, n, u);

                    int index;
                    pointsOrder.TryGetValue(p, out index);
                    tris.Add(index);
                }
                meshes[i] = null;
            }

            ExtractArrays(pointsOrder, mesh);
            mesh.triangles = tris.ToArray();
            mesh.RecalculateBounds();
            return mesh;

        }

        public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
        {
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();

            foreach (VertexData v in list.Keys)
            {
                verts.Add(v.Item1);
                norms.Add(v.Item2);
                uvs.Add(v.Item3);
            }
            mesh.vertices = verts.ToArray();
            mesh.normals = norms.ToArray();
            mesh.uv = uvs.ToArray();
        }

        public static float FractalBrownianMotion(float x, float z, int octaves, float scale, float heightScale, float heightOffset)
        {
            float total = 0;
            float frequency = 1;
            for (int i = 0; i < octaves; i++)
            {
                total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
                frequency *= 2;
            }
            return total + heightOffset;
        }
    }
}