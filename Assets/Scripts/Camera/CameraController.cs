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
        private Vector2 inputMoveDirection;
        private Vector3 moveVector;
        private Vector3 rotateVector;
        private CinemachineTransposer cinemachineTransposer;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotateSpeed = 75f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float MIN_FOLLOW_OFFSET_Y = 0f;
        [SerializeField] private float MAX_FOLLOW_OFFSET_Y = 10f;
        //[SerializeField] private float MIN_FOLLOW_OFFSET_Z = -5f;
        //[SerializeField] private float MAX_FOLLOW_OFFSET_Z = 0f;

        private Vector3 targetFollowOffset;

        void Start()
        {
            cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        }
        // Update is called once per frame
        void Update()
        {
            MoveCamera();
            RotateCamera();
            CameraZoom();
        }

        private void MoveCamera()
        {
            inputMoveDirection = InputManager.Instance.GetCameraMoveVector();
            moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
            transform.position += moveVector.normalized * moveSpeed * Time.deltaTime;
        }

        private void RotateCamera()
        {
            rotateVector = new Vector3(0, 0, 0);
            rotateVector.y = InputManager.Instance.GetCameraRotateAmount();
            transform.eulerAngles += rotateVector.normalized * rotateSpeed * Time.deltaTime;
        }

        private void CameraZoom()
        {
            float zoomIncreaseAmount = 1f;
            targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
            targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);
            //targetFollowOffset.z = Mathf.Clamp(targetFollowOffset.z, MIN_FOLLOW_OFFSET_Z, MAX_FOLLOW_OFFSET_Z);
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * zoomSpeed);
        }
    }
}

