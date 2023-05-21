using UnityEngine;
using TMPro;

namespace Voxel
{
    public class EnemySpawner : MonoBehaviour
    {
        private int enemyCount = 0;
        public GameObject task;
        public GameObject enemyPrefab;         // the enemy object to spawn
        public float spawnDelay = 5f;          // the delay between spawns
        public int maxEnemies = 6;             // the maximum number of enemies allowed
        public float spawnRange = 10f;         // the range within which enemies can spawn

        private int currentEnemies = 0;        // the current number of spawned enemies
        private GameObject newEnemy;



        // Use this for initialization
        void Start()
        {
            InvokeRepeating("SpawnEnemy", spawnDelay, spawnDelay);
        }

        // Spawns a new enemy if the maximum number of enemies has not been reached
        void SpawnEnemy()
        {
            if (currentEnemies < maxEnemies)
            {
                Vector3 spawnPosition = RandomSpawnPosition();
                newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                newEnemy.GetComponent<Enemy>().onDead += EnemyDestroyed;
                currentEnemies++;
            }
        }

        private Vector3 RandomSpawnPosition()
        {
            Vector3 randomOffset = new(
                Random.Range(-spawnRange, spawnRange),
                0,
                Random.Range(-spawnRange, spawnRange));

            return FindLandingPoint(transform.position + randomOffset);
        }

        private Vector3 FindLandingPoint(Vector3 originalPosition)
        {
            Vector3 landingPoint = originalPosition;
            Ray lookDownRay = new(originalPosition + Vector3.up * 5f, Vector3.down);
            if (Physics.Raycast(lookDownRay, out RaycastHit hitInfo, 10f))
            {
                landingPoint = hitInfo.point;
            }
            return landingPoint;
        }

        // Decrements the current number of enemies when an enemy is destroyed
        public void EnemyDestroyed()
        {
            currentEnemies--;
            enemyCount++;
            task.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = enemyCount + "/6 Goblin";
            if (enemyCount >= maxEnemies)
            {
                task.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = maxEnemies + "/6 Goblin";

                GameObject canvasObject = GameObject.Find("Canvas");
                canvasObject.transform.GetChild(0).gameObject.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

}

