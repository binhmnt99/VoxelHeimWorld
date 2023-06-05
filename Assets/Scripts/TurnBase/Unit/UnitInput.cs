using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TurnBase
{
    public class UnitInput : MonoBehaviour
    {
        private UnitInputController unitInput;

        public bool isLeftClicked { get; private set; }
        // Start is called before the first frame update
        void Awake()
        {
            unitInput = new UnitInputController();
            unitInput.Unit.Enable();
            GetLeftMouse();
        }

        private void GetLeftMouse()
        {
            unitInput.Unit.MoveClick.started += LeftMouse;
            unitInput.Unit.MoveClick.performed += LeftMouse;
            unitInput.Unit.MoveClick.canceled += LeftMouse;
        }

        private void LeftMouse(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isLeftClicked = true;

            }
            if (context.performed)
            {

            }
            if (context.canceled)
            {
                isLeftClicked = false;
            }
        }
    }
}

