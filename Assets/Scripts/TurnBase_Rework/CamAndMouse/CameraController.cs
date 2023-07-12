namespace TurnBaseGrid
{
    using UnityEngine;
    using Cinemachine;

    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float speed = 8f;

        private const float MIN_FOLLOW_OFFSET_Y = 5f;
        private const float MAX_FOLLOW_OFFSET_Y = 45f;

        private CinemachineTransposer cinemachineTransposer;
        private Vector3 targetFollowOffset;
        private Vector2 inputMoveDirection;
        private Vector3 moveVector;
        private Vector3 rotateVector;

        private void Start()
        {
            cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            targetFollowOffset = cinemachineTransposer.m_FollowOffset;
        }

        private void Update()
        {
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            Vector3 input = InputValues(out float yRotation).normalized;
            transform.Translate(new Vector3(input.x, 0f, input.z) * speed * Time.deltaTime);
            transform.Rotate(Vector3.up * yRotation * speed * Time.deltaTime * 4);
            targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y + input.y * 2f, MIN_FOLLOW_OFFSET_Y,MAX_FOLLOW_OFFSET_Y);
            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, Time.deltaTime * speed);
        }

        private Vector3 InputValues(out float y)
        {
            //Move and zoom
            Vector3 values = new Vector3();
            values.x = InputManager.Instance.GetCameraMoveVector().x;
            values.z = InputManager.Instance.GetCameraMoveVector().y;
            values.y = InputManager.Instance.GetCameraZoomAmount();

            //Rotation
            y = 0;
            y = InputManager.Instance.GetCameraRotateAmount();
            return values;
        }
    }
}