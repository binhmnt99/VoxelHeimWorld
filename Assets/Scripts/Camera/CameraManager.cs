using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject actionCameraGameObject;
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
                    Vector3 cameraCharacterHeight = (Vector3.up + Vector3.left) * 2.5f;
                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                    float shoulderOffsetAmount = 0.5f;
                    Vector3 shoulderOffet = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;
                    Vector3 actionCameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffet + shootDirection * -1;
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