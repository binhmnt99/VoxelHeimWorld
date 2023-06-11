using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject actionCameraGameObject;
        [SerializeField] private Vector3 _actionCameraOffset;

        void Start()
        {
            BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
            BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;

            HideActionCamera();
        }

        private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction:
                    HideActionCamera();
                    break;
                default:
                    break;
            }
        }

        private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
        {
            switch (sender)
            {
                case ShootAction shootAction:
                    Unit shooterUnit = shootAction.GetUnit();
                    Unit targetUnit = shootAction.GetTargetUnit();
                    Vector3 cameraCharacterHeight = Vector3.up * _actionCameraOffset.y;
                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                    float shoulderOffsetAmount = _actionCameraOffset.x;
                    Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;
                    Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight 
                        + shoulderOffset + shootDirection * _actionCameraOffset.z;
                    actionCameraGameObject.transform.position = actionCameraPosition;
                    actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                    ShowActionCamera();
                    break;
                default:
                    break;
            }
        }

        private void ShowActionCamera()
        {
            actionCameraGameObject.SetActive(true);
        }
        private void HideActionCamera()
        {
            actionCameraGameObject.SetActive(false);
        }
    }

}