using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class ScreenShakeAction : MonoBehaviour
    {
        void Start()
        {
            ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        }

        private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            ScreenShake.Instance.Shake();
        }
    }

}