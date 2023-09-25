using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class World : MonoBehaviour
    {
        public static Vector3 worldDimensions = new Vector3(4, 3, 3);
        public static Vector3 chunkDimensions = new Vector3(10, 10, 10);

        public GameObject chunkPrefabs;


        private void Start() {
            StartCoroutine(BuildWorld());
        }

        IEnumerator BuildWorld()
        {
            for (int z = 0; z < worldDimensions.z; z++)
            {
                for (int y = 0; y < worldDimensions.y; y++)
                {
                    for (int x = 0; x < worldDimensions.x; x++)
                    {
                        GameObject chunk = Instantiate(chunkPrefabs, transform);
                        Vector3 position = new Vector3(x * chunkDimensions.x, (y * chunkDimensions.y) + .5f, z * chunkDimensions.z);
                        chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, position);
                        chunk.isStatic = true;
                        yield return null;
                    }
                }
            }
        }
    }

}