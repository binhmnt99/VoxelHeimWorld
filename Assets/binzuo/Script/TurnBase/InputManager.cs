#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace binzuo
{
    public class InputManager : Singleton<InputManager>
    {
        private UnitInputController inputActions;

        protected override void Awake()
        {
            base.Awake();
            inputActions = new();
            inputActions.Player.Enable();
        }
        public Vector2 GetMouseScreenPosition()
        {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        public bool IsMouseButtonDown()
        {
#if USE_NEW_INPUT_SYSTEM
            return inputActions.Player.MouseClick.WasPressedThisFrame();
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public Vector3 GetCameraMoveVector()
        {
#if USE_NEW_INPUT_SYSTEM
            return inputActions.Player.MoveCamera.ReadValue<Vector2>();
#else

            Vector3 inputMoveDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.z = +1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.z = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x = +1f;
            }

            return inputMoveDir;
#endif
        }

        public float GetCameraRotateAmount()
        {
#if USE_NEW_INPUT_SYSTEM
            return inputActions.Player.RotateCamera.ReadValue<float>();
#else
            float rotateAmount = 0f;

            if (Input.GetKey(KeyCode.Q))
            {
                rotateAmount = +1f;
            }
            if (Input.GetKey(KeyCode.E))
            {
                rotateAmount = -1f;
            }

            return rotateAmount;
#endif
        }

        public float GetCameraZoomAmount()
        {
#if USE_NEW_INPUT_SYSTEM
            return inputActions.Player.ZoomCamera.ReadValue<float>();
#else
            float zoomAmount = 0f;

            if (Input.mouseScrollDelta.y > 0)
            {
                zoomAmount = -1f;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                zoomAmount = +1f;
            }

            return zoomAmount;
#endif
        }

    }
}

