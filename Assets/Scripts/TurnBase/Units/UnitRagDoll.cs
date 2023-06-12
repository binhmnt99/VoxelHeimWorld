using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class UnitRagDoll : MonoBehaviour
    {
        [SerializeField] private Transform ragDollRootBone;

        public void SetUp(Transform originalRootBone)
        {
            MatchAllChildTransforms(originalRootBone, ragDollRootBone);
            ApplyExplosionToRagDoll(ragDollRootBone, 450f, transform.position, 10f);
        }

        private void MatchAllChildTransforms(Transform root, Transform clone)
        {
            foreach (Transform child in root)
            {
                Transform cloneChild = clone.Find(child.name);
                if (cloneChild != null)
                {
                    cloneChild.position = child.position;
                    cloneChild.position = child.position;
                    MatchAllChildTransforms(child, cloneChild);
                }
            }
        }

        private void ApplyExplosionToRagDoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
        {
            foreach (Transform child in root)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
                {
                    childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
                }

                ApplyExplosionToRagDoll(child, explosionForce, explosionPosition, explosionRange);
            }
        }
    }

}