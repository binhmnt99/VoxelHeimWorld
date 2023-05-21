using System.Collections.Generic;
using UnityEngine;
namespace Voxel
{
    public class WorldRenderer : MonoBehaviour
    {
        public GameObject chunkPrefab;
        public GameObject chunkParent;
        public Queue<ChunkRenderer> chunkPool = new Queue<ChunkRenderer>();

        public void Clear(WorldData worldData)
        {
            foreach (var item in worldData.chunkDictionary.Values)
            {
                Destroy(item.gameObject);
            }
            chunkPool.Clear();
        }


        public ChunkRenderer RenderChunk(WorldData worldData, Vector3Int position, MeshData meshData)
        {
            ChunkRenderer newChunk = null;
            if (chunkPool.Count > 0)
            {
                newChunk = chunkPool.Dequeue();
                newChunk.transform.position = position;
            }
            else
            {
                GameObject chunkObject = Instantiate(chunkPrefab, position, Quaternion.identity,chunkParent.transform);
                newChunk = chunkObject.GetComponent<ChunkRenderer>();
            }

            newChunk.InitializeChunk(worldData.chunkDataDictionary[position]);
            newChunk.UpdateChunk(meshData);
            newChunk.gameObject.SetActive(true);
            return newChunk;
        }

        public void RemoveChunk(ChunkRenderer chunk)
        {
            chunk.gameObject.SetActive(false);
            chunkPool.Enqueue(chunk);
        }
    }
}

