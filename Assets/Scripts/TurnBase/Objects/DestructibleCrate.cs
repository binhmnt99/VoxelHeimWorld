using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class DestructibleCrate : MonoBehaviour
    {
        public static event EventHandler OnAnyDestroyed;
        [SerializeField] private Transform crateDestroyedPrefab;
        private GridPosition gridPosition;
        void Start()
        {
            gridPosition = HexLevelGrid.Instance.GetGridPosition(transform.position);
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
        public void Damage()
        {
            Transform crateDestroyTransform = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
            ApplyExplosionToChildren(crateDestroyTransform,150f,transform.position,10f);
            Destroy(gameObject);
            OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
        }

        private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
        {
            foreach (Transform child in root)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                {
                    childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                }
                ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }
}