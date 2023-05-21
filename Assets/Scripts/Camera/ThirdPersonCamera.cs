using UnityEngine;
using Pathfinding;

namespace Voxel
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("References")]
        public Transform orientation;
        public Transform player;
        public Transform playerObject;

        public float rotationSpeed = 5;

        public Transform combatLookAt;

        private Vector2 player2DInput;

        public CameraStyle currentStyle;

        public GameObject thirdPersonCam;
        public GameObject combatCam;
        public GameObject topDownCam;

        public enum CameraStyle
        {
            Basic,
            Combat,
            Topdown,
        }

        private void Awake()
        {

            SwitchCameraStyle(currentStyle);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }

        private void ThirdPersonControl()
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;
            if (currentStyle == CameraStyle.Basic || currentStyle == CameraStyle.Topdown)
            {
                player2DInput = GameObject.FindObjectOfType<CharacterControls>().character2DInput;
                Vector3 inputDir = orientation.forward * player2DInput.y + orientation.right * player2DInput.x;

                if (inputDir != Vector3.zero)
                    playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
            else if (currentStyle == CameraStyle.Combat)
            {
                Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
                orientation.forward = dirToCombatLookAt.normalized;

                playerObject.forward = dirToCombatLookAt.normalized;
            }
        }

        public void SwitchCameraStyle(CameraStyle newStyle)
        {
            combatCam.SetActive(false);
            thirdPersonCam.SetActive(false);
            //topDownCam.SetActive(false);

            if (newStyle == CameraStyle.Basic) thirdPersonCam.SetActive(true);
            if (newStyle == CameraStyle.Combat) combatCam.SetActive(true);
            if (newStyle == CameraStyle.Topdown) topDownCam.SetActive(true);

            currentStyle = newStyle;
        }

        private void Update()
        {
            ThirdPersonControl();
        }

    }
}

