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

        public bool isMoveClicked { get; private set; }
        public bool isSpinClicked { get; private set; }

        void Awake()
        {
            unitInput = new UnitInputController();
            unitInput.Unit.Enable();
            GetMoveButton();
            GetSpinButton();
        }

        private void GetSpinButton()
        {
            unitInput.Unit.SpinClick.started += RightMouseButton;
            unitInput.Unit.SpinClick.performed += RightMouseButton;
            unitInput.Unit.SpinClick.canceled += RightMouseButton;
        }

        private void RightMouseButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isSpinClicked = true;

            }
            if (context.performed)
            {

            }
            if (context.canceled)
            {
                isSpinClicked = false;
            }
        }

        private void GetMoveButton()
        {
            unitInput.Unit.MoveClick.started += LeftMouseButton;
            unitInput.Unit.MoveClick.performed += LeftMouseButton;
            unitInput.Unit.MoveClick.canceled += LeftMouseButton;
        }

        private void LeftMouseButton(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                isMoveClicked = true;

            }
            if (context.performed)
            {

            }
            if (context.canceled)
            {
                isMoveClicked = false;
            }
        }
    }
}

