using Cinemachine;
using System.Collections;
using UnityEngine;
using Pathfinding;

namespace Voxel
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        private GameObject player;
        public Vector3Int currentPlayerChunkPosition;
        private Vector3Int currentChunkCenter = Vector3Int.zero;

        public World world;

        public float detectionTime = 1;
        public CinemachineFreeLook camera_basic;
        public CinemachineFreeLook camera_combat;
        private ThirdPersonCamera mainCamera;
        private ProceduralGridMover proceduralGridMover;

        public void SpawnPlayer()
        {
            if (player != null)
                return;
            Vector3Int raycastStartposition = new Vector3Int(world.chunkSize / 2, 100, world.chunkSize / 2);
            RaycastHit hit;
            if (Physics.Raycast(raycastStartposition, Vector3.down, out hit, 120))
            {
                mainCamera = Camera.main.GetComponent<ThirdPersonCamera>();
                proceduralGridMover = Camera.main.GetComponent<ProceduralGridMover>();
                player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
                camera_basic.Follow = player.transform;
                camera_basic.LookAt = player.transform;
                camera_combat.Follow = player.transform;
                camera_combat.LookAt = player.transform.GetChild(1).transform.GetChild(0);

                mainCamera.orientation = player.transform.GetChild(1);
                mainCamera.player = player.transform;
                mainCamera.playerObject = player.transform.GetChild(0);
                mainCamera.combatLookAt = player.transform.GetChild(1).transform.GetChild(0);
                mainCamera.gameObject.SetActive(true);
                proceduralGridMover.target = player.transform;
                proceduralGridMover.gameObject.SetActive(true);
                //AstarPath.active.Scan();
                StartCheckingTheMap();
            }
        }

        public void StartCheckingTheMap()
        {
            SetCurrentChunkCoordinates();
            StopAllCoroutines();
            StartCoroutine(CheckIfShouldLoadNextPosition());
        }

        IEnumerator CheckIfShouldLoadNextPosition()
        {
            yield return new WaitForSeconds(detectionTime);
            if (
                Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.chunkSize ||
                Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.chunkSize ||
                (Mathf.Abs(currentPlayerChunkPosition.y - player.transform.position.y) > world.chunkHeight)
                )
            {
                world.LoadAdditionalChunksRequest(player);

            }
            else
            {
                StartCoroutine(CheckIfShouldLoadNextPosition());
            }
        }

        private void SetCurrentChunkCoordinates()
        {
            currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromBlockCoords(world, Vector3Int.RoundToInt(player.transform.position));
            currentChunkCenter.x = currentPlayerChunkPosition.x + world.chunkSize / 2;
            currentChunkCenter.z = currentPlayerChunkPosition.z + world.chunkSize / 2;
        }
    }
}
