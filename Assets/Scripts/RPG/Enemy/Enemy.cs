using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pathfinding;
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
        //[SerializeField] private Rigidbody rigid;
        [SerializeField] private AIPath aiPath;
        [SerializeField] private AIDestinationSetter aiDestinationSetter;
        [SerializeField] private Transform hightBlockTransform;
        [SerializeField] private Transform lowBlockTransform;
        [SerializeField] private VoxelType hightBlockType;
        [SerializeField] private VoxelType lowBlockType;
        //[SerializeField] GameObject hitVFX;
        //[SerializeField] GameObject ragdoll;
        public float jumpForce = 6f;

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
        [SerializeField] private LayerMask layerMask;

        public event Action onDead;

        GameObject player;
        Animator animator;
        float timePassed;
        float newDestinationCD = 0.5f;

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
            if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
            {

                newDestinationCD = 0.5f;
                aiDestinationSetter.target = player.transform;
                inAggroRange = true;
            }
            else
            {
                aiDestinationSetter.target = null;
                inAggroRange = false;
            }
            newDestinationCD -= Time.deltaTime;
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
            World world = GameObject.Find("World").GetComponent<World>();
            Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(world, (int)(transform.position.x), (int)(transform.position.y), (int)(transform.position.z));

            ChunkData containerChunk = null;

            world.worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);
            Debug.Log(containerChunk.worldPosition);
            if (containerChunk == null)
            {
                hightBlockType = VoxelType.NOTHING;
                lowBlockType = VoxelType.NOTHING;
            }

            Vector3Int hightBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int((int)(transform.position.x + hightBlockTransform.position.x), (int)(transform.position.y + hightBlockTransform.position.y), (int)(transform.position.z + hightBlockTransform.position.z)));
            Vector3Int lowBlockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int((int)(transform.position.x + lowBlockTransform.position.x), (int)(transform.position.y + lowBlockTransform.position.y), (int)(transform.position.z + lowBlockTransform.position.z)));
            hightBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, hightBlockInCHunkCoordinates);
            lowBlockType = Chunk.GetBlockFromChunkCoordinates(containerChunk, lowBlockInCHunkCoordinates);
        }

        void Update()
        {
            if (player == null)
            {
                return;
            }

            DisableOnDistance();

            Movement();

            Attack();

            Aggro();

            CheckInRange();

            CheckFrontBlockNearEnemy();

            if (transform.localPosition.y <= -100)
            {
                Die();
            }
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
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                //print(true);
                player = collision.gameObject;
            }
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

