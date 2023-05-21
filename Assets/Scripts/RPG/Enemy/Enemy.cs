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
        private GameObject enemyCanvas;
        public Slider slider;
        public Gradient gradient;
        public Image fill;

        private bool _isStopped = false;
        public float _distanceToPlayer;

        //[SerializeField] private Rigidbody rigid;
        [SerializeField] private AIPath aiPath;
        [SerializeField] float health = 3;
        [SerializeField] float damage = 2;
        //[SerializeField] GameObject hitVFX;
        //[SerializeField] GameObject ragdoll;

        [Header("Combat")]
        [SerializeField] float attackCD = 3f;
        [SerializeField] float attackRange = 1f;
        [SerializeField] float aggroRange = 4f;
        [SerializeField] Vector3 offset = new Vector3(0, 0.5f, 0);
        [SerializeField] LayerMask layerMask;
        [SerializeField] float maxDistance;
        private RaycastHit hit;

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
            enemyCanvas = transform.GetChild(1).gameObject;
            slider.maxValue = health;
            slider.value = health;
            fill.color = gradient.Evaluate(1f);
            animator = GetComponentInChildren<Animator>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        void Update()
        {
            if (player == null)
            {
                return;
            }

            DisableOnDistance();

            if (Physics.BoxCast(transform.position + offset, transform.lossyScale / 2, transform.forward, out hit,
                transform.rotation, maxDistance, layerMask))
            {

            }

            animator.SetFloat("speed", GetComponent<AIPath>().velocity.magnitude / GetComponent<AIPath>().maxSpeed);

            if (timePassed >= attackCD)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                {
                    animator.SetTrigger("attack");
                    timePassed = 0;
                }
            }
            timePassed += Time.deltaTime;

            if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
            {

                newDestinationCD = 0.5f;
                GetComponent<AIDestinationSetter>().target = player.transform;
            }
            else
            {
                transform.LookAt(player.transform);
            }
            newDestinationCD -= Time.deltaTime;

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
            Gizmos.DrawWireCube(transform.position + offset + transform.forward * maxDistance, transform.lossyScale / 2);
        }
    }
}

