using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class LookAtCamera : MonoBehaviour {
        private Transform cameraTransform;

        void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }
}

