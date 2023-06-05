using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBase
{
    public class CameraInput : MonoBehaviour
    {
        private CameraInputController camInput;
        public Vector2 moveDirection { get; private set; }
        public float rotateAxis { get; private set; }
        public float zoomAxis { get; private set; }
        // Start is called before the first frame update
        void Awake()
        {
            camInput = new CameraInputController();
            camInput.Camera.Enable();
            CameraMovement();
            CameraRotation();
            CameraZoom();
        }

        private void CameraZoom()
        {
            camInput.Camera.Zoom.started += CameraZoom;
            camInput.Camera.Zoom.performed += CameraZoom;
            camInput.Camera.Zoom.canceled += CameraZoom;
        }

        private void CameraZoom(InputAction.CallbackContext context)
        {
            if (context.started)
            {

            }
            if (context.performed)
            {
                zoomAxis = context.ReadValue<float>();
            }
            if (context.canceled)
            {
                zoomAxis = 0f;
            }
        }

        private void CameraRotation()
        {
            camInput.Camera.Rotate.started += RotateCamera;
            camInput.Camera.Rotate.performed += RotateCamera;
            camInput.Camera.Rotate.canceled += RotateCamera;
        }

        private void RotateCamera(InputAction.CallbackContext context)
        {
            if (context.started)
            {

            }
            if (context.performed)
            {
                rotateAxis = context.ReadValue<float>();
            }
            if (context.canceled)
            {
                rotateAxis = 0f;
            }
        }

        private void CameraMovement()
        {
            camInput.Camera.Move.started += MoveCamera;
            camInput.Camera.Move.performed += MoveCamera;
            camInput.Camera.Move.canceled += MoveCamera;
        }

        private void MoveCamera(InputAction.CallbackContext context)
        {
            if (context.started)
            {

            }
            if (context.performed)
            {
                moveDirection = context.ReadValue<Vector2>();
            }
            if (context.canceled)
            {
                moveDirection = Vector2.zero;
            }
        }
    }

}
