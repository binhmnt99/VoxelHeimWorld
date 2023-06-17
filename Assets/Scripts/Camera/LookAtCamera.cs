using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform cameraTransform;

        void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }
}