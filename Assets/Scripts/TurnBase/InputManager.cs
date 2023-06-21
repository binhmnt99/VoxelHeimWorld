#define USE_NEW_INPUT_SYSTEM
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBase
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        private UnitInputAction unitInputAction;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            unitInputAction = new UnitInputAction();
            unitInputAction.Player.Enable();
        }
        public Vector2 GetMouseScreenPosition()
        {
#if USE_NEW_INPUT_SYSTEM
            return Mouse.current.position.ReadValue();
#else
            return Input.mousePosition;
#endif
        }

        public bool GetLeftMouseButtonDown()
        {
#if USE_NEW_INPUT_SYSTEM
            return unitInputAction.Player.LeftMouseClick.WasPressedThisFrame();
#else
            return Input.GetMouseButtonDown(0);
#endif
        }

        public bool GetRightMouseButtonDown()
        {
#if USE_NEW_INPUT_SYSTEM
            return unitInputAction.Player.RightMouseClick.WasPressedThisFrame();
#else
            return Input.GetMouseButtonDown(1);
#endif
        }

        public Vector2 GetCameraMoveVector()
        {
#if USE_NEW_INPUT_SYSTEM
            return unitInputAction.Player.CameraMovement.ReadValue<Vector2>();
#else
            Vector2 inputMoveDirection = new Vector2(0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDirection.y = 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDirection.y = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDirection.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDirection.x = 1f;
            }
            return inputMoveDirection;
#endif
        }

        public float GetCameraRotateAmount()
        {
#if USE_NEW_INPUT_SYSTEM
            return unitInputAction.Player.CameraRotate.ReadValue<float>();
#else
            float rotateAmount = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                rotateAmount = 1f;
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
            return unitInputAction.Player.CameraZoom.ReadValue<float>();
#else
            float zoomAmount = 0f;
            if (Input.mouseScrollDelta.y > 0)
            {
                zoomAmount = -1f;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                zoomAmount = 1f;
            }
            return zoomAmount;
#endif
        }
    }

}
