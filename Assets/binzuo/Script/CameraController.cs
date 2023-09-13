using UnityEngine;
using Cinemachine;

namespace binzuo
{
    public class CameraController : MonoBehaviour
    {
        private const float MIN_FOLLOW_OFFSET_Y = 2f;
        private const float MAX_FOLLOW_OFFSET_Y = 12f;
        [SerializeField] private float cameraMoveSpeed = 10f;
        [SerializeField] private float cameraRotateSpeed = 100f;
        [SerializeField] private float cameraZoomSpeed = 5f;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        private CinemachineTransposer cinemachineTransposer;
        private Vector3 targetFollowOffset;

        private void Start()
        {
            cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        }
        private void Update()
        {
            HandleMovement();

            HandleRotation();

            HandleZoom();
        }

        private void HandleMovement()
        {
            Vector2 inputMoveDirection = InputManager.Instance.GetCameraMoveVector();

            Vector3 moveVector = transform.forward * inputMoveDirection.y + transform.right * inputMoveDirection.x;
            transform.position += moveVector * cameraMoveSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            Vector3 rotationVector = new Vector3(0, 0, 0);

            rotationVector.y = InputManager.Instance.GetCameraRotateAmount();

            transform.eulerAngles += rotationVector * cameraRotateSpeed * Time.deltaTime;
        }

        private void HandleZoom()
        {
            float zoomAmount = 1f;
            targetFollowOffset.y += InputManager.Instance.GetCameraZoomAmount() * zoomAmount;
            targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_OFFSET_Y, MAX_FOLLOW_OFFSET_Y);
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * cameraZoomSpeed);
        }
    }

}