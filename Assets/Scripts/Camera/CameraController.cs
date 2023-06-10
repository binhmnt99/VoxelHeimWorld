using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace TurnBase
{
    public class CameraController : MonoBehaviour
    {
        private Vector3 inputMoveDir;
        private Vector3 moveVector;
        private Vector3 rotateVector;
        private CameraInput cameraInput;
        private CinemachineTransposer cinemachineTransposer;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotateSpeed = 75f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float MIN_FOLLOW_OFFSET_Y = 2f;
        [SerializeField] private float MAX_FOLLOW_OFFSET_Y = 12f;
        [SerializeField] private float MIN_FOLLOW_OFFSET_Z = -10f;
        [SerializeField] private float MAX_FOLLOW_OFFSET_Z = -5f;

        void Awake()
        {
            cameraInput = GetComponent<CameraInput>();
        }


        private void MoveCamera()
        {
            if (cameraInput.moveDirection != Vector2.zero)
            {
                inputMoveDir = new Vector3(cameraInput.moveDirection.x, 0, cameraInput.moveDirection.y);
                moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
                transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;
            }
        }

        private void RotateCamera()
        {
            if (cameraInput.rotateAxis != 0)
            {
                rotateVector = new Vector3(0, cameraInput.rotateAxis, 0);
                transform.eulerAngles += rotateVector.normalized * rotateSpeed * Time.deltaTime;
            }
        }

        private void CameraZoom()
        {
            if (cameraInput.zoomAxis != 0)
            {
                cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                Vector3 followOffset = cinemachineTransposer.m_FollowOffset;
                float zoomAmount = 1f;
                if (cameraInput.zoomAxis > 0)
                {
                    followOffset.y -= zoomAmount;
                    followOffset.z += zoomAmount;

                }
                if (cameraInput.zoomAxis < 0)
                {
                    followOffset.y += zoomAmount;
                    followOffset.z -= zoomAmount;

                }
                followOffset.y = Mathf.Clamp(followOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);
                followOffset.z = Mathf.Clamp(followOffset.z, MIN_FOLLOW_OFFSET_Z, MAX_FOLLOW_OFFSET_Z);
                cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
            }
        }
        // Update is called once per frame
        void Update()
        {
            MoveCamera();
            RotateCamera();
            CameraZoom();
        }
    }
}

