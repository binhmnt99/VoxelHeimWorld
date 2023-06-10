using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private bool invert;
        private Transform cameraTransform;

        void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        void LateUpdate()
        {
            if (invert)
            {
                Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;
                transform.LookAt(transform.position + directionToCamera * -1);
            }
            else
            {
                transform.LookAt(cameraTransform);
            }
        }
    }
}