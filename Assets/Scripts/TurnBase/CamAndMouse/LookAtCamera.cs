using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField]private Transform cameraTransform;

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