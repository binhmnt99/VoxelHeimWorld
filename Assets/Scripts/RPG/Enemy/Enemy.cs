using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pathfinding;
using Pathfinding.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace Voxel
{
    public class Enemy : MonoBehaviour
    {
        [Header("Health UI")]
        private GameObject enemyCanvas;
        public Slider slider;
        public Gradient gradient;
        public Image fill;

        private bool _isStopped = false;
        public float _distanceToPlayer;

        [Header("AI Checker")]
        [SerializeField] private AIPath aiPath;
        [SerializeField] private AIDestinationSetter aiDestinationSetter;
        [SerializeField] private VoxelType hightBlockType;
        [SerializeField] private int hightYOffset = 2;
        [SerializeField] private VoxelType midBlockType;
        [SerializeField] private int midYOffset = 1;
        [SerializeField] private VoxelType lowBlockType;
        [SerializeField] private int lowYOffset = 0;
        [SerializeField] private VoxelType entityBlockType;
        [SerializeField] private Vector3Int entityOffset = new Vector3Int(1, 0, 1);
        //[SerializeField] GameObject hitVFX;
        //[SerializeField] GameObject ragdoll;

        [Header("Combat")]
        [SerializeField] private float maxHealth = 8;
        [SerializeField] private float health;
        [SerializeField] private float damage = 2;
        [SerializeField] private float movementSpeed = 4;
        [SerializeField] private float attackSpeed = 0.625f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private bool inAttackRange = false;
        [SerializeField] private float aggroRange = 10f;
        [SerializeField] private bool inAggroRange = false;

        [Header("Ground Check")]
        [SerializeField]
        private bool grounded;
        private EnemyWallCheck enemyWallCheck;

        public event Action onDead;

        GameObject player;
        Animator animator;
        float timePassed;
        Vector3 nextPosLerp = Vector3.zero;

        public float GetDamage()
        {
            return damage;
        }

        void Start()
        {
            //----------CB----------
            health = maxHealth;
            //----------UI----------
            enemyCanvas = GetComponentInChildren<Canvas>().gameObject;
            slider.maxValue = maxHealth;
            slider.minValue = 0f;
            slider.value = health;
            fill.color = gradient.Evaluate(1f);
            //----------AI----------
            aiPath = GetComponent<AIPath>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            animator = GetComponentInChildren<Animator>();
            player = GameObject.FindGameObjectWithTag("Player");
            aiPath.maxSpeed = movementSpeed;

            enemyWallCheck = GetComponentInChildren<EnemyWallCheck>();
        }

        private void Movement()
        {
            animator.SetFloat("speed", aiPath.velocity.magnitude / aiPath.maxSpeed);
        }

        private void Attack()
        {
            if (timePassed >= (1 / attackSpeed))
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                {
                    animator.SetTrigger("attack");
                    timePassed = 0;
                    inAttackRange = true;
                }
                else
                {
                    inAttackRange = false;
                }
            }
            timePassed += Time.deltaTime;
        }

        private void Aggro()
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
            {

                aiDestinationSetter.target = player.transform;
                inAggroRange = true;
            }
            else
            {
                aiDestinationSetter.target = transform;
                inAggroRange = false;
            }
        }

        private void CheckInRange()
        {
            if (inAggroRange && inAttackRange)
            {
                aiPath.maxSpeed = 0;
            }
            else if (inAggroRange)
            {
                aiPath.maxSpeed = movementSpeed;
            }
        }

        private void CheckFrontBlockNearEnemy()
        {
            if (transform.position.y < 0)
            {
                return;
            }

            World world = GameObject.Find("World").GetComponent<World>();
            Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(world, (int)(transform.position.x), (int)(transform.position.y), (int)(transform.position.z));

            ChunkData containerChunk = null;

            world.worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

            if (containerChunk == null)
            {
                hightBlockType = VoxelType.NOTHING;
                lowBlockType = VoxelType.NOTHING;
            }

            Vector3Int blockPos = GetComponentInChildren<EnemyCheckBlock>().blockPos;

            Vector3Int entityBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int((int)transform.position.x + entityOffset.x, (int)transform.position.y + entityOffset.y, (int)transform.position.z + entityOffset.z));

            Vector3Int hightBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(blockPos.x, (int)transform.position.y + hightYOffset, blockPos.z));

            Vector3Int midBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(blockPos.x, (int)transform.position.y + midYOffset, blockPos.z));

            Vector3Int lowBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(blockPos.x, (int)transform.position.y + lowYOffset, blockPos.z));

            hightBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, hightBlockInCHunkCoordinates);
            midBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, midBlockInCHunkCoordinates);
            lowBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, lowBlockInCHunkCoordinates);
            entityBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, entityBlockInCHunkCoordinates);
        }


        private void CheckBlockToChangeYPos()
        {
            if (inAggroRange && grounded)
            {
                if (hightBlockType == VoxelType.AIR && midBlockType != hightBlockType)
                {

                    aiPath.Move(transform.up.normalized);
                }
                if (lowBlockType == VoxelType.AIR)
                {
                    aiPath.Move(transform.forward.normalized*0.1f);
                }
            }
        }



        private void GroundCheck()
        {
            grounded = enemyWallCheck.isWall;
        }

        void Update()
        {
            if (player == null)
            {
                return;
            }

            GroundCheck();

            DisableOnDistance();

            Movement();

            Attack();

            Aggro();

            CheckInRange();

            CheckFrontBlockNearEnemy();

            if (transform.localPosition.y <= -10)
            {
                Die();
            }
        }

        private void FixedUpdate()
        {
            CheckBlockToChangeYPos();
        }

        private void DisableOnDistance()
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            bool needToStop = distance > 100f;
            _distanceToPlayer = distance;
            if (needToStop != _isStopped)
            {
                _isStopped = needToStop;
                aiPath.enabled = !_isStopped;
            }
        }


        private void LateUpdate()
        {
            enemyCanvas.transform.LookAt(enemyCanvas.transform.position + Camera.main.transform.forward);
            Vector3 pos = Vector3.zero;
        }

        void Die()
        {
            //Instantiate(ragdoll, transform.position,transform.rotation);
            Destroy(this.gameObject);
            onDead?.Invoke();
            onDead = null;
        }

        public void TakeDamage(float damageAmount)
        {
            health -= damageAmount;
            animator.SetTrigger("hit");
            slider.value = health;
            //CameraShake.Instance.ShakeCamera(2f, 0.2f);

            if (health <= 0)
            {
                Die();
            }
        }
        public void StartDealDamage()
        {
            GetComponentInChildren<EnemyDamageDealer>().StartDealDamage();
        }
        public void EndDealDamage()
        {
            GetComponentInChildren<EnemyDamageDealer>().EndDealDamage();
        }

        // public void HitVFX(Vector3 hitPosition)
        // {
        //     GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        //     Destroy(hit, 3f);
        // }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 1f);
        }
    }
}

