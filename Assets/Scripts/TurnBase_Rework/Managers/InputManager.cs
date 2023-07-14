#define USE_NEW_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private UnitInputAction unitInputAction;
    protected override void Awake()
    {
        base.Awake();
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

